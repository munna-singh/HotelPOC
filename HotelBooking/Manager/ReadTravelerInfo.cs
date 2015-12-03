using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Sabre.Hotels.TravelItineraryReadInfo;
using Common;

namespace Manager
{
    public class ReadTravelerInfo
    {
        public TravelItineraryReadRS ReadInfo(string securityToken, string pnrIdentifier = null)
        {
            TravelItineraryReadRQ tirq = new TravelItineraryReadRQ();
            //tirq.Version = "3.6.0";
            tirq.MessagingDetails = new TravelItineraryReadRQMessagingDetails();
            tirq.MessagingDetails.SubjectAreas = new string[] { "FULL" };
            if (pnrIdentifier != null)
            {
                tirq.UniqueID = new TravelItineraryReadRQUniqueID();
                tirq.UniqueID.ID = pnrIdentifier;
            }

            TravelItineraryReadService trs = new TravelItineraryReadService();
            trs.Security = this.CreateSecurityDto(securityToken);
            trs.MessageHeaderValue = this.CreateMessageHeader();
            var t = Utility.Serialize<TravelItineraryReadRQ>(tirq);
            return trs.TravelItineraryReadRQ(tirq);
        }
        private Security1 CreateSecurityDto(string securityToken)
        {

            Security1 security = new Security1();
            security.BinarySecurityToken = securityToken;

            return security;
        }

        private MessageHeader CreateMessageHeader()
        {
            MessageHeader msgHeader = new MessageHeader();
            msgHeader.ConversationId = "TestSession";

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

            msgHeader.CPAId = "4REG";
            msgHeader.Action = "TravelItineraryReadLLSRQ";
            Service service = new Service();
            service.Value = "TravelItineraryReadLLSRQ";
            msgHeader.Service = service;

            MessageData msgData = new MessageData();
            msgData.MessageId = "mid:20001209-133003-2333@clientofsabre.com1";
            //msgData.Timestamp = tstamp;
            msgHeader.MessageData = msgData;
            return msgHeader;
        }
    }
}
