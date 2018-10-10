namespace LucaLeone.WebCatalog.MVC.Validation
{
    public class CatalogInputValidation
    {
        public static bool ValidateSearchInput(string name, int? minPrice, int? maxPrice)
        {
            return !string.IsNullOrWhiteSpace(name) || minPrice.HasValue || minPrice >= 0 ||
                   maxPrice.HasValue || maxPrice >= 0 || minPrice.HasValue && maxPrice.HasValue ||
                   minPrice < maxPrice;
        }

        public static void ValidateGetCatalogInput(ref int page, ref int maxNumElem)
        {
            page = page < 1 ? 1 : page;
            maxNumElem = maxNumElem < 10 ? 10 : maxNumElem;
            maxNumElem = maxNumElem > 50 ? 50 : maxNumElem;
        }
    }
}