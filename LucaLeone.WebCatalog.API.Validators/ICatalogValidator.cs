namespace LucaLeone.WebCatalog.API.Validators
{
    public interface ICatalogValidator
    {
        void ValidateSearch(uint minPrice, uint? maxPrice);
        bool ValidateGetCatalog(int page, int maxNumElem);
    }
}
