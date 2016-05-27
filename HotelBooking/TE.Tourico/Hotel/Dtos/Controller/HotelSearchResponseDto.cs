using System.Collections.Generic;
using TE.Core.ServiceCatalogues.HotelCatalog.Dtos.Controller;

namespace TE.Core.Tourico.Hotel.Dtos.Controller
{
    public class HotelSearchResponseDto
    {
        public HotelSearchResponseDto()
        {
            Hotels = new List<HotelSearchResultItem>();
        }

        public IList<HotelSearchResultItem> Hotels { get; set; }
    }

    public enum SortBy
    {
        Price = 1,

        HotelName,

        Rating,

        Distance
    }

    public enum SortOrder
    {
        Ascending,

        Descending
    }
}
