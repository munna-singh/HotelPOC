using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using TE.Common.Communication;
using TE.Common.Helpers;
using TE.Common.Logging;
using TE.ThirdPartyProviders.Tourico;
using System.ServiceModel.Description;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.IO;
using TE.ThirdPartyProviders.Tourico.TouricoIHotelFlowSvc;
using TE.Core.Hotel.Tourico.Exceptions;

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
            Logger.Instance.LogFunctionEntry("TouricoMessageHelper", "InitializeRequest");

            state.Url = "http://demo-hotelws.touricoholidays.com/HotelFlow.svc/bas"; //TouricoConfigSettings.Instance.ApiUrl;

            Logger.Instance.LogFunctionExit("TouricoMessageHelper", "InitializeRequest");
        }

        /// <summary>
        /// Sets the action to use for the SOAP header and in the Soap header 
        /// element's MessageHeader.
        /// 
        /// Expects that the request body payload implements 
        /// ISabreRequestObject, see the ThirdPartyProvider code for details 
        /// on adding this to generated code.
        /// </summary>
        /// <param name="message"></param>
        public static void AddSoapAction(SoapMessage message)
        {
            var touricoMessage = message as TouricoMessage;
            if (touricoMessage == null)
            {
                throw new InvalidCastException("Expected TouricoMessage");
            }

           touricoMessage.Action = (touricoMessage.RequestBodyPayload as ITouricoRequestObject).ServiceAction;
        }
        
        /// <summary>
        /// Use XmlSerializerUtils to serialize the request into XML.
        /// </summary>
        /// <param name="message">SoapMessage to serialize.</param>
        public static void SerializeRequest(SoapMessage message)
        {
            if (message.RequestBodyPayload == null)
            {
                message.RequestBodyPayloadText = string.Empty;
                return;
            }

            if (message.RequestBodyPayload is string)
            {
                message.RequestBodyPayloadText = message.RequestBodyPayload.ToString();
                return;
            }

            try
            {
                TypedMessageConverter converter = null;

                try
                {
                    //Serialize Tourico request type using TypedMessageConverter to get exactly the same XML format to POST
                     converter = 
                        TypedMessageConverter.Create
                                    (
                                       message.RequestBodyPayload.GetType(),
                                       null,                                    //Dont specify Action name here - Action would part of POST header
                                       new XmlSerializerFormatAttribute()
                                     );                    
                }
                catch (ArgumentException)
                {
                    Logger.Instance.Warn("TouricoMessageHelper", "SerializeRequest", "Unable to Serialize using TypedMessageConverter");
                }

                //Convert to XML representation
                string payloadString
                             = converter.ToMessage(message.RequestBodyPayload, MessageVersion.Soap11).ToString();
             
                //Parse payload
                var doc = XDocument.Parse(payloadString);
                var result = doc.ToString();
                             
                message.RequestBodyPayloadText = result;
            }
            catch (Exception ex)
            {
                Logger.Instance.Error("TouricoMessageHelper", "SerializeRequest", ex, "Unable to serialize request body.");
                throw;
            }
        }
                
        /// <summary>
        /// Parses the response, parses body elements.
        /// </summary>
        /// <typeparam name="TRes"></typeparam>
        /// <param name="message"></param>
        public static void ParseResponse<TRes>(SoapMessage message)
        {
            Logger.Instance.LogFunctionEntry("TouricoMessageHelper", "ParseResponse");

            var touricoMessage = message as TouricoMessage;
            if (touricoMessage == null)
            {
                throw new InvalidCastException("Expected SabreMessage");
            }

            var doc = new XmlDocument();
            doc.LoadXml(message.RawResponseText);
            var nsMgr = new XmlNamespaceManager(doc.NameTable);
            nsMgr.AddNamespace("soap", "http://schemas.xmlsoap.org/soap/envelope/");

            //Body
            var bodyNode = doc.SelectSingleNode("//soap:Body", nsMgr);
            if (bodyNode != null)
            {

                //Try catch here to know if body is actual response or Fault 
                XmlReader bodyReader = XmlReader.Create(new StringReader(bodyNode.InnerXml));
                Message msg = Message.CreateMessage(MessageVersion.Soap11, null, bodyReader);
                var converter = TypedMessageConverter.Create(typeof(TRes), null, new XmlSerializerFormatAttribute());
                touricoMessage.ResponseBodyPayload = (TRes)converter.FromMessage(msg);

                touricoMessage.ResponseBodyText = bodyNode.InnerXml;
             
                
                if (touricoMessage.ResponseBodyPayload == null)
                {
                    var fault = XmlSerializerUtil<WSFault>.Deserialize(touricoMessage.ResponseBodyText);
                    if (fault != null)
                    {
                        throw new TouricoProviderException(fault);
                    }

                    throw new Exception("ResponseBodyPayload is null."); 
                }

            }
            Logger.Instance.LogFunctionExit("TouricoMessageHelper", "ParseResponse");
        }
        
     

    }
}
