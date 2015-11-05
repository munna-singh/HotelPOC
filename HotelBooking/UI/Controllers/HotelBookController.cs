using Manager;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UI.Controllers
{
    public class HotelBookController : Controller
    {
        //
        // GET: /HotelBook/

        public ActionResult Index(FormCollection collection)
        {
            HotelSelectDto select = new HotelSelectDto();
            select.StartDate = collection["startDate"];
            select.EndDate = collection["endDate"];
            select.TotalTravellers = collection["totalTravellers"];
            select.HotelCode = collection["hotelCode"];


            select.StartDate = "11-22";
            select.EndDate = "11-24";
            select.HotelCode = "0008100";
            select.TotalTravellers = "2";
            select.propertyRphNumber = "1";

            new HotelBookingManager().Book(select);

            return View();
        }

    }
}
