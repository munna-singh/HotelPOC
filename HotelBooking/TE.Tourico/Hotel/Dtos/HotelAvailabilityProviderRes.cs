using System.Collections.Generic;
using TE.Core.ServiceCatalogues.HotelCatalog.Dtos.Controller;
using TE.Core.Tourico.Hotel.Dtos.Controller;

namespace TE.Core.Tourico.Hotel.Dtos
{
    public class HotelAvailabilityProviderRes
    {
        /// <summary>
        /// Contains available hotels returned/filled up by the hotel provider(s)
        /// </summary>
        public List<HotelSearchResultItem> Hotels { get; set; }
    }
}
