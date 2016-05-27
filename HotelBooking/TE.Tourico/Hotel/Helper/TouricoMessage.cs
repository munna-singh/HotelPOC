using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TE.Common.Communication;
using TE.Common.Helpers;

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
        /// Sabre has MessageHeader & Security elements in the SOAP header, serialize both.
        /// </summary>
        public override string RequestHeaderPayloadText
        {
            get
            {
                string messageHeaderString = string.Empty;
                var requestMessageHeader = this.CreateMessageHeader();
                if (requestMessageHeader != null)
                {
                    messageHeaderString = XmlSerializerUtil.Serialize(requestMessageHeader, true);
                    XDocument doc = System.Xml.Linq.XDocument.Parse(messageHeaderString.Trim());

                    messageHeaderString = doc.ToString();
                    messageHeaderString = messageHeaderString.Replace("\r\n", "").Replace("q1:", "").Replace(":q1", "");
                }

                var requestSecurity = this.CreateSecurity();
                string securityString = string.Empty;
                if (requestSecurity != null)
                {
                    securityString = XmlSerializerUtil.Serialize(requestSecurity, true);
                    XDocument doc = XDocument.Parse(securityString.Trim());

                    securityString = doc.ToString();
                    securityString = securityString.Replace("\r\n", "").Replace("q1:", "").Replace(":q1", "");
                }

                return string.Format("{0}\n{1}", messageHeaderString, securityString);
            }
        }

        public override string FormattedRequest
        {
            get
            {
                return
                    string.Format("<?xml version=\"1.0\" encoding=\"utf-8\"?>\n"
                                  + "<soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">"
                                  + @"<soap:Header>
                    {0}
                    </soap:Header>
                    <soap:Body>
                            {1}
                    </soap:Body>
            </soap:Envelope>",
                        RequestHeaderPayloadText,
                        RequestBodyPayloadText);
            }
        }

        #endregion


        #region Response Properties

        public MessageHeader ResponseMessageHeader { get; set; }
        public Security ResponseSecurity { get; set; }

        #endregion

        public string ResponseBodyText { get; set; }

        private MessageHeader CreateMessageHeader()
        {
            var header = new MessageHeader();
            header.From = new From();
            header.From.PartyId = new[] { new PartyId { Value = SabreConstants.MessageHeaderFromPartyId } };
            header.To = new To();
            header.To.PartyId = new[] { new PartyId { Value = SabreConstants.MessageHeaderToPartyId } };

            header.CPAId = RequestSession.IPCC;

            header.ConversationId = this.RequestSession.ConversationId;
            header.Service = new Service { Value = String.IsNullOrEmpty(this.Service) ? this.Action : this.Service };
            header.Action = this.Action;

            header.MessageData = new MessageData();
            header.MessageData.MessageId = Guid.NewGuid().ToString();
            header.MessageData.Timestamp = DateTime.UtcNow.ToString(SabreConstants.ZuluTimestampFormat);

            return header;
        }

        private Security CreateSecurity()
        {
            var security = new Security();
            if (!string.IsNullOrEmpty(RequestSession.Token))
            {
                security.BinarySecurityToken = RequestSession.Token;
            }
            else
            {
                var authSession = RequestSession as SabreSessionWithAuth;

                if (authSession == null)
                {
                    throw new ApplicationException("Missing authentication details when trying to open session.");
                }

                security.UsernameToken = new SecurityUsernameToken();
                security.UsernameToken.Domain = authSession.Domain;
                security.UsernameToken.Organization = authSession.IPCC;
                security.UsernameToken.Username = authSession.Username;
                security.UsernameToken.Password = authSession.Password;
            }

            return security;
        }

    }
}
