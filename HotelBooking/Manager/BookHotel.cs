using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Sabre.Hotels.Reservation;
using Common;

namespace Manager
{
    public class BookHotel
    {
        public OTA_HotelResRS Book(string securityToken, string propertyInfo) 
        {
            OTA_HotelResRQ hrq = new OTA_HotelResRQ();
            hrq.Hotel = new OTA_HotelResRQHotel();
            hrq.Hotel.BasicPropertyInfo = new OTA_HotelResRQHotelBasicPropertyInfo();
            hrq.Hotel.BasicPropertyInfo.RPH = propertyInfo;
            hrq.Hotel.Customer = new OTA_HotelResRQHotelCustomer();
            hrq.Hotel.Customer.NameNumber = "1.1";
            hrq.Hotel.Guarantee = new OTA_HotelResRQHotelGuarantee();
            hrq.Hotel.Guarantee.Type = "GDPST";
            hrq.Hotel.Guarantee.CC_Info = new OTA_HotelResRQHotelGuaranteeCC_Info();
            hrq.Hotel.Guarantee.CC_Info.PaymentCard = new OTA_HotelResRQHotelGuaranteeCC_InfoPaymentCard();
            hrq.Hotel.Guarantee.CC_Info.PaymentCard.Code = "AX";
            hrq.Hotel.Guarantee.CC_Info.PaymentCard.Number = "371449635398431";
            hrq.Hotel.Guarantee.CC_Info.PaymentCard.ExpireDate = "2016-12";
            hrq.Hotel.Guarantee.CC_Info.PersonName = new OTA_HotelResRQHotelGuaranteeCC_InfoPersonName();
            hrq.Hotel.Guarantee.CC_Info.PersonName.Surname = "Test";
            hrq.Hotel.RoomType = new OTA_HotelResRQHotelRoomType();
            hrq.Hotel.RoomType.NumberOfUnits = "1";

            OTA_HotelResService hrs = new OTA_HotelResService();
            hrs.Security = this.CreateSecurityDto(securityToken);
            hrs.MessageHeaderValue = this.CreateMessageHeader();
            var t = Utility.Serialize<OTA_HotelResRQ>(hrq);
            return hrs.OTA_HotelResRQ(hrq);
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
            msgHeader.Action = "OTA_HotelResLLSRQ";
            Service service = new Service();
            service.Value = "OTA_HotelResLLSRQ";
            msgHeader.Service = service;

            MessageData msgData = new MessageData();
            msgData.MessageId = "mid:20001209-133003-2333@clientofsabre.com1";
            //msgData.Timestamp = tstamp;
            msgHeader.MessageData = msgData;
            return msgHeader;
        }
    }
}
