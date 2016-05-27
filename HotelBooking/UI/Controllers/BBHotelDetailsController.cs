using com.hotelbeds.distribution.hotel_api_model.auto.messages;
using com.hotelbeds.distribution.hotel_api_model.auto.model;
using com.hotelbeds.distribution.hotel_api_sdk;
using com.hotelbeds.distribution.hotel_api_sdk.helpers;
using HotelBeds.Handlers;
using HotelBeds.ServiceCatalogues.HotelCatalog.Provider;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UI.Controllers
{
    public class BBHotelDetailsController : Controller
    {
        //
        // GET: /BBHotelDetails/

        public ActionResult Index(FormCollection collection)
        {
            #region Hotel Search
            //Hotel hotelfiltered = new Hotel();

            HotelApiClient client = new HotelApiClient();
            StatusRS status = client.status();
            List<Hotel> hotels = new List<Hotel>();
            if (status != null && status.error == null)
            {
                //List<Tuple<string, string>> param;
                Availability avail = new Availability();
                avail.checkIn = Convert.ToDateTime(collection["checkIn"]);
                avail.checkOut = Convert.ToDateTime(collection["checkOut"]);
                var address = collection["add"];
                ViewBag.checkIn = avail.checkIn;
                ViewBag.checkOut = avail.checkOut;
                //avail.destination = "PMI";
                //avail.zone = 90;
                avail.language = "CAS";
                avail.shiftDays = 2;
                AvailRoom room = new AvailRoom();
                room.adults = Convert.ToInt32(collection["totalTravellers"]);
                ViewBag.GuestNo = room.adults;
                room.children = 0;

            //    room.details = new List<RoomDetail>();
            //    room.adultOf(30);
            //    avail.rooms.Add(room);
            //    room = new AvailRoom();
            //    room.children = 0;
            //    room.details = new List<RoomDetail>();
            //    room.adultOf(30);
            //    avail.rooms.Add(room);
            //    avail.payed = Availability.Pay.AT_HOTEL;

            //    ViewBag.StartDate = Convert.ToDateTime(collection["checkIn"]);
            //    ViewBag.EndDate = Convert.ToDateTime(collection["checkOut"]);

            //    AvailabilityRQ ar = new AvailabilityRQ();
            //    ar.stay = new Stay(Convert.ToDateTime(collection["checkIn"]), Convert.ToDateTime(collection["checkOut"]), 0, true);

            //    ar.occupancies = new List<Occupancy>();
            //    ar.occupancies.Add(new Occupancy
            //    {
            //        adults = Convert.ToInt32(collection["totalTravellers"]),
            //        rooms = 1,
            //        children = 0,
            //        paxes = new List<Pax>()
            //        {
            //            new Pax
            //            {
            //                 age = 35,
            //                 type = com.hotelbeds.distribution.hotel_api_model.auto.common.SimpleTypes.HotelbedsCustomerType.AD,
            //                 name = "Munna",
            //                 surname = "Singh"
            //            }
            //        }
            //    });

            //    ar.geolocation = new GeoLocation();
            //    ar.geolocation.latitude = Convert.ToDouble(collection["LatOrg"]);
            //    ar.geolocation.longitude = Convert.ToDouble(collection["LanOrg"]);
            //    ViewBag.LatOrg = ar.geolocation.latitude;
            //    ViewBag.LanOrg = ar.geolocation.longitude;
            //    ar.geolocation.radius = 100;
            //    ViewBag.radius = ar.geolocation.radius;

            //    ar.geolocation.unit = com.hotelbeds.distribution.hotel_api_model.util.UnitMeasure.UnitMeasureType.km;

            //    AvailabilityRS responseAvail = client.doAvailability(ar);
            //    if (responseAvail != null && responseAvail.hotels != null && responseAvail.hotels.hotels != null && responseAvail.hotels.hotels.Count > 0)
            //    {
            //        int hotelsAvailable = responseAvail.hotels.hotels.Count;
            //        hotels = responseAvail.hotels.hotels;
            //        int hotelcode = Convert.ToInt32(collection["hotelcode"]);
            //        ViewBag.HotelCode = hotelcode;
            //        ViewBag.TotalTravellers = Convert.ToInt32(collection["totalTravellers"]);
            //        hotelfiltered = hotels.Where(ahotel => ahotel.code == hotelcode).FirstOrDefault();
            //    }
            //}

            #endregion
            HotelBedsDetailsHandler hotelInfo = new HotelBedsDetailsHandler();
            HotelAvailabilityProviderReq providerReq = new HotelAvailabilityProviderReq();
            HotelSearchDto searchCriteria = new HotelSearchDto();
            searchCriteria.HotelCodes = collection["hotelCode"];
            searchCriteria.StartDate = collection["checkIn"];
            searchCriteria.EndDate = collection["checkOut"];
            Hotel hotelfiltered = new Hotel();
            providerReq.CheckInDate = Convert.ToDateTime(collection["checkIn"]);
            providerReq.CheckOutDate = Convert.ToDateTime(collection["checkOut"]);
            providerReq.TotalAdults = Convert.ToInt32(collection["totalTravellers"]);
            providerReq.TotalRooms = Convert.ToInt32(collection["totalRooms"]);
            if (searchCriteria.HotelCodes.Length > 0)
            {
                providerReq.HotelCodes = searchCriteria.HotelCodes.Split(' ').ToList<string>();
            }
            var hotelinfoRS = hotelInfo.Execute(providerReq).Hotels.FirstOrDefault();
            return View(hotelinfoRS);
        }

    }
}
