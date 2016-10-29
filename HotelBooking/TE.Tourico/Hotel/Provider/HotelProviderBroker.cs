using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using TE.Core.Providers.Leonardo;
using TE.Core.Providers.Sabre.Hotel;
using TE.Core.ServiceCatalogues.HotelCatalog;
using TE.Core.Services.TravelServices.HotelService.Provider;
using TE.DataAccessLayer.Enums;

namespace TE.Tourico.Hotel
{
    public enum HotelSearchProviderTypes
    {
        [Description("Sabre")]
        Sabre = 1,

        //msanka change - Added enum 
        [Description("Tourico")]
        Tourico = 2,

        //msanka change - Added enum 
        [Description("HotelBeds")]
        HotelBeds = 3

    }

    public class HotelProviderBroker
    {
        public static IHotelSearchProvider GetHotelSearchProvider(HotelSearchProviderTypes searchProviderType)
        {
            if (searchProviderType == HotelSearchProviderTypes.Sabre)
            {
               //return new SabreHotelSearchProvider();
            }

            //msanka change 
            if (searchProviderType == HotelSearchProviderTypes.Tourico)
            {
                return new TouricoHotelSearchProvider();
                
            }

             //msanka change 
            if (searchProviderType == HotelSearchProviderTypes.Tourico)
            {
                return new TouricoHotelSearchProvider();
                
            }

            throw new ApplicationException("Unexpected search provider type.");
        }

        public static IEnumerable<IHotelDataProvider> GetHotelDataProviders()
        {
            return new List<IHotelDataProvider> { new LeonardoHotelDataProvider() };
        }

        public static IHotelTravelServiceProvider GetHotelTravelServiceProvider(ProviderTypes provider)
        {
            //if (provider == ProviderTypes.Tourico)
            //{
            //    return new TouricoHotelTravelServiceProvider();
            //}

            throw new NotImplementedException(provider.ToString());
        }
    }
}
