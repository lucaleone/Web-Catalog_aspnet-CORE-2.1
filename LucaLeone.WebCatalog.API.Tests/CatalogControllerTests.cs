using System.Collections.Generic;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using LucaLeone.WebCatalog.API.Controllers;
using LucaLeone.WebCatalog.API.DTO;
using LucaLeone.WebCatalog.API.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace LucaLeone.WebCatalog.API.Tests
{
    public class CatalogControllerTests
    {

        private readonly IFixture _fixture;
        public CatalogControllerTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _fixture.Register(() => new Mock<ICatalogService>());
            _fixture.Register(() => new Mock<ILogger>());
        }
        [Fact]
        public async void GetCatalog_ShouldReturnOKResult()
        {
            var successful = _fixture.Create<IEnumerable<ProductDto>>();
            _fixture.Freeze<Mock<ICatalogService>>()
                    .Setup(x => x.GetCatalogPageAsync(5, 8))
                    .ReturnsAsync(successful);

            var controller = _fixture.Create<CatalogController>();

            //act
            var response = await controller.GetCatalog();

            //assert
            response.Should()
                    .BeOfType<IEnumerable<ProductDto>>();
        }
    }
}
