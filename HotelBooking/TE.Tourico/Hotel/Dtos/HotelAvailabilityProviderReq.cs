using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TE.Core.ServiceCatalogues.HotelCatalog.Dtos;
using TE.Core.ServiceCatalogues.HotelCatalog.Enums;
using TE.DataAccessLayer.Models;


namespace TE.Core.Tourico.Hotel.Dtos
{
    /// <summary>
    /// This class will hold minimal search reqeust information
    /// which will be passed to the provider for searching a hotel
    /// </summary>
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
        [Range(1,9)]
        public int TotalAdults { get; set; }

        /// <summary>
        ///  Latitude and longitude of the destination
        /// </summary>
        public GeoLocation? GeoLocation { get; set; }

        /// <summary>
        /// Selected hotels which will come from database cache
        /// </summary>
        public List<string> HotelCodes { get; set; }

        /// <summary>
        /// Currency to be used
        /// </summary>
        [Required]
        public TCurrency CurrencyCode { get; set; }

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
    }
}
