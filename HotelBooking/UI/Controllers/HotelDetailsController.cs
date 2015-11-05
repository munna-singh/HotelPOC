
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

            var t = new HotelPropertyDescription()
               .HotelDescription(select.HotelCode, int.Parse(select.TotalTravellers), select.StartDate, select.EndDate);

            ViewBag.HotelProperty = t;

            //return View(t);

            //Get pricing information
            HotelPricing pricing = new HotelPricing();
            var result = pricing.GetPricing(select);
            return View(t);
        }

    }
}

