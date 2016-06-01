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
using HotelBeds.Handlers;
using HotelBeds.ServiceCatalogues.HotelCatalog;
using HotelBeds.Provider;

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
            HotelBedsSearchProvider provider = new HotelBedsSearchProvider();
            HotelAvailabilityProviderReq providerReq = new HotelAvailabilityProviderReq();
            if (hotelDto != null)
            {
               // List<Tuple<string, string>> param;
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

                //Hotel code should come from search manager class
                if (hotelDto.HotelCodes != null) 
                {
                    providerReq.HotelCodes = hotelDto.HotelCodes.Split(',').ToList<string>();
                }
                
                else if (hotelDto.Latitude.ToString().Length > 0)
                {
                    providerReq.GeoLocation = new HotelBeds.ServiceCatalogues.HotelCatalog.Dtos.GeoLocation()
                    {
                        
                        Latitude = Convert.ToDecimal(hotelDto.Latitude),
                        Longitude = Convert.ToDecimal(hotelDto.Longitude)
                    };
                }
                ViewBag.LatOrg = hotelDto.Latitude;
                ViewBag.LanOrg = hotelDto.Longitude;
                providerReq.TotalAdults = Convert.ToInt16(hotelDto.TotalGuest);
                ViewBag.TotalTravellers = providerReq.TotalAdults;
                providerReq.TotalRooms = Convert.ToInt16(hotelDto.TotalRoom);
                ViewBag.TotalRooms = providerReq.TotalRooms;
                try
                {
                    //Call provider not handler
                    var hotels = provider.Search(providerReq).Hotels;
                    if (hotels != null)
                        return View(hotels);
                    else
                        return null;
                }
                catch (Exception ex)
                {
                    ViewBag.error = ex.Message;
                    return View();
                }
                
               
            }
            else
            {
                return View("Home");
            }
            
        }

    }
}
