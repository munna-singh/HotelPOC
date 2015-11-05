using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Sabre.Hotels.CloseSession;

namespace Manager
{
    public class SessionClose
    {
        public void Close(string securityToken)
        {
            SessionCloseRQ scrq = new SessionCloseRQ();
            scrq.POS = new SessionCloseRQPOS();
            scrq.POS.Source = new SessionCloseRQPOSSource();
            scrq.POS.Source.PseudoCityCode = "4REG";


            SessionCloseRQService scrqs = new SessionCloseRQService();
            scrqs.SecurityValue = this.CreateSecurityDto(securityToken);
            scrqs.MessageHeaderValue = this.CreateMessageHeader();
            scrqs.SessionCloseRQ(scrq);
        }

        private Security CreateSecurityDto(string securityToken)
        {

            Security security = new Security();
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
            msgHeader.Action = "EndTransactionLLSRQ";
            Service service = new Service();
            service.Value = "EndTransactionLLSRQ";
            msgHeader.Service = service;

            MessageData msgData = new MessageData();
            msgData.MessageId = "mid:20001209-133003-2333@clientofsabre.com1";
            //msgData.Timestamp = tstamp;
            msgHeader.MessageData = msgData;
            return msgHeader;
        }
    }
}
