using HotelBeds.Dtos;
using HotelBeds.ServiceCatalogues.HotelCatalog.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBeds.Contract
{
    public interface IHotelSearchProvider
    {
        HotelAvailabilityProviderRes Search(HotelAvailabilityProviderReq request);
    }
}
