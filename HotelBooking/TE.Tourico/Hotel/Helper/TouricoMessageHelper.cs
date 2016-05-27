using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TE.Common.Communication;
using TE.Common.Logging;

namespace TE.Core.Hotel.Messaging
{
    public class TouricoMessageHelper
    {
        /// <summary>
        /// Sets the URL for the SOAP request.
        /// </summary>
        /// <param name="state"></param>
        public static void InitializeRequest(SoapMessage state)
        {
            Logger.Instance.LogFunctionEntry("AmadeusMessageHelper", "InitializeRequest");

            state.Url = ""; //SabreConfigSettings.Instance.ApiUrl;

            Logger.Instance.LogFunctionExit("AmadeusMessageHelper", "InitializeRequest");
        }



    }
}
