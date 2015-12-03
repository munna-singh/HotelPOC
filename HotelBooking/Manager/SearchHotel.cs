using Common;
using Common.Sabre.Hotels.Search;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager
{
    public class SearchHotel
    {
        public OTA_HotelAvailRS Search(HotelSearchDto searchCriteria)
        {
            var session = SabreSessionManager.Create();
            OTA_HotelAvailRQ availability = new OTA_HotelAvailRQ();
            OTA_HotelAvailRQAvailRequestSegment req = new OTA_HotelAvailRQAvailRequestSegment();
            OTA_HotelAvailRQAvailRequestSegmentHotelSearchCriteria crt =
                new OTA_HotelAvailRQAvailRequestSegmentHotelSearchCriteria();
            OTA_HotelAvailRQAvailRequestSegmentHotelSearchCriteriaCriterion cirterian =
                new OTA_HotelAvailRQAvailRequestSegmentHotelSearchCriteriaCriterion();
            OTA_HotelAvailRQAvailRequestSegmentHotelSearchCriteriaCriterionHotelRef[] refrs =
                new OTA_HotelAvailRQAvailRequestSegmentHotelSearchCriteriaCriterionHotelRef[1];
            OTA_HotelAvailRQAvailRequestSegmentHotelSearchCriteriaCriterionHotelRef ref1 =
                new OTA_HotelAvailRQAvailRequestSegmentHotelSearchCriteriaCriterionHotelRef();
            OTA_HotelAvailRQAvailRequestSegmentTimeSpan journeyDate =
                new OTA_HotelAvailRQAvailRequestSegmentTimeSpan();
            OTA_HotelAvailRQAvailRequestSegmentGuestCounts guest = new OTA_HotelAvailRQAvailRequestSegmentGuestCounts();
            Security1 sec = new Security1();
            OTA_HotelAvailRQAvailRequestSegmentPOS pos = new OTA_HotelAvailRQAvailRequestSegmentPOS();
            OTA_HotelAvailRQAvailRequestSegmentPOSSource source = new OTA_HotelAvailRQAvailRequestSegmentPOSSource();

            OTA_HotelAvailRQAvailRequestSegmentRatePlanCandidates ratePlan = new OTA_HotelAvailRQAvailRequestSegmentRatePlanCandidates();
            ratePlan.RateRange = new OTA_HotelAvailRQAvailRequestSegmentRatePlanCandidatesRateRange() { Min = "1" };

            if (searchCriteria.Address != null && searchCriteria.Address != string.Empty)
                ref1.HotelCityCode = searchCriteria.Address;

            if (searchCriteria.Latitude != null && searchCriteria.Longitude != null)
            {
                ref1.Latitude =  searchCriteria.Latitude.ToString("N2");
                ref1.Longitude = searchCriteria.Longitude.ToString("N2");
            }
            refrs[0] = ref1;
            cirterian.HotelRef = refrs;
            crt.Criterion = cirterian;
            req.HotelSearchCriteria = crt;
            guest.Count = searchCriteria.TotalGuest;
            req.GuestCounts = guest;
            crt.NumProperties = "30";

            //req.po
            var startDate = Convert.ToDateTime(searchCriteria.StartDate);
            var endDate = Convert.ToDateTime(searchCriteria.EndDate);
            journeyDate.Start = startDate.ToString("MM-dd");// .Month.ToString() + "-" + startDate.Day.ToString();
            journeyDate.End = endDate.ToString("MM-dd"); //.Month.ToString() + "-" + endDate.Day.ToString();
            req.TimeSpan = journeyDate;

            availability.AvailRequestSegment = req;

            OTA_HotelAvailService ss = new OTA_HotelAvailService();
            sec.BinarySecurityToken = session.SecurityValue.BinarySecurityToken;
            ss.Security = sec;
            ss.MessageHeaderValue = Get("OTA_HotelAvailLLSRQ", "");
            var XMLRequest = Common.Utility.Serialize(availability);
            var result = ss.OTA_HotelAvailRQ(availability);
            SessionClose close = new SessionClose();
            close.Close(session.SecurityValue.BinarySecurityToken);
            var XML = Common.Utility.Serialize(result);
            return result;
        }

        public MessageHeader Get(string actionName, string headerServiceValue)
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
