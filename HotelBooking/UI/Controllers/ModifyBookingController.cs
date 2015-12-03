using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UI.Controllers
{
    public class ModifyBookingController : Controller
    {
        //
        // GET: /ModifyBooking/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ModifyBooking(FormCollection collection)
        {
            var pnr = collection["hdnPNR"];
            Manager.ModifyBooking modify = new Manager.ModifyBooking();
            modify.Modify(pnr);
            return View();
        }

        public ActionResult CancelBooking(FormCollection collection)
        {
            var pnr = collection["hdnPNR"];
            return View();
        }
    }
}
