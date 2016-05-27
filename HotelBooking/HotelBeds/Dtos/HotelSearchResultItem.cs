using HotelBeds.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBeds.Dtos
{
    public class HotelSearchResultItem
    {
        public HotelInfo HotelInfo { get; set; }

        public TMoney Price { get; set; }

        public decimal Distance { get; set; }
    }
}
