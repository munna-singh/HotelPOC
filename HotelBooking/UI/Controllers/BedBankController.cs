using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Runtime.Caching;
using com.hotelbeds.distribution.hotel_api_sdk;
using com.hotelbeds.distribution.hotel_api_model.auto.messages;
using com.hotelbeds.distribution.hotel_api_sdk.helpers;
using Newtonsoft.Json;
using com.hotelbeds.distribution.hotel_api_model.auto.model;
using Manager;
using Repository;

namespace UI.Controllers
{
    public class BedBankController : Controller
    {
        //
        // GET: /BedBank/

        [Authorize]
        public ActionResult Index()
        {

            return View();
        }

        public ActionResult SearchHotel(FormCollection collection)
        {

            HotelSearchDto hotelDto = TempData["HotelSearchDto"] as HotelSearchDto;


            #region Hotel Search
            HotelApiClient client = new HotelApiClient();
            StatusRS status = client.status();
            List<Hotel> hotels = new List<Hotel>();
            if (status != null && status.error == null)
            {
                List<Tuple<string, string>> param;
                Availability avail = new Availability();
                //avail.checkIn = Convert.ToDateTime(collection["checkIn"]);
                avail.checkIn = Convert.ToDateTime(hotelDto.StartDate);
                //avail.checkOut = Convert.ToDateTime(collection["checkOut"]);
                avail.checkOut = Convert.ToDateTime(hotelDto.EndDate);
                //var address = collection["add"];
                var address = hotelDto.Address;
                ViewBag.checkIn = avail.checkIn;
                ViewBag.checkOut = avail.checkOut;
                //avail.destination = "PMI";
                //avail.zone = 90;
                avail.language = "CAS";
                avail.shiftDays = 2;
                AvailRoom room = new AvailRoom();
                //room.adults = Convert.ToInt32(collection["ddlTotalGuest"]);
                room.adults = Convert.ToInt32(hotelDto.TotalGuest);
                ViewBag.GuestNo = room.adults;
                room.children = 0;

                room.details = new List<RoomDetail>();
                room.adultOf(30);
                avail.rooms.Add(room);
                room = new AvailRoom();
                room.adults = Convert.ToInt32(hotelDto.TotalGuest);
                room.children = 0;
                room.details = new List<RoomDetail>();
                room.adultOf(30);
                avail.rooms.Add(room);
                avail.payed = Availability.Pay.AT_HOTEL;

                AvailabilityRQ ar = new AvailabilityRQ();
                ar.stay = new Stay(Convert.ToDateTime(hotelDto.StartDate), Convert.ToDateTime(hotelDto.EndDate), 0, true);

                ar.occupancies = new List<Occupancy>();
                ar.occupancies.Add(new Occupancy
                {
                    adults = Convert.ToInt32(hotelDto.TotalGuest),
                    rooms = 1,
                    children = 0,
                    paxes = new List<Pax>()
                    {
                        new Pax
                        {
                             age = 35,
                             type = com.hotelbeds.distribution.hotel_api_model.auto.common.SimpleTypes.HotelbedsCustomerType.AD
                        }
                    }
                });

                ar.geolocation = new GeoLocation();
                ar.geolocation.latitude = Convert.ToDouble(hotelDto.Latitude);
                ar.geolocation.longitude = Convert.ToDouble(hotelDto.Longitude);
                ViewBag.latOrg = ar.geolocation.latitude;
                ViewBag.lanOrg = ar.geolocation.longitude;
                ar.geolocation.radius = 100;
                ViewBag.radius = ar.geolocation.radius;

                ar.geolocation.unit = com.hotelbeds.distribution.hotel_api_model.util.UnitMeasure.UnitMeasureType.km;

                AvailabilityRS responseAvail = client.doAvailability(ar);
                if (responseAvail != null && responseAvail.hotels != null && responseAvail.hotels.hotels != null && responseAvail.hotels.hotels.Count > 0)
                {
                    int hotelsAvailable = responseAvail.hotels.hotels.Count;
                    hotels = responseAvail.hotels.hotels;
                    //ViewBag.LatOrg = hotels[0].latitude;
                    //ViewBag.LanOrg = hotels[0].longitude;
                    ViewBag.totalTravellers = Convert.ToInt32(hotelDto.TotalGuest);
                }
            }

            #endregion
            return View(hotels);
        }

    }
}
