using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TE.Core.Tourico.Hotel.Dtos;
using TE.Core.Tourico.Hotel.Handler;
using TE.Core.Tourico.Hotel.ServiceCatlog;
using TE.Core.ServiceCatalogues.HotelCatalog.Dtos;
using TE.Core.ServiceCatalogues.HotelCatalog.Provider;

namespace TE.Tourico.Hotel
{
   public class TouricoHotelSearchProvider : IHotelSearchProvider, IHotelDetailsProvider
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

        public HotelAvailabilityProviderRes Search(HotelAvailabilityProviderReq request)
        {
            var response = new TouricoHotelAvailabilityHandler().Execute(request);
            return response;
        }

        HotelInfo IHotelDetailsProvider.RetrieveHotelInfo(string hotelCode)
        {
            throw new NotImplementedException();
        }
    }
}
