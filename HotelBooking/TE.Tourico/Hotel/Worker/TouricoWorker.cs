using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TE.Common.Logging;
using TE.Common.Helpers;
using TE.Common.Communication;
using System.Net;
using TE.Core.Hotel.Messaging;
using TE.ThirdPartyProviders.Tourico;

namespace TE.Core.Tourico.Hotel
{
    public class TouricoWorker : IDisposable
    {
        protected readonly TouricoPipelineManager PipelineManager;

        public TouricoWorker(TouricoPipelineManager pipelineManager = null)
        {
            Logger.Instance.LogFunctionEntry(this.GetType().Name, "TouricoWorker");

            this.PipelineManager = pipelineManager ?? new TouricoPipelineManager();

            Logger.Instance.LogFunctionExit(this.GetType().Name, "TouricoWorker");
        }


        /// <summary>
        /// Execute a SOAP req/resp against the Sabre Web Services.
        /// </summary>
        /// <typeparam name="TReq">Request Object type</typeparam>
        /// <typeparam name="TRes">Expected Response Object type.</typeparam>
        /// <param name="request">Request object.</param>
        /// <returns>Response object.</returns>
        public TRes Execute<TReq, TRes>(TReq request) where TReq : ITouricoRequestObject
        {
            Logger.Instance.LogFunctionEntry(this.GetType().Name, "Execute");        
            
            var res = this.PipelineManager.Execute<TReq, TRes>(request);

            Logger.Instance.LogFunctionExit(this.GetType().Name, "Execute");

            return res;
        }




        //public TWS_HotelDetailsV3 Execute(HotelID[] hotelCode)
        //{ 
        //    //instantiate HotelFlowClient
        //    _hotelflowClient = new HotelFlowClient();

        //    //search Hotels
        //    var sreq = _hotelflowClient.GetHotelDetailsV3(GetHeader(),hotelCode, GetFeature());

        //    //close instance
        //    _hotelflowClient.Close();

        //    return sreq;
        //}

        //public SearchResult Execute(SearchHotelsByIdRequest sReq)
        //{
        //    SearchHotelsByIdRequest1 req1 = new SearchHotelsByIdRequest1();
        //    req1.AuthenticationHeader = GetHeader();
        //    req1.request = sReq;
        //    req1.features = GetFeature();

        //    var reqMessage = XmlSerializerUtil.Serialize(req1, true);

        //    using (var apiCallLogger = new ApiCallLogger("Tourico", "Execute", true, "xml"))
        //    {
        //        apiCallLogger.LogRequest(
        //            () => SoapMessagingHelper.FormatXml(reqMessage));


        //        //default features
        //        var features = new Feature[]{ new Feature { name = "OriginalImageSize" , value = "true" }  };

        //        var sreq = this.Execute(GetHeader(), sReq, GetFeature());


        //        reqMessage = XmlSerializerUtil.Serialize(sreq, true);
        //        apiCallLogger.LogRequest(
        //            () => SoapMessagingHelper.FormatXml(reqMessage));

        //        return sreq;
        //    }
        //}

        //#region common code 
        //private AuthenticationHeader GetHeader()
        //{
        //    //build authorization header - change as CreateAuthHeader()
        //    var authHeader = new AuthenticationHeader();
        //    authHeader.LoginName = "Tra105"; authHeader.Password = "111111";
        //    return authHeader;
        //}

        //private Feature[] GetFeature()
        //{
        //    //default features
        //    var features = new Feature[] { new Feature { name = "OriginalImageSize", value = "true" } };
        //    return features;
        //}
        //#endregion

        //public SearchResult Execute(AuthenticationHeader authHeader, SearchHotelsByIdRequest sReq, Feature[] features)
        //{
        //    //instantiate HotelFlowClient
        //    _hotelflowClient = new HotelFlowClient();

        //    //search Hotels
        //    var sreq = _hotelflowClient.SearchHotelsById(authHeader, sReq, features);

        //    //close instance
        //    _hotelflowClient.Close();

        //    return sreq;
        //}

        public void Dispose()
        {
           // _hotelflowClient = null;
        }  

    }
}