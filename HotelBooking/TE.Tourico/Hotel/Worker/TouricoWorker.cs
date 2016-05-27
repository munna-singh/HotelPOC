using System;
using TE.ThirdPartyProviders.Tourico.TouricoHotelSvc;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TE.Common.Logging;
using TE.Common.Helpers;
using TE.Common.Communication;
using System.Net;

namespace TE.Core.Tourico.Hotel
{
    public class TouricoWorker : IDisposable
    {

        private HotelFlowClient _hotelflowClient;


        public TouricoWorker()
        {
            Logger.Instance.LogFunctionEntry(this.GetType().Name, "TouricoWorker");
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
            SearchHotelsByIdRequest1 req1 = new SearchHotelsByIdRequest1();
            req1.AuthenticationHeader = GetHeader();
            req1.request = sReq;
            req1.features = GetFeature();

            var reqMessage = XmlSerializerUtil.Serialize(req1, true);

            using (var apiCallLogger = new ApiCallLogger("Tourico", "Execute", true, "xml"))
            {
                apiCallLogger.LogRequest(
                    () => SoapMessagingHelper.FormatXml(reqMessage));
           
            
                //default features
                var features = new Feature[]{ new Feature { name = "OriginalImageSize" , value = "true" }  };
            
                var sreq = this.Execute(GetHeader(), sReq, GetFeature());


                reqMessage = XmlSerializerUtil.Serialize(sreq, true);
                apiCallLogger.LogRequest(
                    () => SoapMessagingHelper.FormatXml(reqMessage));

                return sreq;
            }
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



        public static HttpWebRequest PrepareRequest()
        {
            var req = (HttpWebRequest)WebRequest.CreateDefault(new Uri("http://demo-hotelws.touricoholidays.com/HotelFlow.svc?wsdl"));
            // Sabre expects text/xml content type instead of application/xml
            req.ContentType = "text/xml; charset=utf-8";
            req.Method = "POST";
            req.Accept = "gzip, deflate";
            // Soap action should have been added by SabreMessageHelper.AddSoapAction filter
            req.Headers.Add("SOAPAction", "SearchHotelsById");
            req.Timeout = 300000;
            req.Proxy = null;

            return req;
        }

        //public static void SendRequest(SoapMessage message)
        //{
        //    var sabreMessage = message as SabreMessage;
        //    if (sabreMessage == null)
        //    {
        //        throw new InvalidCastException("Expected SabreMessage");
        //    }

        //    // With Sabre we will log using the action to identify steps rather than request object names
        //    SoapMessagingHelper.SendRequest(
        //        message,
        //        PrepareRequest(sabreMessage),
        //        "Sabre",
        //        LogProviderNameDirectory);
        //}


    }
}