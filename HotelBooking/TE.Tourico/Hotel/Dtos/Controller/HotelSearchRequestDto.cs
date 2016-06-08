using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using TE.Common.Validation;
using TE.Core.Providers;
using TE.Core.ServiceCatalogues.TravelCatalogues.Dtos;
using TE.Core.ServiceCatalogues.HotelCatalog.Dtos;

namespace TE.Core.Tourico.Hotel.Dtos.Controller
{
    public class HotelSearchRequestDto : AvailabilityCriterionDtoBase
    {
        public HotelSearchRequestDto()
        {
            //default provider type
            ProviderType = HotelSearchProviderTypes.Tourico;
        }

        #region Prepopulate Quote

        /// <summary>
        /// Specifies the agent to use when creating a quote from the search results.
        /// </summary>
        [Required]
        [Range(1, int.MaxValue)]
        public int AgentId { get; set; }

        /// <summary>
        /// Optional, the client id to use when create a quote.
        /// </summary>
        [Range(1, int.MaxValue)]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? ClientId { get; set; }

        /// <summary>
        /// Optional, when adding to an existing quote.
        /// </summary>
        public int? QuoteId { get; set; }

        #endregion

        #region Search Criteria

        /// <summary>
        /// Used to find hotels near the specified destination
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [Required]
        public HotelDestinationDto Destination { get; set; }

        /// <summary>
        /// Used to find hotels near the specified geographic location.
        /// </summary>
        [Required]
        public GeoLocation Location { get; set; }
        
        /// <summary>
        /// Find hotels belonging to the given hotel chain.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [Display(Name = "Hotel Chain")]
        public string HotelChainId { get; set; }

        /// <summary>
        /// Find hotels with the matching name.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [Display(Name = "Hotel Name")]
        public string HotelName { get; set; }

        /// <summary>
        /// Find hotels with a minimum Northstar Travel Media (NTM) rating.
        /// </summary>
        [Range(0, 5)]
        [Display(Name = "Minimum Rating")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int MinRating { get; set; }

        /// <summary>
        /// Find hotels with a maxium Northstar Travel Media (NTM) rating.
        /// </summary>
        [Range(0, 5)]
        [Display(Name = "Maximum Rating")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int MaxRating { get; set; }

        #endregion

        #region Check In Information

        /// <summary>
        /// Check-in date for the room(s).
        /// </summary>
        [Required]
        [Display(Name = "Check In Date")]
        [FutureDateRange(TimeInFuture = false)]
        public DateTime CheckInDate { get; set; }

        /// <summary>
        /// Check-out date for the room(s).
        /// </summary>
        [Required]
        [Display(Name = "Check Out Date")]
        [FutureDateRange(TimeInFuture = false)]
        public DateTime CheckOutDate { get; set; }

        /// <summary>
        /// Number of rooms to book. Note each room must have the same number of adults.
        /// </summary>
        [Required]
        [Range(1, 4)]
        public int NumberOfRooms { get; set; }

        /// <summary>
        /// Number of adults per room.
        /// </summary>
        [Required]
        [Range(1, 4)]
        public int NumberOfAdults { get; set; }

        #endregion

        /// <summary>
        /// Provider type.
        /// </summary>
        /// <remarks>Currently only Sabre is supported.</remarks>
        public HotelSearchProviderTypes ProviderType { get; set; }
       

        public List<string> AgentRateCodes { get; set; }

        /// <summary>
        /// If this is false, local cache db will be queried before performing the API search.
        /// Incase of true, it will skip the local search and directly call the API with Lat and Long.
        /// </summary>
        public bool SkipLocalCache { get; set; }

        /// <summary>
        /// Indicate the currency code to use for returned pricing.
        /// </summary>
        public string Currency { get; set; }
    }
}
