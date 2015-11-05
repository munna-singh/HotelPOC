using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Sabre.Hotels.CreateSession;
//using Common.Sabre.Hotels.CloseSession;

namespace Common
{
    public class SabreSessionManager
    {
        public static SessionCreateRQService Create()
        {
            SessionCreateRQService serviceObj = new SessionCreateRQService();
            SessionCreateRS resp = new SessionCreateRS();
            try
            {
                // Set user information, including security credentials and the IPCC.
                string username = "7971";
                string password = "ws011213";
                string ipcc = "4REG";
                string domain = "DEFAULT";



                Security security = new Security();
                SecurityUsernameToken securityUserToken = new SecurityUsernameToken();
                securityUserToken.Username = username;
                securityUserToken.Password = password;
                securityUserToken.Organization = ipcc;
                securityUserToken.Domain = domain;
                security.UsernameToken = securityUserToken;


                SessionCreateRQ req = new SessionCreateRQ();
                SessionCreateRQPOS pos = new SessionCreateRQPOS();
                SessionCreateRQPOSSource source = new SessionCreateRQPOSSource();
                source.PseudoCityCode = ipcc;
                pos.Source = source;
                req.POS = pos;


                serviceObj.MessageHeaderValue = Get("SessionCreateRQ", "SessionCreate");
                serviceObj.SecurityValue = security;


                resp = serviceObj.SessionCreateRQ(req);
            }
            catch (Exception e)
            {
                throw new Exception(e.InnerException.ToString());
            }

            return serviceObj;
        }

        public static MessageHeader Get(string actionName, string headerServiceValue)
        {
            //Create the message header and provide the conversation ID.
            MessageHeader msgHeader = new MessageHeader();
            msgHeader.ConversationId = "TestSession";          // Set the ConversationId

            From from = new From();
            PartyId fromPartyId = new PartyId();
            PartyId[] fromPartyIdArr = new PartyId[1];
            fromPartyId.Value = "WebServiceClient";
            fromPartyIdArr[0] = fromPartyId;
            from.PartyId = fromPartyIdArr;
            msgHeader.From = from;

            To to = new To();
            PartyId toPartyId = new PartyId();
            PartyId[] toPartyIdArr = new PartyId[1];
            toPartyId.Value = "WebServiceSupplier";
            toPartyIdArr[0] = toPartyId;
            to.PartyId = toPartyIdArr;
            msgHeader.To = to;

            //Add the value for eb:CPAId, which is the IPCC. 
            //Add the value for the action code of this Web service, SessionCreateRQ.

            msgHeader.CPAId = "4REG";
            msgHeader.Action = actionName;
            Service service = new Service();
            service.Value = headerServiceValue;
            msgHeader.Service = service;

            MessageData msgData = new MessageData();
            msgData.MessageId = "mid:20001209-133003-2333@clientofsabre.com1";
            //msgData.Timestamp = tstamp;
            msgHeader.MessageData = msgData;
            return msgHeader;
        }
    }
}
