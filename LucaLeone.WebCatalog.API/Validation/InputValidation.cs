using LucaLeone.WebCatalog.Models;

namespace LucaLeone.WebCatalog.Validation
{
    public static class InputValidation
    {
        public static bool ValidateSearchInput(ref string name, int? minPrice, int? maxPrice)
        {
            if (!string.IsNullOrEmpty(name))
                name = name.Trim();
            return !string.IsNullOrWhiteSpace(name) || minPrice.HasValue || minPrice >= 0 ||
                   maxPrice.HasValue || maxPrice >= 0 || minPrice.HasValue && maxPrice.HasValue ||
                   minPrice < maxPrice;
        }

        public static bool ValidateGetCatalogInput(int page, ref int maxNumElem)
        {
            if (maxNumElem > 50)
                maxNumElem = 50;

            return page > 0;
        }

        public static bool ValidateProductInput(NewProduct product)
        {
            return product != null && product.Price > 0 && !string.IsNullOrWhiteSpace(product.Name);
        }
    }
}