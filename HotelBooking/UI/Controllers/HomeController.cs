using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Common;
using Common.Sabre.Hotels.Search;
using Manager;
using Repository;
using System.Runtime.Caching;

namespace UI.Controllers
{
    public class HomeController : Controller
    {
        private static MemoryCache _cache = new MemoryCache("ExampleCache");

        [Authorize]
        public ActionResult Index()
        {
            //ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";

            return View();
        }



        [Authorize]
        [HttpPost]
        [MultipleButton(Name = "action", Argument = "bedbank")]
        public ActionResult searchBedbank(FormCollection collection)
        {
            HotelSearchDto searchCriteria = new HotelSearchDto();
            searchCriteria.Address = collection["add"];
            searchCriteria.Latitude = double.Parse(collection["lat"]);
            searchCriteria.Longitude = double.Parse(collection["lan"]);
            searchCriteria.StartDate = collection["checkIn"];
            searchCriteria.EndDate = collection["checkOut"];
            searchCriteria.TotalGuest = collection["ddlTotalGuest"];
            searchCriteria.TotalRoom = collection["ddlNoOfRooms"];

            TempData["HotelSearchDto"] = searchCriteria;
            return RedirectToAction("SearchHotel", "BedBank");
        }

        [Authorize]
        [HttpPost]
        [MultipleButton(Name = "action", Argument = "sebre")]
        public ActionResult SearchHotel(FormCollection collection)
        {
            HotelSearchDto searchCriteria = new HotelSearchDto();
            searchCriteria.Address = collection["add"];
            searchCriteria.Latitude = double.Parse(collection["lat"]);
            searchCriteria.Longitude = double.Parse(collection["lan"]);
            searchCriteria.StartDate = collection["checkIn"];
            searchCriteria.EndDate = collection["checkOut"];
            searchCriteria.TotalGuest = collection["ddlTotalGuest"];
            searchCriteria.TotalRoom = collection["ddlNoOfRooms"];
            //Check in cache
            var key = searchCriteria.Latitude.ToString() + searchCriteria.Longitude.ToString() + searchCriteria.StartDate.ToString() + searchCriteria.EndDate.ToString();
            var result = GetFromCache(key);
            if (result == null)
            {
                SearchHotel mgr = new SearchHotel();
                result = mgr.Search(searchCriteria);
                AddToCache(result, key);
            }
            ViewBag.StartDate = searchCriteria.StartDate;
            ViewBag.EndDate = searchCriteria.EndDate;
            ViewBag.TotalTravellers = searchCriteria.TotalGuest;
            ViewBag.Lat = searchCriteria.Latitude;
            ViewBag.Lan = searchCriteria.Longitude;
            return View(result);

        }

        [Authorize]
        [HttpPost]
        [MultipleButton(Name = "action", Argument = "Tourico")]
        public ActionResult SearchTourico(FormCollection collection)
        {
            TempData["col"] = collection;
            return RedirectToAction("SearchTourico", "BookTourico", collection);
        }

        private void AddToCache(object value, string key)
        {
            _cache.Set(key, value, new CacheItemPolicy());
        }

        private object GetFromCache(string key)
        {
            var item = _cache.Get(key);
            return item;
        }
    }
}
