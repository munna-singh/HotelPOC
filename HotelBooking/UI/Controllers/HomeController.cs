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
using TE.Core.Providers.Tourico.Hotel.Provider;
using TE.Core.ServiceCatalogues.HotelCatalog.Provider;

using TE.Core.Providers;
using TE.Core.Providers.HotelBeds.Provider;
using TE.Platform.Api.TravelServices.Hotel.Dtos;


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

            searchCriteria.Provider = collection["ddlProvider"];

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
            //if (collection["hotelcodes"] != "")
            searchCriteria.HotelCodes = GetHotels(collection);
            TempData["HotelSearchDto"] = searchCriteria;
            //Call manager class and there make a call to provider based on selected ddl value(Provider)

            ViewBag.StartDate = searchCriteria.StartDate;
            ViewBag.EndDate = searchCriteria.EndDate;
            ViewBag.TotalTravellers = searchCriteria.TotalGuest;
            ViewBag.Lat = searchCriteria.Latitude;
            ViewBag.Lan = searchCriteria.Longitude;

            var provider = HotelProviderBroker.GetHotelSearchProvider((HotelSearchProviderTypes)int.Parse(searchCriteria.Provider));
            var searchResponse = provider.Search(ConvertToProviderReqeust(searchCriteria));
            return View("SearchHotelTourico", searchResponse);

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
            return View("SearchHotel", result);

        }

       
        [Authorize]
        [HttpPost]
        [MultipleButton(Name = "action", Argument = "Tourico")]
        public List<string> GetHotels(FormCollection collection)
        {
            //TempData["col"] = collection;
            // return RedirectToAction("SearchTourico", "BookTourico", collection); //musanka comment

            GeoLocation gloc = new GeoLocation();
            gloc.Latitude = Convert.ToDecimal(collection["lat"]);
            gloc.Longitude = Convert.ToDecimal(collection["lan"]);

            //HotelSearchRequestDto searchCriteria = new HotelSearchRequestDto();

            //// searchCriteria.Address = collection["add"];           
            //searchCriteria.Location = gloc;
            //searchCriteria.CheckInDate = Convert.ToDateTime(collection["checkIn"]);
            //searchCriteria.CheckOutDate = Convert.ToDateTime(collection["checkOut"]);
            //searchCriteria.NumberOfAdults = Convert.ToInt16(collection["ddlTotalGuest"]);
            //searchCriteria.NumberOfRooms = Convert.ToInt16(collection["ddlNoOfRooms"]);

            ////Check in cache
            //var key = gloc.Latitude.ToString() + gloc.Longitude.ToString() + searchCriteria.CheckInDate.ToString() + searchCriteria.CheckInDate.ToString();
            //var result = GetFromCache(key);

            //holds list of hotel ids to search
            var hotelIds = new List<string>();

            //if (result == null)
            //{
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
                //TODO: 
                //hotelIds = hotelCatalogManager.RrieveHotelsByLatLongCatalog(catalogRequest);
            }

            //    var provider = HotelProviderBroker.GetHotelSearchProvider(HotelSearchProviderTypes.Tourico);

            //    var providerRes = provider.Search(ConvertToHotelSearchReq(searchCriteria, hotelIds));

            //    result = providerRes; // ConvertFromHotelSearchResp(providerRes);


            //    AddToCache(result, key);
            //}
            //ViewBag.StartDate = searchCriteria.CheckInDate;
            //ViewBag.EndDate = searchCriteria.CheckOutDate;
            //ViewBag.TotalTravellers = searchCriteria.NumberOfAdults;
            //ViewBag.Lat = gloc.Latitude;
            //ViewBag.Lan = gloc.Longitude;
            //return View("SearchHotelTourico", result);
            return hotelIds;

        }

        //private HotelSearchResponseDto ConvertFromHotelSearchResp(HotelAvailabilityProviderRes response)
        //{
        //    var results = new HotelSearchResponseDto();
        //    results.Hotels = new List<HotelSearchResultItem>();
        //    HotelCatalogManager manager = new HotelCatalogManager();
        //    foreach (var providerHotel in response.Hotels)
        //    {
        //        var staticData = new HotelProperty(); //TODO: fix: //manager.GetHotelPropertyInformation(providerHotel.HotelInfo.HotelCode);
        //        if (staticData != null)
        //        {
        //            providerHotel.HotelInfo.HeroImageUrl = staticData.ImageUrl;
        //            providerHotel.HotelInfo.HomepageUrl = staticData.HomepageUrl;
        //            // providerHotel.HotelInfo.Thumbnails = staticData.Thumbnails;
        //            providerHotel.HotelInfo.Email = staticData.Email;
        //        }
        //        // else //msanka change
        //        //  providerHotel.HotelInfo.HeroImageUrl = LeonardoConfigSettings.Instance().ImageUrl;

        //        results.Hotels.Add(providerHotel);
        //    }
        //    return results;
        //}

        private void AddToCache(object value, string key)
        {
            _cache.Set(key, value, new CacheItemPolicy());
        }

        private object GetFromCache(string key)
        {
            var item = _cache.Get(key);
            return item;
        }

        private HotelAvailabilityProviderReq ConvertToProviderReqeust(HotelSearchDto hotelDto)
        {
            HotelBedsSearchProvider provider = new HotelBedsSearchProvider();
            HotelAvailabilityProviderReq providerReq = new HotelAvailabilityProviderReq();
            if (hotelDto != null)
            {
                providerReq.CheckInDate = Convert.ToDateTime(hotelDto.StartDate);

                providerReq.CheckOutDate = Convert.ToDateTime(hotelDto.EndDate);
                providerReq.MaxRating = hotelDto.MaxRating;
                providerReq.MinRating = hotelDto.MinRating;

                //Hotel code should come from search manager class
                if (hotelDto.HotelCodes != null)
                {
                    providerReq.HotelCodes = hotelDto.HotelCodes;
                }

                else if (hotelDto.Latitude.ToString().Length > 0)
                {
                    providerReq.GeoLocation = new GeoLocation()
                    {

                        Latitude = Convert.ToDecimal(hotelDto.Latitude),
                        Longitude = Convert.ToDecimal(hotelDto.Longitude)
                    };
                }
                //TODO:RoomDto
                //providerReq.TotalAdults = Convert.ToInt16(hotelDto.TotalGuest);

            }
            return providerReq;
        }
    }
    public class HotelProviderBroker
    {
        public static IHotelSearchProvider GetHotelSearchProvider(HotelSearchProviderTypes searchProviderType)
        {
            if (searchProviderType == HotelSearchProviderTypes.Sabre)
            {
                //return new SabreHotelSearchProvider();
            }

            //msanka change 
            if (searchProviderType == HotelSearchProviderTypes.Tourico)
            {
                return new TouricoHotelSearchProvider();
                //throw new NotImplementedException("Tourico provider type not implemented!");
            }


            //msanka change 
            if (searchProviderType == HotelSearchProviderTypes.HotelBeds)
            {
                return new HotelBedsSearchProvider();

            }


            throw new ApplicationException("Unexpected search provider type.");
        }

        public static IHotelDetailsProvider GetHotelDetailsProvider(HotelSearchProviderTypes searchProviderType)
        {
            if (searchProviderType == HotelSearchProviderTypes.Tourico)
            {
                return new TouricoHotelSearchProvider();
            }

            throw new ApplicationException("Unexpected search provider type.");
        }
    }
}
