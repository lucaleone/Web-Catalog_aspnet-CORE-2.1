using System;
using System.Collections.Generic;
using System.Text;

namespace LucaLeone.WebCatalog.API.Validation
{
    public interface ICatalogValidator
    {
        void ValidateSearch(uint minPrice, uint? maxPrice);
        bool ValidateGetCatalog(int page, int maxNumElem);
    }
}
