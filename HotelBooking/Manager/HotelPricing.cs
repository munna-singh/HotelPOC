
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Sabre.Hotels.RateDetails;
using Common;

namespace Manager
{
    public class HotelPricing
    {
        public HotelRateDescriptionRS GetPricing(HotelSelectDto searchCriteria)
        {    
         
            Security1 sec = new Security1();
            HotelRateDescriptionRQ req = new HotelRateDescriptionRQ();
            HotelRateDescriptionRQAvailRequestSegment availSeg =
                new HotelRateDescriptionRQAvailRequestSegment();
            HotelRateDescriptionRQAvailRequestSegmentGuestCounts guests =
                new HotelRateDescriptionRQAvailRequestSegmentGuestCounts();
            HotelRateDescriptionRQAvailRequestSegmentHotelSearchCriteria searchCrit =
                new HotelRateDescriptionRQAvailRequestSegmentHotelSearchCriteria();
            HotelRateDescriptionRQAvailRequestSegmentHotelSearchCriteriaCriterion criterian =
                new HotelRateDescriptionRQAvailRequestSegmentHotelSearchCriteriaCriterion();
            HotelRateDescriptionRQAvailRequestSegmentHotelSearchCriteriaCriterionHotelRef hotelRef =
                new HotelRateDescriptionRQAvailRequestSegmentHotelSearchCriteriaCriterionHotelRef();
            HotelRateDescriptionRQAvailRequestSegmentTimeSpan journeyDate =
                new HotelRateDescriptionRQAvailRequestSegmentTimeSpan();
            HotelRateDescriptionRQAvailRequestSegmentRatePlanCandidates ratePlans =
                new HotelRateDescriptionRQAvailRequestSegmentRatePlanCandidates();
            HotelRateDescriptionRQAvailRequestSegmentRatePlanCandidatesRatePlanCandidate ratePlan =
                new HotelRateDescriptionRQAvailRequestSegmentRatePlanCandidatesRatePlanCandidate();


            var startDate = Convert.ToDateTime(searchCriteria.StartDate);
            var endDate = Convert.ToDateTime(searchCriteria.EndDate);
            journeyDate.Start = startDate.Month.ToString() + "-" + startDate.Day.ToString();
            journeyDate.End = endDate.Month.ToString() + "-" + endDate.Day.ToString(); ;

            hotelRef.HotelCode = searchCriteria.HotelCode;
            criterian.HotelRef = hotelRef;
            searchCrit.Criterion = criterian;

            guests.Count = searchCriteria.TotalTravellers;

            ratePlan.RateCode = "USD";
            ratePlan.RPH = searchCriteria.RPHNumber.TrimStart('0');
            ratePlans.RatePlanCandidate = ratePlan;

            /*availSeg.HotelSearchCriteria = searchCrit;
            availSeg.GuestCounts = guests;
            availSeg.TimeSpan = journeyDate;              */
            availSeg.RatePlanCandidates = ratePlans;      

            req.AvailRequestSegment = availSeg;

            HotelRateDescriptionService client = new HotelRateDescriptionService();
            client.MessageHeaderValue = Get("HotelRateDescriptionLLSRQ", "HotelRateDescriptionRQ");
            sec.BinarySecurityToken = searchCriteria.SessionId;
            client.Security = sec;
            
            var result = client.HotelRateDescriptionRQ(req);

            var XML = Common.Utility.Serialize(result);
            return result;
        }

        private MessageHeader Get(string actionName, string headerServiceValue)
        {
            //Create the message header and provide the conversation ID.
            MessageHeader msgHeader = new MessageHeader();
            msgHeader.ConversationId = new Guid().ToString();          // Set the ConversationId

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

