using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UI.Controllers
{
    public class HotelRoomsController : Controller
    {
        //
        // GET: /HotelRooms/
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

    }
}
