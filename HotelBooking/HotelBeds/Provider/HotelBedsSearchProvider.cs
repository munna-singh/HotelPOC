using System;
using System.Collections.Generic;
using Common;
using HotelBeds.Handlers;
using TE.Core.ServiceCatalogues.HotelCatalog;
using TE.Core.ServiceCatalogues.HotelCatalog.Provider; 
namespace HotelBeds.Provider
{
    public class HotelBedsSearchProvider : IHotelSearchProvider
    {
        public HotelRateProviderRes RetrieveHotelRateDetails(HotelRateProviderReq request)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<HotelRateProviderRes> RetrieveHotelRates(HotelPropertyProviderReq request)
        {
            throw new NotImplementedException();
        }


        public HotelAvailabilityProviderRes Search(HotelAvailabilityProviderReq request)
        {
            var response = new HotelBedsAvailabilityHandler().Execute(request);
            return response;
        }

    }
}
