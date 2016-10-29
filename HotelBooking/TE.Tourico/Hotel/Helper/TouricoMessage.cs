using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TE.Common.Communication;
using TE.Common.Helpers;
using TE.ThirdPartyProviders.Tourico.TouricoIHotelFlowSvc;

namespace TE.Core.Hotel.Messaging
{
    public class TouricoMessage : SoapMessage
    {

        #region Common Properties

        #endregion

        #region Request Properties

        /// <summary>
        /// SOAP action, each Tourico API has a ServiceAction? which must be specified.
        /// </summary>
        public string Action { get; set; }

        public string Service { get; set; }

       
        /// <summary>
        /// Returns the "Method" to log.
        /// </summary>
        public override string LogMethod
        {
            get
            {
                return Action;
            }
        }

        /// <summary>
        /// Tourico has AuthenticationHeader in the SOAP header - serialize the same.
        /// </summary>
        public override string RequestHeaderPayloadText
        {
            get
            {
                var requestSecurity = this.CreateSecurity(); //AuthHeader
                string securityString = string.Empty;
                if (requestSecurity != null)
                {
                    securityString = XmlSerializerUtil.Serialize(requestSecurity, true);
                    XDocument doc = XDocument.Parse(securityString.Trim());

                    securityString = doc.ToString();
                    securityString = securityString.Replace("\r\n", "").Replace("q1:", "").Replace(":q1", "");
                }

                return string.Format("{0}", securityString);
            }
        }

        /// <summary>
        /// Tourico specific Formatted request
        /// </summary>
        public override string FormattedRequest
        {
            get
            {
                return  RequestBodyPayloadText;
            }
        }

        #endregion


        #region Response Properties

      
        #endregion

        public string ResponseBodyText { get; set; }
           

        private AuthenticationHeader  CreateSecurity()
        {
            var authHeader = new AuthenticationHeader();

           return authHeader;

        }

    }
}
