using com.hotelbeds.distribution.hotel_api_model.auto.messages;
using com.hotelbeds.distribution.hotel_api_model.auto.model;
using com.hotelbeds.distribution.hotel_api_sdk;
using com.hotelbeds.distribution.hotel_api_sdk.helpers;
using Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Serialization;

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
                #region Commented
                //List<Tuple<string, string>> param;
                //Availability avail = new Availability();
                //avail.checkIn = Convert.ToDateTime(collection["checkIn"]);
                //avail.checkOut = Convert.ToDateTime(collection["checkOut"]);

                //var address = collection["add"];
                //ViewBag.checkIn = avail.checkIn;
                //ViewBag.checkOut = avail.checkOut;
                //avail.language = "CAS";
                //avail.shiftDays = 2;
                //AvailRoom room = new AvailRoom();
                //room.adults = Convert.ToInt32(collection["totalTravellers"]);
                //ViewBag.totalTravellers = room.adults;
                //room.children = 0;

                //room.details = new List<RoomDetail>();
                //room.adultOf(30);
                //avail.rooms.Add(room);
                //room = new AvailRoom();
                //room.children = 0;
                //room.details = new List<RoomDetail>();
                //room.adultOf(30);
                //avail.rooms.Add(room);
                //avail.payed = Availability.Pay.AT_HOTEL;


                #region Availability Request
                //AvailabilityRQ ar = new AvailabilityRQ();
                //ar.stay = new Stay(Convert.ToDateTime(collection["checkIn"]), Convert.ToDateTime(collection["checkOut"]), 0, true);

                //ar.occupancies = new List<Occupancy>();
                //ar.occupancies.Add(new Occupancy
                //{
                //    adults = Convert.ToInt32(collection["totalTravellers"]),
                //    //rooms = 1,
                //    rooms = 1,
                //    children = 0,
                //    paxes = new List<Pax>()
                //    {
                //        new Pax
                //        {
                //             age = 35,
                //             type = com.hotelbeds.distribution.hotel_api_model.auto.common.SimpleTypes.HotelbedsCustomerType.AD,
                //             name = "Munna",
                //             surname = "Singh"
                //        }
                //    }
                //});

                //ar.geolocation = new GeoLocation();
                //ar.geolocation.latitude = Convert.ToDouble(collection["LatOrg"]);
                //ar.geolocation.longitude = Convert.ToDouble(collection["LanOrg"]);

                //ViewBag.Lat = ar.geolocation.latitude;
                //ViewBag.Lan = ar.geolocation.longitude;
                //ar.geolocation.radius = 100;
                //ViewBag.radius = ar.geolocation.radius;

                //ar.geolocation.unit = com.hotelbeds.distribution.hotel_api_model.util.UnitMeasure.UnitMeasureType.km;

                #endregion Availability Request

                #endregion Commented

                #region Hotels Available
                //AvailabilityRS responseAvail = client.doAvailability(ar);
                //if (responseAvail != null && responseAvail.hotels != null && responseAvail.hotels.hotels != null && responseAvail.hotels.hotels.Count > 0)
                //{
                //    int hotelsAvailable = responseAvail.hotels.hotels.Count;
                //    hotels = responseAvail.hotels.hotels;
                //    int hotelcode = Convert.ToInt32(collection["hotelcode"]);
                //    ViewBag.HotelCode = hotelcode;
                //    ViewBag.TotalTravellers = Convert.ToInt32(collection["ddlTotalGuest"]);

                //    Hotel firstHotel = responseAvail.hotels.hotels.Where(ahotel => ahotel.code == hotelcode).FirstOrDefault();

                //    string rateKeySent = collection["rateKey"];
                //    string paymenttype = string.Empty;
                //    string rateKey = string.Empty;

                //    rateKey = rateKeySent;

                //    //paymenttype = firstHotel.rooms.SelectMany(y => y.rates)
                //    //    .Single(r => r.rateKey == rateKeySent).paymentType.ToString();

                //    foreach (var room1 in firstHotel.rooms)
                //    {
                //        foreach (var bbb in room1.rates)
                //        {
                //            if (rateKeySent == bbb.rateKey)
                //            {
                //                paymenttype = bbb.paymentType.ToString();
                //                continue;
                //            }
                //        }
                //    }




                #region Rate Key Available
                //if (!String.IsNullOrEmpty(rateKey))
                //{

                //    ConfirmRoom confirmRoom = new ConfirmRoom();
                //    confirmRoom.details = new List<RoomDetail>();
                //    confirmRoom.detailed(RoomDetail.GuestType.ADULT, 30, "Munna", "Singh", 1);
                //    string rateKeySent1 = collection["rateKey"];

                //    BookingCheck bookingCheck = new BookingCheck();
                //    bookingCheck.addRoom(rateKeySent1, confirmRoom);
                //    CheckRateRQ checkRateRQ = bookingCheck.toCheckRateRQ();

                //    //var vvv = client.doCheck(checkRateRQ

                //    #region if check rate is available
                //    if (checkRateRQ != null)
                //        {
                //            CheckRateRS responseRate = client.doCheck(checkRateRQ);
                //            if (responseRate.error != null)
                //                ViewBag.ErrorGot = responseRate.error.message;
                //            if (responseRate != null && responseRate.error == null)
                //            {
                //            BookingRQ bookingRQ = null;
                //            BookingRS responseBooking = null;
                //            if (responseRate.hotel.rooms[0].rates[0].paymentType.ToString() != "AT_WEB")
                //            {
                //                com.hotelbeds.distribution.hotel_api_sdk.helpers.Booking booking = new com.hotelbeds.distribution.hotel_api_sdk.helpers.Booking();
                //                booking.createHolder("Rosetta", "Pruebas");
                //                booking.clientReference = "SDK Test";
                //                booking.remark = "***SDK***TESTING";
                //                //NOTE: ONLY LIBERATE (PAY AT HOTEL MODEL) USES PAYMENT DATA NODES. FOR OTHER PRICING MODELS THESE NODES MUST NOT BE USED.
                //                booking.cardType = "VI";
                //                booking.cardNumber = "4444333322221111";
                //                booking.expiryDate = "0620";
                //                booking.cardCVC = "620";
                //                booking.email = "pmayol@multinucleo.com";
                //                booking.phoneNumber = "654654654";
                //                booking.cardHolderName = "Munna Kumar Singh";

                //                booking.addRoom(rateKey, confirmRoom);

                //                #region BookingRQ
                //                bookingRQ = booking.toBookingRQ();

                //                responseBooking = client.confirm(bookingRQ);
                //            }

                //            if (bookingRQ == null)
                //            {
                //                if (paymenttype.ToString() == "AT_WEB")
                //                {
                //                    BookingRQ br = new BookingRQ();
                //                    br.rooms = new List<BookingRoom>();
                //                    br.rooms.Add(new BookingRoom
                //                    {
                //                        rateKey = rateKey,
                //                        paxes = new List<Pax>
                //                        {
                //                            Capacity = 1

                //                        }
                //                    });
                //                    br.remark = "***SDK***TESTING";
                //                    br.holder = new Holder();
                //                    br.holder.name = "Munna";
                //                    br.holder.surname = "Singh";

                //                    br.clientReference = "Client reference";

                //                    responseBooking = client.confirm(br);
                //                }
                //            }
                //            if (bookingRQ != null)
                //            {
                //                #region AT WEB
                //                if (paymenttype.ToString() == "AT_WEB")
                //                {
                //                    BookingRQ br = new BookingRQ();
                //                    br.rooms = new List<BookingRoom>();
                //                    br.rooms.Add(new BookingRoom
                //                    {
                //                        rateKey = rateKey,
                //                        paxes = new List<Pax>
                //                        {
                //                            Capacity = 1
                //                        }
                //                    });
                //                    br.remark = "***SDK***TESTING";
                //                    br.holder = new Holder();
                //                    br.holder.name = "Test";
                //                    br.holder.surname = "Surname";

                //                    br.clientReference = "Client reference";

                //                    responseBooking = client.confirm(br);

                //                }
                //                #endregion
                //                //"Booking Response"
                //                if (responseBooking != null)
                //                {
                //                    if (responseBooking.error != null)
                //                        ViewBag.ErrorGot = responseBooking.error.message;
                //                    else
                //                    {
                //                        ViewBag.BookingRef = responseBooking.booking.reference;
                //                    }
                //                    //if (responseBooking.booking.reference == null)

                //                }
                //                    //"Confirmation succedded. Canceling reservation with id "    
                //                    #region Confirmation Succeeded
                //                    if (responseBooking != null && responseBooking.error == null && responseBooking.booking != null)
                //                    {
                //                        Console.WriteLine("Confirmation succedded. Canceling reservation with id " + responseBooking.booking.reference);
                //                        param = new List<Tuple<string, string>>
                //                        {
                //                            new Tuple<string, string>("${bookingId}", responseBooking.booking.reference),
                //                            //new Tuple<string, string>("${bookingId}", "1-3087550"),
                //                            new Tuple<string, string>("${flag}", "C")
                //                        };

                //                        BookingCancellationRS bookingCancellationRS = client.Cancel(param);

                //                        #region Cancel Booking
                //                        if (bookingCancellationRS != null)
                //                        {
                //                            //Console.WriteLine("Id cancelled: " + responseBooking.booking.reference);
                //                            ViewBag.BookCancelled = "cancelled" + responseBooking.booking.reference;

                //                        }
                //                        #endregion
                //                    }
                //                    #endregion
                //                }
                //                #endregion
                //            }
                //        }
                //        #endregion




                //}
                #endregion
                //}
                #endregion

                #region Rate Key Available
                //if (!String.IsNullOrEmpty(rateKey))
                //{
                string paymenttype = "";
                List<Tuple<string, string>> param;
                ConfirmRoom confirmRoom = new ConfirmRoom();
                confirmRoom.details = new List<RoomDetail>();
                confirmRoom.detailed(RoomDetail.GuestType.ADULT, 30, "Munna", "Singh", 1);
                string rateKeySent1 = collection["rateKey"];

                BookingCheck bookingCheck = new BookingCheck();
                bookingCheck.addRoom(rateKeySent1, confirmRoom);
                CheckRateRQ checkRateRQ = bookingCheck.toCheckRateRQ();

                #region if check rate is available
                if (checkRateRQ != null)
                {
                    CheckRateRS responseRate = client.doCheck(checkRateRQ);
                    if (responseRate.error != null)
                        ViewBag.ErrorGot = responseRate.error.message;
                    if (responseRate != null && responseRate.error == null)
                    {
                        BookingRQ bookingRQ = null;
                        BookingRS responseBooking = null;
                        if (responseRate.hotel.rooms[0].rates[0].paymentType.ToString() != "AT_WEB")
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

                            booking.addRoom(rateKeySent1, confirmRoom);

                            #region BookingRQ
                            bookingRQ = booking.toBookingRQ();

                            responseBooking = client.confirm(bookingRQ);
                            var Request = XMLSerializer.Serialize<BookingRQ>(bookingRQ);
                            var Response = XMLSerializer.Serialize<BookingRS>(responseBooking);
                        }

                        if (bookingRQ == null)
                        {
                            if (responseRate.hotel.rooms[0].rates[0].paymentType.ToString() == "AT_WEB")
                            {
                                BookingRQ br = new BookingRQ();
                                br.rooms = new List<BookingRoom>();
                                br.rooms.Add(new BookingRoom
                                {
                                    rateKey = rateKeySent1,
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
                                var Request = XMLSerializer.Serialize<BookingRQ>(br);
                                var Response = XMLSerializer.Serialize<BookingRS>(responseBooking);

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
                                    rateKey = rateKeySent1,
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
                }
                #endregion

                //}
                #endregion
            }

            return View();
        }




    }
}
