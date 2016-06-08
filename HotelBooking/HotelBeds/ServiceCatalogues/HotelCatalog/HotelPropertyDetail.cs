//using TE.HotelBeds.ServiceCatalogues.HotelCatalog.Dtos;
//using TE.HotelBeds.ServiceCatalogues.HotelCatalog.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TE.Core.ServiceCatalogues.HotelCatalog.Dtos;
using TE.Core.ServiceCatalogues.HotelCatalog.Provider.Enums;

namespace TE.HotelBeds.ServiceCatalogues.HotelCatalog
{
    public class HotelPropertyDetail
    {
        public HotelPropertyDetail()
        {
            HotelFullAddress = new List<string>();
            AwardProvider = new List<string>();
            HotelOptions = new List<HotelOptionTypes>();
            HotelPropertyTypes = new List<PropertyTypes>();
            Attractions = new List<HotelNearbyAttractionDto>();
            CancellationPolicies = new List<string>();
            Directions = new List<string>();
            HotelFacilities = new List<string>();
        }

        /// <summary>
        /// Full adress of the property
        /// </summary>
        public List<string> HotelFullAddress { get; set; }

        /// <summary>
        /// Hotel Latitude Info
        /// </summary>
        public decimal Latitude { get; set; }

        /// <summary>
        /// Hotel Longitude Info
        /// </summary>
        public decimal Longitude { get; set; }

        /// <summary>
        /// No Of Floors
        /// </summary>
        public int NumberOfFloors { get; set; }

        /// <summary>
        /// Hotel Award Provider
        /// </summary>
        public List<string> AwardProvider { get; set; }

        /// <summary>
        /// Fax No
        /// </summary>
        public string HotelFaxNumber { get; set; }

        /// <summary>
        /// List of available Hotel Options 
        /// </summary>
        public List<HotelOptionTypes> HotelOptions { get; set; }

        /// <summary>
        /// List of property types
        /// </summary>
        public List<PropertyTypes> HotelPropertyTypes { get; set; }

        /// <summary>
        /// List of hotel attractions with distance information
        /// </summary>
        public List<HotelNearbyAttractionDto> Attractions { get; set; }

        /// <summary>
        /// Hotel Cancellation Policy
        /// </summary>
        public List<string> CancellationPolicies { get; set; }

        /// <summary>
        /// List of available direction to reach the hotel
        /// </summary>
        public List<string> Directions { get; set; }

        /// <summary>
        /// List of available facilities
        /// </summary>
        public List<string> HotelFacilities { get; set; }
    }
}
