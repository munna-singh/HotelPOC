//using HotelBeds.ServiceCatalogues.HotelCatalog.Dtos.Controller;
using HotelBeds.ServiceCatalogues.HotelCatalog.Dtos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HotelBeds.ServiceCatalogues.HotelCatalog.Enums;

namespace HotelBeds.ServiceCatalogues.HotelCatalog.Provider
{
    public class HotelAvailabilityProviderReq
    {
        /// <summary>
        ///  Destination location Id.
        ///  This could be city, location, airport code etc..
        /// </summary>
        [Required]
        public string LocationId { get; set; }

        /// <summary>
        /// Type of the address selected during search
        /// </summary>
        public LocationTypes LocationType { get; set; }

        /// <summary>
        ///  Checkin date
        /// </summary>
        [Required]
        public DateTime CheckInDate { get; set; }

        /// <summary>
        ///  Checkout date
        /// </summary>
        [Required]
        public DateTime CheckOutDate { get; set; }

        /// <summary>
        ///  Guest which age is >= 18
        /// </summary>
        [Required]
        [Range(1, 9)]
        public int TotalAdults { get; set; }

        /// <summary>
        ///  Latitude and longitude of the destination
        /// </summary>
        public GeoLocation GeoLocation { get; set; }

        /// <summary>
        /// Selected hotels which will come from database cache
        /// </summary>
        public List<string> HotelCodes { get; set; }

        /// <summary>
        /// Currency to be used
        /// </summary>
        [Required]
        public string CurrencyCode { get; set; }

        /// <summary>
        /// Hotel minimum Rating
        /// </summary>
        [Required]
        [Range(0, 5)]
        public int MinRating { get; set; }

        /// <summary>
        /// Hotel maximum Rating
        /// </summary>
        [Required]
        [Range(0, 5)]
        public int MaxRating { get; set; }

        public int TotalRooms { get; set; }
    }
}
