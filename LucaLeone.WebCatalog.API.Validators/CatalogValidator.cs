using LucaLeone.WebCatalog.API.Exceptions;

namespace LucaLeone.WebCatalog.API.Validators
{
    public class CatalogValidator : ICatalogValidator
    {

        /// <summary>
        ///
        /// </summary>
        /// <exception cref="LucaLeone.WebCatalog.API.Exceptions.ValidationException"></exception>
        /// <param name="minPrice"></param>
        /// <param name="maxPrice"></param>
        public void ValidateSearch(uint minPrice, uint? maxPrice)
        {
            if (maxPrice.HasValue)
                if (minPrice <= maxPrice)
                    throw new ValidationException(new string[] {"Data non consistent: minPrice greater than maxPrice"});
        }

        public bool ValidateGetCatalog(int page, int maxNumElem)
        {
            const int maxElemToReturn = 50;
            var isValid = page > 0 && maxNumElem <= maxElemToReturn;
            return isValid;
        }
    }
}