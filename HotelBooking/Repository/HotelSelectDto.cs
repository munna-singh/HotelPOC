using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class HotelSelectDto
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string TotalTravellers { get; set; }
        public string HotelCode { get; set; }
        public string SessionId { get; set; }
        public string propertyRphNumber { get; set; }
        public string RPHNumber { get; set; }
    }
}
