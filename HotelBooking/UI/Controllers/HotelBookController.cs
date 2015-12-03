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

        [Authorize]
        public ActionResult Index(FormCollection collection)
        {
            HotelSelectDto select = new HotelSelectDto();
            select.StartDate = collection["startDate"];
            select.EndDate = collection["endDate"];
            select.TotalTravellers = collection["totalTravellers"];
            select.HotelCode = collection["hotelCode"];
            select.propertyRphNumber = collection["propertyRphNumber"].TrimStart('0'); 

            var pnrDetails = new HotelBookingManager().Book(select);

            return View(pnrDetails);
        }

    }
}
