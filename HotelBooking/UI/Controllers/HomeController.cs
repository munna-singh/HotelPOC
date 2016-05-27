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
using TE.Core.ServiceCatalogues.HotelCatalog.Dtos.Controller;
using TE.Core.ServiceCatalogues.HotelCatalog.Dtos;
using TE.Core.ServiceCatalogues.HotelCatalog;

using TE.Core.ServiceCatalogues.HotelCatalog.Provider;



using TE.Tourico.Hotel;
using TE.DataAccessLayer.Models;


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
            if (collection["lat"] != "")
                searchCriteria.Latitude = double.Parse(collection["lat"]);
            if (collection["lan"] != "")
                searchCriteria.Longitude = double.Parse(collection["lan"]);
            searchCriteria.StartDate = collection["checkIn"];
            searchCriteria.EndDate = collection["checkOut"];
            searchCriteria.TotalGuest = collection["ddlTotalGuest"];
            searchCriteria.TotalRoom = collection["ddlNoOfRooms"];
            searchCriteria.Provider = collection["ddlProvider"];
            if (collection["hotelcodes"] != "")
                searchCriteria.HotelCodes = collection["hotelcodes"];
            TempData["HotelSearchDto"] = searchCriteria;
            //Call manager class and there make a call to provider based on selected ddl value(Provider)
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
            searchCriteria.Provider = collection["ddlProvider"];
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
            return View("SearchHotel",result);

        }

        private HotelAvailabilityProviderReq ConvertToHotelSearchReq(HotelSearchRequestDto request, List<string> hotelCode)
        {
            return new HotelAvailabilityProviderReq
            {
                CheckInDate = request.CheckInDate,
                CheckOutDate = request.CheckOutDate,
                LocationType = TE.Core.ServiceCatalogues.HotelCatalog.Enums.LocationTypes.HotelCode,
                TotalAdults = request.NumberOfAdults,
                GeoLocation = request.Location,
                HotelCodes = hotelCode,
                CurrencyCode = new TCurrency { CurrencyCode = request.Currency },
                MinRating = request.MinRating,
                MaxRating = request.MaxRating
            };
        }

        [Authorize]
        [HttpPost]
        //[MultipleButton(Name = "action", Argument = "Tourico")]
        public ActionResult SearchTourico(FormCollection collection)
        {
            //TempData["col"] = collection;
            // return RedirectToAction("SearchTourico", "BookTourico", collection); //musanka comment

            GeoLocation gloc = new GeoLocation();
            gloc.Latitude = Convert.ToDecimal(collection["lat"]);
            gloc.Longitude = Convert.ToDecimal(collection["lan"]);

            HotelSearchRequestDto searchCriteria = new HotelSearchRequestDto();
          
           // searchCriteria.Address = collection["add"];           
            searchCriteria.Location = gloc;
            searchCriteria.CheckInDate = Convert.ToDateTime(collection["checkIn"]);
            searchCriteria.CheckOutDate = Convert.ToDateTime(collection["checkOut"]);
            searchCriteria.NumberOfAdults = Convert.ToInt16(collection["ddlTotalGuest"]);
            searchCriteria.NumberOfRooms = Convert.ToInt16(collection["ddlNoOfRooms"]);
           
            //Check in cache
            var key = gloc.Latitude.ToString() + gloc.Longitude.ToString() + searchCriteria.CheckInDate.ToString() + searchCriteria.CheckInDate.ToString();
            var result = GetFromCache(key);

            //holds list of hotel ids to search
            var hotelIds = new List<string>();

            if (result == null)
            {
                //check here which provider is selected in UI
                //ProviderType = "Tourico";

               //Retrieve from catalog the list of hotel ids, that will be passed to the provider, if the coordinates are specified in the request
                var catalogRequest = new HotelCatalogRequest
                {
                    GeoLocation = gloc, //request.Location,
                    MinRating = 1, //request.MinRating,
                    MaxRating = 5 //request.MaxRating
                };

                //HotelSearhManager call to get the provider and search

                //If "Skip Local Cache" is false, then call the catalog manager for local search -- (!request.SkipLocalCache)
                if (true)
                {
                    var hotelCatalogManager = new HotelCatalogManager();
                    hotelIds = hotelCatalogManager.RetrieveHotelsByLatLongCatalog(catalogRequest);
                }                

                var provider = HotelProviderBroker.GetHotelSearchProvider(HotelSearchProviderTypes.Tourico);

                var providerRes = provider.Search(ConvertToHotelSearchReq(searchCriteria, hotelIds));

                result = providerRes; // ConvertFromHotelSearchResp(providerRes);

            
              AddToCache(result, key);
            }
            ViewBag.StartDate = searchCriteria.CheckInDate;
            ViewBag.EndDate = searchCriteria.CheckOutDate;
            ViewBag.TotalTravellers = searchCriteria.NumberOfAdults;
            ViewBag.Lat = gloc.Latitude;
            ViewBag.Lan = gloc.Longitude;          
            return View("SearchHotelTourico", result);

        }

        private HotelSearchResponseDto ConvertFromHotelSearchResp(HotelAvailabilityProviderRes response)
        {
            var results = new HotelSearchResponseDto();
            results.Hotels = new List<HotelSearchResultItem>();
            HotelCatalogManager manager = new HotelCatalogManager();
            foreach (var providerHotel in response.Hotels)
            {
                var staticData = manager.GetHotelPropertyInformation(providerHotel.HotelInfo.HotelCode);
                if (staticData != null)
                {
                    providerHotel.HotelInfo.HeroImageUrl = staticData.ImageUrl;
                    providerHotel.HotelInfo.HomepageUrl = staticData.HomepageUrl;
                   // providerHotel.HotelInfo.Thumbnails = staticData.Thumbnails;
                    providerHotel.HotelInfo.Email = staticData.Email;
                }
               // else //msanka change
                  //  providerHotel.HotelInfo.HeroImageUrl = LeonardoConfigSettings.Instance().ImageUrl;

                results.Hotels.Add(providerHotel);
            }
            return results;
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
