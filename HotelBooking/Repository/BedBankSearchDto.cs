using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class BedBankSearchDto
    {
        public string Address { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalGuest { get; set; }
        public int TotalRoom { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
