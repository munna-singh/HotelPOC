using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBeds.ServiceCatalogues.HotelCatalog.Dtos
{
    public class AddressDto
    {
        public int? AddressId { get; set; }

        //[MaxLength(80, ErrorMessageResourceName = "Val_MaxLengthX_Default", ErrorMessageResourceType = typeof(UserMessages))]
        public string Address1 { get; set; }

        //[MaxLength(80, ErrorMessageResourceName = "Val_MaxLengthX_Default", ErrorMessageResourceType = typeof(UserMessages))]
        public string Address2 { get; set; }

        //[ValidateObject]
        public CountryLookupDto Country { get; set; }

        //[ValidateObject]
        public SubdivisionLookupDto Subdivision { get; set; }

        //[MaxLength(80, ErrorMessageResourceName = "Val_MaxLengthX_Default", ErrorMessageResourceType = typeof(UserMessages))]
        public string City { get; set; }

        //[MaxLength(12, ErrorMessageResourceName = "Val_MaxLengthX_Default", ErrorMessageResourceType = typeof(UserMessages))]
        public string Zip { get; set; }
    }
}
