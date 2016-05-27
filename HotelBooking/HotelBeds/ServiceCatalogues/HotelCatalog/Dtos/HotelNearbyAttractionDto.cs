using HotelBeds.ServiceCatalogues.HotelCatalog.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBeds.ServiceCatalogues.HotelCatalog.Dtos
{
    public class HotelNearbyAttractionDto
    {
        /// <summary>
        /// Name of attraction
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Distance from the hotel
        /// </summary>
        public decimal? Distance { get; set; }
        /// <summary>
        /// Distance measurement KM/MI
        /// </summary>
        public DistanceMeasureType? DistanceUnit { get; set; }
        /// <summary>
        /// Directions from the hotel. It could be N, S, E, W
        /// </summary>
        public string Direction { get; set; }

    }
}
