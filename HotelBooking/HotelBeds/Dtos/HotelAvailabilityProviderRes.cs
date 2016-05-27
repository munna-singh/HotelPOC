using com.hotelbeds.distribution.hotel_api_model.auto.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBeds.Dtos
{
    public class HotelAvailabilityProviderRes
    {

        public List<HotelSearchResultItem> Hotels { get; set; }

        //public List<Hotel> Hotels { get; set; }
    }
}
