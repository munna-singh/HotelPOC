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
   public class TouricoHotelSearchProvider : IHotelSearchProvider,  IHotelDetailsProvider
    {
        //SearchHotelById - returns list of Hotels  based on hotelCodes[]
        public HotelAvailabilityProviderRes Search(HotelAvailabilityProviderReq request) 
        {
            var response = new TouricoHotelAvailabilityHandler().Execute(request);
            return response;
        }

        //Call to GetHotelDetailsV3 
        public HotelInfo RetrieveHotelInfo(string hotelCode)
        {            
            var request = new HotelPropertyProviderReq
            {
                HotelCode = hotelCode
            };
            
            
            var propertyInfo = new TouricoRetrieveHotelPropertyHandler().Execute(request);
            
            propertyInfo.HotelInfo = propertyInfo.HotelInfo;

            propertyInfo.HotelInfo.PropertyInformation = propertyInfo.HotelDetails;


            return propertyInfo.HotelInfo;
        }

        //Call to GetHotelDetailsV3 
        public HotelPropertyProviderRes RetrieveHotelInfos(string hotelCode)
        {
            var request = new HotelPropertyProviderReq
            {
                HotelCode = hotelCode
            };


            var propertyInfo = new TouricoRetrieveHotelPropertyHandler().Execute(request);

            return propertyInfo;
        }

        //public HotelPropertyProviderRes RetrieveHotelInfo(string hotelCode)
        //{
        //    //HotelPropertyDescription call to return both 
        //    var request = new TE.Core.ServiceCatalogues.HotelCatalog.Provider.HotelPropertyProviderReq
        //    {
        //        HotelCode = hotelCode
        //    };

        //    var propertyInfo = new TouricoRetrieveHotelPropertyHandler().Execute(request);

        //    //propertyInfo.HotelInfo.PropertyInformation = propertyInfo.HotelDetails;
        //    //propertyInfo.HotelInfo = propertyInfo.HotelInfo;

        //    return propertyInfo;
        //}


        public IEnumerable<HotelRateProviderRes> RetrieveHotelRates(HotelPropertyProviderReq request) //SearchHotelsById - List of Rates
        {
            var propertyInfo = new TouricoRetrieveHotelRateHandler().Execute(request);

            if (propertyInfo.HotelRates == null || !propertyInfo.HotelRates.Any())
            {
                throw new ApplicationException("No rates were returned by the HotelPropertyDescription API.");
            }

            return propertyInfo.HotelRates;
        }

        public HotelRateProviderRes RetrieveHotelRateDetails(HotelRateProviderReq request) //CheckAvailabilityAndPrices - one Rate based on hotelRoomTypeId (set in RateCode param)
        {
            return new TouricoRetrieveHotelRatesHandler().Execute(request);   
        }
              
             
    }
}
