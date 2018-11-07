using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.DataAnnotations;
using FluentAssertions;
using LucaLeone.WebCatalog.API.Controllers;
using LucaLeone.WebCatalog.API.DTO;
using LucaLeone.WebCatalog.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace LucaLeone.WebCatalog.API.Tests
{
    public class CatalogControllerTests
    {

        private readonly IFixture _fixture;
        private readonly Bogus.Faker _faker;
        public CatalogControllerTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _fixture.Register(() => new Mock<ICatalogService>());
            _fixture.Register(() => new Mock<ILogger>());

            _faker = new Bogus.Faker();
        }

        #region GetCatalog
        [Fact]
        public async void GetCatalog_WithGoodModel_ShouldReturnOKResult()
        {
            const int minModelVal = 1;
            const int maxModelVal = 50;
            _fixture.Customizations.Add(
                new RandomNumericSequenceGenerator(minModelVal, maxModelVal));
            var getCatalog = _fixture.Create<GetCatalogDto>();
            var successful = _fixture.Create<IEnumerable<ProductDto>>();
            _fixture.Freeze<Mock<ICatalogService>>()
                    .Setup(x => x.GetCatalogPageAsync(It.IsAny<GetCatalogDto>()))
                    .ReturnsAsync(successful);
            var controller = _fixture.Create<CatalogController>();

            //act, assert
            bool valid = Validator.TryValidateObject(getCatalog,
                new ValidationContext(getCatalog),
                new List<ValidationResult>(),
                true);
            Assert.True(valid);
            var response = await controller.GetCatalog(getCatalog);
            Assert.IsType<OkObjectResult>(response);
            Assert.IsAssignableFrom<IEnumerable<ProductDto>>(
                (response as OkObjectResult).Value);
            Assert.NotNull((response as OkObjectResult).Value);
        }

        [Fact]
        public void GetCatalog_BadModel_ShouldReturnBadRequestResult()
        {
            _fixture.Customize(new NoDataAnnotationsCustomization());
            const int minModelVal = 51;
            const int maxModelVal = int.MaxValue;
            _fixture.Customizations.Add(
                new RandomNumericSequenceGenerator(minModelVal, maxModelVal));
            var getCatalog = _fixture.Create<GetCatalogDto>();
            //act, assert
            bool valid = Validator.TryValidateObject(getCatalog,
                new ValidationContext(getCatalog),
                new List<ValidationResult>(),
                true);
            Assert.False(valid);
        }
        #endregion

        #region Search
        [Fact]
        public async void Search_WithGoodModel_ShouldReturnOKResult()
        {
            _fixture.Customizations.Add(
                new RandomNumericSequenceGenerator(1, int.MaxValue));
            var price1 = _fixture.Create<int>();
            var price2 = _fixture.Create<int>();
            var requestContent = _fixture.Build<SearchDto>()
                .With(x => x.MinPrice, Math.Min(price1, price2))
                .With(x => x.MaxPrice, Math.Max(price1, price2))
                .Create();
            var successful = _fixture.Create<IEnumerable<ProductDto>>();
            _fixture.Freeze<Mock<ICatalogService>>()
                    .Setup(x => x.SearchProductsAsync(It.IsAny<SearchDto>()))
                    .ReturnsAsync(successful);
            var controller = _fixture.Create<CatalogController>();

            //act, assert
            bool valid = Validator.TryValidateObject(requestContent,
               new ValidationContext(requestContent),
               new List<ValidationResult>(),
               true);
            Assert.True(valid);
            var response = await controller.Search(requestContent);
            Assert.IsType<OkObjectResult>(response);
            Assert.NotNull((response as OkObjectResult).Value);
            Assert.IsAssignableFrom<IEnumerable<ProductDto>>(
                (response as OkObjectResult).Value);
        }

        [Fact]
        public void Search_WithNegativePricesModel_ShouldReturnBadRequestResult()
        {
            _fixture.Customize(new NoDataAnnotationsCustomization());
            _fixture.Customizations.Add(
                 new RandomNumericSequenceGenerator(int.MinValue, 0));
            var requestContent = _fixture.Create<SearchDto>();

            //act, assert
            bool valid = Validator.TryValidateObject(requestContent,
                new ValidationContext(requestContent),
                new List<ValidationResult>(),
                true);
            Assert.False(valid);
        }

        [Fact]
        public void Search_WithInvalidPricesModel_ShouldReturnBadRequestResult()
        {
            _fixture.Customize(new NoDataAnnotationsCustomization());
            _fixture.Customizations.Add(
                  new RandomNumericSequenceGenerator(1, int.MaxValue));
            var price1 = _fixture.Create<int>();
            var price2 = _fixture.Create<int>();
            var requestContent = _fixture.Build<SearchDto>()
                .With(x => x.MinPrice, Math.Max(price1, price2))
                .With(x => x.MaxPrice, Math.Min(price1, price2))
                .Create();

            //act, assert
            bool valid = Validator.TryValidateObject(requestContent,
                new ValidationContext(requestContent),
                new List<ValidationResult>(),
                true);
            Assert.False(valid);
        }
        #endregion

        #region GetProduct
        [Fact]
        public async void GetProduct_ExistingProduct_ShouldReturnOKResult()
        {
            var successful = _fixture.Create<ProductDto>();
            _fixture.Freeze<Mock<ICatalogService>>()
                    .Setup(x => x.GetProduct(It.IsAny<Guid>()))
                    .ReturnsAsync(successful);
            var controller = _fixture.Create<CatalogController>();

            //act
            var response = await controller.GetProduct(_fixture.Create<Guid>());

            //assert
            Assert.IsType<OkObjectResult>(response);
            Assert.NotNull((response as OkObjectResult).Value);
            Assert.IsAssignableFrom<ProductDto>(
                (response as OkObjectResult).Value);
        }

        [Fact]
        public async void GetProduct_NotExistingProduct_ShouldReturnNotFoundResult()
        {
            ProductDto unsuccessful = null;
            _fixture.Freeze<Mock<ICatalogService>>()
                    .Setup(x => x.GetProduct(It.IsAny<Guid>()))
                    .ReturnsAsync(unsuccessful);
            var controller = _fixture.Create<CatalogController>();
            var productId = _fixture.Create<Guid>();

            //act
            var response = await controller.GetProduct(productId);

            //assert
            Assert.IsType<NotFoundObjectResult>(response);
            Assert.NotNull((response as NotFoundObjectResult).Value);
            Assert.IsAssignableFrom<Guid>(
                (response as NotFoundObjectResult).Value);
        }
        #endregion

        #region AddProduct
        [Fact]
        public async void AddProduct_WithGoodModel_ShouldReturnOkResult()
        {
            var requestContent = _fixture.Build<ProductDto>()
                .With(x => x.Photo, _fixture.Create<Uri>().ToString())
                .Create();
            var succesful = _fixture.Create<Guid>();
            _fixture.Freeze<Mock<ICatalogService>>()
                    .Setup(x => x.AddProductAsync(It.IsAny<ProductDto>()))
                    .ReturnsAsync(succesful);
            _fixture.Freeze<Mock<IUrlHelper>>()
                     .Setup(x => x.Link(It.IsAny<string>(),
                     It.IsAny<object>()))
                     .Returns(succesful.ToString());
            var controller = _fixture.Create<CatalogController>();

            //act
            bool valid = Validator.TryValidateObject(requestContent,
                new ValidationContext(requestContent),
                new List<ValidationResult>(),
                true);
            var response = await controller.AddProduct(requestContent);

            //assert
            Assert.True(valid);
            Assert.IsType<CreatedResult>(response);
            Assert.NotNull((response as CreatedResult).Value);
            Assert.IsAssignableFrom<ProductDto>((response as CreatedResult).Value);
            Assert.Equal(requestContent, (response as CreatedResult).Value);
            Assert.Contains(succesful.ToString(), (response as CreatedResult).Location);
        }

        [Fact]
        public void AddProduct_WithInvalidModel_ShouldReturnBadRequestResult()
        {
            var requestContent = _fixture.Build<ProductDto>()
                .With(x => x.Photo, _fixture.Create<int>().ToString())
                .Create();
            //act
            bool valid = Validator.TryValidateObject(requestContent,
                new ValidationContext(requestContent),
                new List<ValidationResult>(),
                true);

            //assert
            Assert.False(valid);
        }
        #endregion
    }
}
