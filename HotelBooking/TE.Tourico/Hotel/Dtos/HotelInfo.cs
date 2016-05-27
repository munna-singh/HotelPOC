using System.Collections.Generic;
using TE.Core.ServiceCatalogues.HotelCatalog.Dtos;
using TE.Core.ServiceCatalogues.HotelCatalog.Enums;
using TE.Core.ServiceCatalogues.HotelCatalog.Provider.Enums;
using TE.Core.Shared.Dtos;
using TE.DataAccessLayer.Enums;

namespace TE.Core.Tourico.Hotel.Dtos
{
    public class HotelInfo
    {
        /// <summary>
        /// Hotel provider.
        /// </summary>
        public ProviderTypes Provider { get; set; }

        /// <summary>
        /// Name of the hotel.
        /// </summary>
        public string HotelName { get; set; }

        /// <summary>
        ///  Hotel Code
        /// </summary>
        public string HotelCode { get; set; }


        /// <summary>
        /// Hotel Chain code
        /// </summary>
        public string HotelChainCode { get; set; }

        /// <summary>
        ///  City Name
        /// </summary>
        public string CityCode { get; set; }

        /// <summary>
        ///  City Name
        /// </summary>
        public string CityName { get; set; }

        /// <summary>
        ///  Location Description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///  Hotel Address
        /// </summary>
        public AddressDto Address { get; set; }

        /// <summary>
        ///   Hotel Phone Number
        /// </summary> 
        public string PhoneNumber { get; set; }

        /// <summary>
        ///  Current offer run by the hotel
        /// </summary>
        public string SpecialOffers { get; set; }

        /// <summary>
        ///  Hotel Rating
        /// </summary>  
        public int? Rating { get; set; }

        /// <summary>
        ///     Hotel rating type
        /// </summary> 
        public RatingTypes RatingType { get; set; }

        /// <summary>
        /// Homepage Url (Source TBD).
        /// </summary>
        public string HomepageUrl { get; set; }
        

        /// <summary>
        /// Hero Image URL.
        /// </summary>
        public string HeroImageUrl { get; set; }
      
        public string Thumbnails { get; set; }

        /// <summary>
        /// Contact Email address (Source TBD).
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Geographic Location (for plotting on maps).
        /// </summary>
        public GeoLocation Location { get; set; }

        /// <summary>
        /// List of available amentities.
        /// </summary>
        public Dictionary<HotelOptionTypes, bool> Amenities { get; set; }

        /// <summary>
        /// Contains more detailed information for the hotel
        /// </summary>
        public HotelPropertyDetail PropertyInformation { get; set; }
    }
}