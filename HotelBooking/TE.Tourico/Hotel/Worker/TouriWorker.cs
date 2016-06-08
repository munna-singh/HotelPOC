using System;
using TE.ThirdPartyProviders.Tourico.TouricoIHotelFlowSvc;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TE.Core.Tourico.Hotel
{
    public class TouriWorker : IDisposable
    {

        private HotelFlowClient _hotelflowClient;


        public TouriWorker()
        {

        }

        public TWS_HotelDetailsV3 Execute(HotelID[] hotelCode)
        { 
            //instantiate HotelFlowClient
            _hotelflowClient = new HotelFlowClient();

            //search Hotels
            var sreq = _hotelflowClient.GetHotelDetailsV3(GetHeader(),hotelCode, GetFeature());

            //close instance
            _hotelflowClient.Close();

            return sreq;
        }
        
        public SearchResult Execute(SearchHotelsByIdRequest sReq)
        {
        
            //default features
            var features = new Feature[]{ new Feature { name = "OriginalImageSize" , value = "true" }  };
            
            var sreq = this.Execute(GetHeader(), sReq, GetFeature());
  
            return sreq;
        }

        #region common code 
        private AuthenticationHeader GetHeader()
        {
            //build authorization header - change as CreateAuthHeader()
            var authHeader = new AuthenticationHeader();
            authHeader.LoginName = "Tra105"; authHeader.Password = "111111";
            return authHeader;
        }

        private Feature[] GetFeature()
        {
            //default features
            var features = new Feature[] { new Feature { name = "OriginalImageSize", value = "true" } };
            return features;
        }
        #endregion

        public SearchResult Execute(AuthenticationHeader authHeader, SearchHotelsByIdRequest sReq, Feature[] features)
        {
            //instantiate HotelFlowClient
            _hotelflowClient = new HotelFlowClient();

            //search Hotels
            var sreq = _hotelflowClient.SearchHotelsById(authHeader, sReq, features);

            //close instance
            _hotelflowClient.Close();
                        
            return sreq;
        }

        public void Dispose()
        {
            _hotelflowClient = null;
        }
               

    }
}