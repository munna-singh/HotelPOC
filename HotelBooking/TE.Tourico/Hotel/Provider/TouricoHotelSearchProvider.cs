using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TE.Core.Tourico.Hotel.Handler;
using TE.Core.ServiceCatalogues.HotelCatalog.Dtos;
using TE.Core.ServiceCatalogues.HotelCatalog.Provider;
using TE.Core.ServiceCatalogues.HotelCatalog;

namespace TE.Tourico.Hotel
{
   public class TouricoHotelSearchProvider : IHotelSearchProvider
    {
        public HotelPropertyProviderRes RetrieveHotelInfo(string hotelCode)
        {
            //HotelPropertyDescription call to return both 
            var request = new TE.Core.ServiceCatalogues.HotelCatalog.Provider.HotelPropertyProviderReq
            {
                HotelCode = hotelCode
            };

            var propertyInfo = new TouricoRetrieveHotelPropertyHandler().Execute(request);

            //propertyInfo.HotelInfo.PropertyInformation = propertyInfo.HotelDetails;
            //propertyInfo.HotelInfo = propertyInfo.HotelInfo;

            return propertyInfo;
        }

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
            var response = new TouricoHotelAvailabilityHandler().Execute(request);
            return response;
        }

    }
}
