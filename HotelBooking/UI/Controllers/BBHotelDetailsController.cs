using com.hotelbeds.distribution.hotel_api_model.auto.messages;
using com.hotelbeds.distribution.hotel_api_model.auto.model;
using com.hotelbeds.distribution.hotel_api_sdk;
using com.hotelbeds.distribution.hotel_api_sdk.helpers;
using HotelBeds.Handlers;
using HotelBeds.ServiceCatalogues.HotelCatalog;
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
