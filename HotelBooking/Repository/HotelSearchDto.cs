using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class HotelSearchDto
    {
        public string Address { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string TotalGuest { get; set; }
        public string TotalRoom { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
