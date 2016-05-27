using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBeds.ServiceCatalogues.HotelCatalog.Dtos
{
    public class SubdivisionLookupDto
    {
        [Key]
        [Range(1, Int32.MaxValue)]
        [Required]
        public int SubdivisionId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }

        public override string ToString()
        {
            return "SubdivisionId: " + SubdivisionId + ", Name: " + Name + ", Code:" + Code;
        }
    }
}
