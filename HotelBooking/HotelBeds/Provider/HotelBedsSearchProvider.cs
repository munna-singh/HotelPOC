using HotelBeds.Contract;
using HotelBeds.Dtos;
using HotelBeds.Handlers;
using HotelBeds.ServiceCatalogues.HotelCatalog.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBeds.Provider
{
    public class HotelBedsSearchProvider : IHotelSearchProvider
    {
        public HotelAvailabilityProviderRes Search(HotelAvailabilityProviderReq request)
        {
            var response = new HotelBedsAvailabilityHandler().Execute(request);
            return response;
        }

    }
}
