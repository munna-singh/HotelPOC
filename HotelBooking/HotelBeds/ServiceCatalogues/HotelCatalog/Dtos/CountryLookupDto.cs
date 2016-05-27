using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBeds.ServiceCatalogues.HotelCatalog.Dtos
{
    public class CountryLookupDto
    {
        [Key]
        [Range(1, Int32.MaxValue)]
        public int CountryId { get; set; }

        public string Name { get; set; }
        public string CountryCode { get; set; }

        public override string ToString()
        {
            return "CountryId: " + CountryId + ", Name: " + Name;
        }
    }
}
