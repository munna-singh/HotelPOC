
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
        [Authorize]
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

            SessionClose closeSession = new SessionClose();
            closeSession.Close(select.SessionId);
            return View(hotelDesc);
        }

    }
}

