using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Sabre.Hotels.EndTransaction;

namespace Manager
{
    public class EndTransaction
    {
        public EndTransactionRS End(string securityToken) 
        {
            EndTransactionRQ etrq = new EndTransactionRQ();
            etrq.EndTransaction = new EndTransactionRQEndTransaction();
            etrq.EndTransaction.Ind = true;

            etrq.Source = new EndTransactionRQSource();
            etrq.Source.ReceivedFrom = "test";

            EndTransactionService ets = new EndTransactionService();
            ets.Security = this.CreateSecurityDto(securityToken);
            ets.MessageHeaderValue = this.CreateMessageHeader();
            return ets.EndTransactionRQ(etrq);
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
