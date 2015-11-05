using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Sabre.Hotels.TravelItineraryAddInfo;

namespace Manager
{
    public class AddTravelerInfo
    {
        public TravelItineraryAddInfoRS AddTraveler(string securityToken) 
        {
            TravelItineraryAddInfoRQ tirq = new TravelItineraryAddInfoRQ();

            tirq.CustomerInfo = new TravelItineraryAddInfoRQCustomerInfo();

            List<TravelItineraryAddInfoRQCustomerInfoPersonName> travelerInfos = 
                new List<TravelItineraryAddInfoRQCustomerInfoPersonName>();

            TravelItineraryAddInfoRQCustomerInfoPersonName travelerInfo 
                = new TravelItineraryAddInfoRQCustomerInfoPersonName();

            travelerInfo.NameNumber = "1.1";
            travelerInfo.GivenName = "RICHARD";
            travelerInfo.Surname = "FEYNMAN";
            travelerInfo.NameReference = "REF1";
            
            
            travelerInfos.Add(travelerInfo);

            tirq.CustomerInfo.PersonName = travelerInfos.ToArray();

            List<TravelItineraryAddInfoRQCustomerInfoEmail> emails =
                new List<TravelItineraryAddInfoRQCustomerInfoEmail>();
            emails.Add(new TravelItineraryAddInfoRQCustomerInfoEmail { 
                Address = "test@test.com",
                NameNumber = "1.1"
            });
            tirq.CustomerInfo.Email = emails.ToArray();

            
            tirq.CustomerInfo.CustomerIdentifier = "1234567890";

            List<TravelItineraryAddInfoRQCustomerInfoContactNumber> contacts 
                = new List<TravelItineraryAddInfoRQCustomerInfoContactNumber>();
            contacts.Add(new TravelItineraryAddInfoRQCustomerInfoContactNumber {
                Phone = "8175551212",
                NameNumber = "1.1",
                LocationCode = "YYZ",
                 PhoneUseType = "H"
            });
            tirq.CustomerInfo.ContactNumbers = contacts.ToArray();

            tirq.AgencyInfo = new TravelItineraryAddInfoRQAgencyInfo();
            tirq.AgencyInfo.Address = new TravelItineraryAddInfoRQAgencyInfoAddress();
            tirq.AgencyInfo.Address.AddressLine = "SABRE TRAVEL";
            tirq.AgencyInfo.Address.StreetNmbr = "3150 SABRE DRIVE";
            tirq.AgencyInfo.Address.CityName = "SOUTHLAKE";
            tirq.AgencyInfo.Address.PostalCode = "76092";
            tirq.AgencyInfo.Address.StateCountyProv = new TravelItineraryAddInfoRQAgencyInfoAddressStateCountyProv();
            tirq.AgencyInfo.Address.StateCountyProv.StateCode = "TX";
            tirq.AgencyInfo.Address.CountryCode = "US";
            
            tirq.AgencyInfo.Ticketing = new TravelItineraryAddInfoRQAgencyInfoTicketing();
            tirq.AgencyInfo.Ticketing.TicketType = "7T-";

            TravelItineraryAddInfoService 
                ti = new TravelItineraryAddInfoService();
            ti.Security = this.CreateSecurityDto(securityToken);
            ti.MessageHeaderValue = this.CreateMessageHeader();

            return ti.TravelItineraryAddInfoRQ(tirq);
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
            msgHeader.Action = "TravelItineraryAddInfoLLSRQ";
            Service service = new Service();
            service.Value = "TravelItineraryAddInfoLLSRQ";
            msgHeader.Service = service;

            MessageData msgData = new MessageData();
            msgData.MessageId = "mid:20001209-133003-2333@clientofsabre.com1";
            //msgData.Timestamp = tstamp;
            msgHeader.MessageData = msgData;
            return msgHeader;
        }
    }
}
