using com.hotelbeds.distribution.hotel_api_model.auto.messages;
using com.hotelbeds.distribution.hotel_api_model.auto.model;
using com.hotelbeds.distribution.hotel_api_sdk;
using com.hotelbeds.distribution.hotel_api_sdk.helpers;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UI.Controllers
{
    public class BedBankBookController : Controller
    {
        //
        // GET: /BedBankBook/

        public ActionResult Index(FormCollection collection)
        {
            Hotel hotelfiltered = new Hotel();
            HotelApiClient client = new HotelApiClient();
            StatusRS status = client.status();
            List<Hotel> hotels = new List<Hotel>();
            if (status != null && status.error == null)
            {
                List<Tuple<string, string>> param;
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
                ViewBag.totalTravellers = room.adults;
                room.children = 0;

                room.details = new List<RoomDetail>();
                room.adultOf(30);
                avail.rooms.Add(room);
                room = new AvailRoom();
                //room.adults = Convert.ToInt32(collection["totalTravellers"]);
                room.children = 0;
                room.details = new List<RoomDetail>();
                room.adultOf(30);
                avail.rooms.Add(room);
                avail.payed = Availability.Pay.AT_HOTEL;

                #region Availability Request
                AvailabilityRQ ar = new AvailabilityRQ();
                ar.stay = new Stay(Convert.ToDateTime(collection["checkIn"]), Convert.ToDateTime(collection["checkOut"]), 0, true);

                ar.occupancies = new List<Occupancy>();
                ar.occupancies.Add(new Occupancy
                {
                    adults = Convert.ToInt32(collection["totalTravellers"]),
                    //rooms = 1,
                    rooms = 1,
                    children = 0,
                    paxes = new List<Pax>()
                    {
                        new Pax
                        {
                             age = 35,
                             type = com.hotelbeds.distribution.hotel_api_model.auto.common.SimpleTypes.HotelbedsCustomerType.AD,
                             name = "Munna",
                             surname = "Singh"
                        }
                    }
                });

                ar.geolocation = new GeoLocation();
                ar.geolocation.latitude = Convert.ToDouble(collection["LatOrg"]);
                ar.geolocation.longitude = Convert.ToDouble(collection["LanOrg"]);
                
                ViewBag.Lat = ar.geolocation.latitude;
                ViewBag.Lan = ar.geolocation.longitude;
                ar.geolocation.radius = 100;
                ViewBag.radius = ar.geolocation.radius;

                ar.geolocation.unit = com.hotelbeds.distribution.hotel_api_model.util.UnitMeasure.UnitMeasureType.km;

                #endregion

                #region Hotels Available
                AvailabilityRS responseAvail = client.doAvailability(ar);
                if (responseAvail != null && responseAvail.hotels != null && responseAvail.hotels.hotels != null && responseAvail.hotels.hotels.Count > 0)
                {
                    int hotelsAvailable = responseAvail.hotels.hotels.Count;
                    hotels = responseAvail.hotels.hotels;
                    int hotelcode = Convert.ToInt32(collection["hotelcode"]);
                    ViewBag.HotelCode = hotelcode;
                    ViewBag.TotalTravellers = Convert.ToInt32(collection["ddlTotalGuest"]);
                    //hotelfiltered = hotels.Where(ahotel => ahotel.code == hotelcode).FirstOrDefault();

                    Hotel firstHotel = responseAvail.hotels.hotels.Where(ahotel => ahotel.code == hotelcode).FirstOrDefault();

                    string rateKeySent = collection["rateKey"];
                    string paymenttype = string.Empty;
                    string rateKey = string.Empty;

                    paymenttype = "AT_WEB";
                    rateKey = rateKeySent;

                    //paymenttype = firstHotel.rooms.SelectMany(y => y.rates)
                    //    .Single(r => r.rateKey == rateKeySent).paymentType.ToString();

                    foreach (var room1 in firstHotel.rooms)
                    {
                        foreach (var bbb in room1.rates)
                        {
                            if (rateKeySent == bbb.rateKey)
                            {
                                paymenttype = bbb.paymentType.ToString();
                            }
                        }
                    }



                    //for (int r = 0; r < firstHotel.rooms.Count && String.IsNullOrEmpty(rateKey); r++)
                    //{
                    //    for (int rk = 0; firstHotel.rooms[r].rates != null && rk < firstHotel.rooms[r].rates.Count && String.IsNullOrEmpty(rateKey); rk++)
                    //    {
                    //        rateKey = firstHotel.rooms[r].rates[rk].rateKey;
                    //        //if (rateKeySent == rateKey)
                    //            paymenttype = firstHotel.rooms[r].rates[rk].paymentType.ToString();
                    //    }
                    //}

                    #region Rate Key Available
                    if (!String.IsNullOrEmpty(rateKey))
                    {

                        ConfirmRoom confirmRoom = new ConfirmRoom();
                        confirmRoom.details = new List<RoomDetail>();
                        confirmRoom.detailed(RoomDetail.GuestType.ADULT, 30, "Munna", "Singh", 1);
                        //confirmRoom.detailed(RoomDetail.GuestType.ADULT, 30, "NombrePasajero2", "ApellidoPasajero2", 1);

                        BookingCheck bookingCheck = new BookingCheck();
                        bookingCheck.addRoom(rateKey, confirmRoom);
                        CheckRateRQ checkRateRQ = bookingCheck.toCheckRateRQ();
                        
                       
                            #region if check rate is available
                            if (checkRateRQ != null)
                            {
                                CheckRateRS responseRate = client.doCheck(checkRateRQ);
                                if (responseRate.error != null)
                                    ViewBag.ErrorGot = responseRate.error.message;
                                if (responseRate != null && responseRate.error == null)
                                {
                                    com.hotelbeds.distribution.hotel_api_sdk.helpers.Booking booking = new com.hotelbeds.distribution.hotel_api_sdk.helpers.Booking();
                                    booking.createHolder("Rosetta", "Pruebas");
                                    booking.clientReference = "SDK Test";
                                    booking.remark = "***SDK***TESTING";
                                    //NOTE: ONLY LIBERATE (PAY AT HOTEL MODEL) USES PAYMENT DATA NODES. FOR OTHER PRICING MODELS THESE NODES MUST NOT BE USED.
                                    booking.cardType = "VI";
                                    booking.cardNumber = "4444333322221111";
                                    booking.expiryDate = "0620";
                                    booking.cardCVC = "620";
                                    booking.email = "pmayol@multinucleo.com";
                                    booking.phoneNumber = "654654654";
                                    booking.cardHolderName = "Munna Kumar Singh";

                                    booking.addRoom(rateKey, confirmRoom);

                                    #region BookingRQ
                                    BookingRQ bookingRQ = booking.toBookingRQ();
                                    BookingRS responseBooking = null;

                                if (bookingRQ == null)
                                {
                                    if (paymenttype.ToString() == "AT_WEB")
                                    {
                                        BookingRQ br = new BookingRQ();
                                        br.rooms = new List<BookingRoom>();
                                        br.rooms.Add(new BookingRoom
                                        {
                                            rateKey = rateKey,
                                            paxes = new List<Pax>
                                            {
                                                Capacity = 1
                                                
                                            }
                                        });
                                        br.remark = "***SDK***TESTING";
                                        br.holder = new Holder();
                                        br.holder.name = "Munna";
                                        br.holder.surname = "Singh";

                                        br.clientReference = "Client reference";

                                        responseBooking = client.confirm(br);
                                    }
                                }
                                if (bookingRQ != null)
                                {
                                    #region AT WEB
                                    if (paymenttype.ToString() == "AT_WEB")
                                    {
                                        BookingRQ br = new BookingRQ();
                                        br.rooms = new List<BookingRoom>();
                                        br.rooms.Add(new BookingRoom
                                        {
                                            rateKey = rateKey,
                                            paxes = new List<Pax>
                                            {
                                                Capacity = 1
                                            }
                                        });
                                        br.remark = "***SDK***TESTING";
                                        br.holder = new Holder();
                                        br.holder.name = "Test";
                                        br.holder.surname = "Surname";

                                        br.clientReference = "Client reference";

                                        responseBooking = client.confirm(br);
                                    }
                                    #endregion
                                    //"Booking Response"
                                    if (responseBooking != null)
                                    {
                                        if (responseBooking.error != null)
                                            ViewBag.ErrorGot = responseBooking.error.message;
                                        else
                                        {
                                            ViewBag.BookingRef = responseBooking.booking.reference;
                                        }
                                        //if (responseBooking.booking.reference == null)
                                            
                                    }
                                        //"Confirmation succedded. Canceling reservation with id "    
                                        #region Confirmation Succeeded
                                        if (responseBooking != null && responseBooking.error == null && responseBooking.booking != null)
                                        {
                                            Console.WriteLine("Confirmation succedded. Canceling reservation with id " + responseBooking.booking.reference);
                                            param = new List<Tuple<string, string>>
                                            {
                                                new Tuple<string, string>("${bookingId}", responseBooking.booking.reference),
                                                //new Tuple<string, string>("${bookingId}", "1-3087550"),
                                                new Tuple<string, string>("${flag}", "C")
                                            };

                                            BookingCancellationRS bookingCancellationRS = client.Cancel(param);

                                            #region Cancel Booking
                                            if (bookingCancellationRS != null)
                                            {
                                                //Console.WriteLine("Id cancelled: " + responseBooking.booking.reference);
                                                ViewBag.BookCancelled = "cancelled" + responseBooking.booking.reference;

                                            }
                                            #endregion
                                        }
                                        #endregion
                                    }
                                    #endregion
                                }
                            }
                            #endregion
                        
                            


                    }
                    #endregion
                }
                #endregion
            }

            return View();
        }

    }
}
