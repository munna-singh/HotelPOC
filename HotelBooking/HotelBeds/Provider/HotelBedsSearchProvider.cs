using System;
using System.Collections.Generic;
using Common;
using TE.HotelBeds.Handlers;
using TE.Core.ServiceCatalogues.HotelCatalog;
using TE.Core.ServiceCatalogues.HotelCatalog.Provider; 
namespace TE.HotelBeds.Provider
{
    public class HotelBedsSearchProvider : IHotelSearchProvider
    {
        public HotelRateProviderRes RetrieveHotelRateDetails(HotelRateProviderReq request)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<HotelRateProviderRes> RetrieveHotelRates(HotelPropertyProviderReq request)
        {
            List<HotelRateProviderRes> abccc = new List<HotelRateProviderRes>();
            var response = new HotelBedsDetailsHandler().Execute(request);
            return abccc;
        }


        public HotelAvailabilityProviderRes Search(HotelAvailabilityProviderReq request)
        {
            var response = new HotelBedsAvailabilityHandler().Execute(request);
            return response;
        }

    }
}
