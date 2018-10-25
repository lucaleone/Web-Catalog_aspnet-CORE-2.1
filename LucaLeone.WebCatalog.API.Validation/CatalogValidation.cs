namespace LucaLeone.WebCatalog.API.Validation
{
    public class CatalogValidation : ICatalogValidation
    {
        public bool ValidateSearch(uint minPrice, uint? maxPrice)
        {
            if (maxPrice.HasValue)
                return minPrice <= maxPrice;
            return true;
        }

        public bool ValidateGetCatalog(int page, int maxNumElem)
        {
            const int maxElemToReturn = 50;
            return page > 0 && maxNumElem <= maxElemToReturn;
        }
    }
}