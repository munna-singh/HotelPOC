using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.hotelbeds.distribution.hotel_api_sdk;
using com.hotelbeds.distribution.hotel_api_model.auto.messages;
using com.hotelbeds.distribution.hotel_api_sdk.helpers;
using Newtonsoft.Json;
using com.hotelbeds.distribution.hotel_api_model.auto.model;
//using Common.hotelflowSvc;
using Repository;

namespace UI
{
    public class SearchBBHotel
    {
        public List<Hotel> Search(BedBankSearchDto searchCriteria)
        {
            HotelApiClient client = new HotelApiClient();
            StatusRS status = client.status();

            List<Hotel> hotels = new List<Hotel>();

            if (status != null && status.error == null)
            {
                //List<Tuple<string, string>> param;

                Availability avail = new Availability();
                avail.checkIn = searchCriteria.StartDate;
                avail.checkOut = searchCriteria.EndDate;
                if (searchCriteria.Address != null && searchCriteria.Address != string.Empty)
                    avail.destination = searchCriteria.Address;
                avail.zone = 90;
                avail.language = "CAS";
                avail.shiftDays = 2;
                AvailRoom room = new AvailRoom();
                room.adults = Convert.ToInt32(searchCriteria.TotalGuest);
                room.children = 0;
                room.details = new List<RoomDetail>();
                room.adultOf(30);
                room.adultOf(30);
                avail.rooms.Add(room);
                room = new AvailRoom();
                room.adults = 2;
                room.children = 0;
                room.details = new List<RoomDetail>();
                room.adultOf(30);
                room.adultOf(30);
                avail.rooms.Add(room);
                //AT_WEB = 0,
                //AT_HOTEL = 1,
                //INDIFFERENT = 2
                avail.payed = Availability.Pay.AT_WEB;
                AvailabilityRQ availabilityRQ = avail.toAvailabilityRQ();
                if (availabilityRQ != null)
                {
                    AvailabilityRS responseAvail = client.doAvailability(availabilityRQ);

                    if (responseAvail != null && responseAvail.hotels != null && responseAvail.hotels.hotels != null && responseAvail.hotels.hotels.Count > 0)
                    {
                        int hotelsAvailable = responseAvail.hotels.hotels.Count;
                        hotels = responseAvail.hotels.hotels;
                        //var abc = JsonConvert.SerializeObject(responseAvail, Formatting.Indented, new JsonSerializerSettings() { DefaultValueHandling = DefaultValueHandling.Ignore });
                    }
                }
            }
                return hotels;
        }
    }
}
