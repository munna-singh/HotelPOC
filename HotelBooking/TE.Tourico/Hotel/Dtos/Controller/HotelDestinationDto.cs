using System.ComponentModel.DataAnnotations;
using TE.Core.ServiceCatalogues.HotelCatalog.Enums;

namespace TE.Core.Tourico.Hotel.Dtos.Controller
{
    public class HotelDestinationDto
    {
        /// <summary>
        /// Name of the destination.
        /// </summary>
        [StringLength(100, MinimumLength = 3)]
        public string Name { get; set; }

        /// <summary>
        /// Type of the destination.
        /// </summary>
        [Required]
        public LocationTypes DestinationType { get; set; }
    }
}
