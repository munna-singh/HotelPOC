
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
            var t = new HotelPropertyDescription()
               .HotelDescription(select);

            ViewBag.HotelProperty = t;

            ViewBag.TotalTravellers = select.TotalTravellers;

            //Get pricing information
            HotelPricing pricing = new HotelPricing();
            var result = pricing.GetPricing(select);

            SessionClose close = new SessionClose();
            close.Close(select.SessionId);
            return View(t);
        }

    }
}

