
using Common;
using Manager;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UI.Controllers
{
    public class HotelDetailsController : Controller
    {
        //
        // GET: /HotelDetails/

        public ActionResult Index(FormCollection collection)
        {
            HotelSelectDto select = new HotelSelectDto();
            select.StartDate = collection["startDate"];
            select.EndDate = collection["endDate"];
            select.TotalTravellers = collection["totalTravellers"];
            select.HotelCode = collection["hotelCode"];

            var session = SabreSessionManager.Create();
            select.SessionId = session.SecurityValue.BinarySecurityToken;
            var hotelDesc = new HotelPropertyDescription()
               .HotelDescription(select);

            ViewBag.TotalTravellers = select.TotalTravellers;

            //Get pricing information
            if (hotelDesc.RoomStay != null && hotelDesc.RoomStay.RoomRates != null)
            {
                List<object> priceDetails = new List<object>();
                HotelPricing pricing = new HotelPricing();
                foreach (var room in hotelDesc.RoomStay.RoomRates.RoomRate)
                {
                    select.RPHNumber = room.RPH;
                    priceDetails.Add(pricing.GetPricing(select));
                }

            }
            SessionClose closeSession = new SessionClose();
            closeSession.Close(select.SessionId);
            return View(hotelDesc);
        }

    }
}

