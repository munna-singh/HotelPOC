using TE.DataAccessLayer.Models;

namespace TE.Core.Tourico.Hotel.Dtos.Controller
{
    public class HotelSearchResultItem
    {
        
        public HotelInfo HotelInfo { get; set; }

        public TMoney Price { get; set; }

        public decimal Distance { get; set; }
    }
}