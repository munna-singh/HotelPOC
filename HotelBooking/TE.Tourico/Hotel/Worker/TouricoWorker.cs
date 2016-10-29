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


        public void Dispose()
        {
           // _hotelflowClient = null;
        }  

    }
}