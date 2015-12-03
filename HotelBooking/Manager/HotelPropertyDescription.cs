using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Sabre.Hotels.PropertyDetails;
using Common;
using Repository;

namespace Manager
{
    public class HotelPropertyDescription
    {
        public HotelPropertyDescriptionRS HotelDescription(HotelSelectDto search)
        {


            HotelPropertyDescriptionRQ hpdrq = new HotelPropertyDescriptionRQ();

            hpdrq.AvailRequestSegment
                = new HotelPropertyDescriptionRQAvailRequestSegment();

            hpdrq.AvailRequestSegment.GuestCounts
                = new HotelPropertyDescriptionRQAvailRequestSegmentGuestCounts();
            hpdrq.AvailRequestSegment.GuestCounts.Count = search.TotalTravellers;

            hpdrq.AvailRequestSegment.HotelSearchCriteria =
                new HotelPropertyDescriptionRQAvailRequestSegmentHotelSearchCriteria();
            hpdrq.AvailRequestSegment.HotelSearchCriteria.Criterion
                = new HotelPropertyDescriptionRQAvailRequestSegmentHotelSearchCriteriaCriterion();
            hpdrq.AvailRequestSegment.HotelSearchCriteria.Criterion.HotelRef
                = new HotelPropertyDescriptionRQAvailRequestSegmentHotelSearchCriteriaCriterionHotelRef();
            hpdrq.AvailRequestSegment.HotelSearchCriteria.Criterion.HotelRef.HotelCode = search.HotelCode;

            hpdrq.AvailRequestSegment.TimeSpan
                = new HotelPropertyDescriptionRQAvailRequestSegmentTimeSpan();
            hpdrq.AvailRequestSegment.TimeSpan.Start = search.StartDate;
            hpdrq.AvailRequestSegment.TimeSpan.End = search.EndDate;

            Security1 security = new Security1();
            security.BinarySecurityToken = search.SessionId;

            HotelPropertyDescriptionService hpds = new HotelPropertyDescriptionService();

            hpds.MessageHeaderValue = this.CreateMessageHeader();
            hpds.Security = security;

            var result =  hpds.HotelPropertyDescriptionRQ(hpdrq);
            var XML = Common.Utility.Serialize(result);
            return result;
        }

        private Security1 CreateSecurityDto()
        {
            var session = SabreSessionManager.Create();

            Security1 security = new Security1();
            security.BinarySecurityToken = session.SecurityValue.BinarySecurityToken;

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
            msgHeader.Action = "HotelPropertyDescriptionLLSRQ";
            Service service = new Service();
            service.Value = "HotelPropertyDescriptionLLSRQ";
            msgHeader.Service = service;

            MessageData msgData = new MessageData();
            msgData.MessageId = "mid:20001209-133003-2333@clientofsabre.com1";
            //msgData.Timestamp = tstamp;
            msgHeader.MessageData = msgData;
            return msgHeader;
        }
    }
}