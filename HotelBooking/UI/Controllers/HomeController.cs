using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Manager;
using Repository;
using System.Runtime.Caching;
using TE.Core.ServiceCatalogues.HotelCatalog.Dtos.Controller;
using TE.Core.ServiceCatalogues.HotelCatalog.Dtos;
using TE.Core.ServiceCatalogues.HotelCatalog;
using TE.Core.ServiceCatalogues.HotelCatalog.Provider;
using TE.Tourico.Hotel;
using TE.DataAccessLayer.Models;
using TE.HotelBeds.Provider;

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

            ViewBag.StartDate = searchCriteria.StartDate;
            ViewBag.EndDate = searchCriteria.EndDate;
            ViewBag.TotalTravellers = searchCriteria.TotalGuest;
            ViewBag.LatOrg = searchCriteria.Latitude;
            ViewBag.LanOrg = searchCriteria.Longitude;
            TempData["HotelSearchDto"] = searchCriteria;
              
            searchCriteria.HotelCodes = GetHotels(collection);

            //Call manager class and there make a call to provider based on selected ddl value(Provider)
            var provider = HotelProviderBroker.GetHotelSearchProvider((HotelSearchProviderTypes)int.Parse(searchCriteria.Provider));
            var searchResponse = provider.Search(ConvertToProviderRequest(searchCriteria));
            return View("SearchHotel", searchResponse);

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
        public List<string> GetHotels(FormCollection collection)
        {
            GeoLocation gloc = new GeoLocation();
            gloc.Latitude = Convert.ToDecimal(collection["lat"]);
            gloc.Longitude = Convert.ToDecimal(collection["lan"]);

            HotelSearchRequestDto searchCriteria = new HotelSearchRequestDto();

            //holds list of hotel ids to search
            var hotelIds = new List<string>();

            //Retrieve from catalog the list of hotel ids, that will be passed to the provider, if the coordinates are specified in the request
            var catalogRequest = new HotelCatalogRequest
            {
                GeoLocation = gloc, 
                MinRating = HotelBedsConstants.MinRating, 
                MaxRating = HotelBedsConstants.MaxRating, 
            };

            //HotelSearchManager call to get the provider and search
            //If "Skip Local Cache" is false, then call the catalog manager for local search -- (!request.SkipLocalCache)
            if (true)
            {
                var hotelCatalogManager = new HotelCatalogManager();
                hotelIds = hotelCatalogManager.RetrieveHotelsByLatLongCatalog(catalogRequest);
            }

            return hotelIds;
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
                    providerHotel.HotelInfo.Email = staticData.Email;
                }
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

        private HotelAvailabilityProviderReq ConvertToProviderRequest(HotelSearchDto hotelDto)
        {
            //HotelBedsSearchProvider provider = new HotelBedsSearchProvider();
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

                providerReq.TotalAdults = Convert.ToInt16(hotelDto.TotalGuest);

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
            if (searchProviderType == HotelSearchProviderTypes.Tourico)
            {
                return new TouricoHotelSearchProvider();
            }
            if (searchProviderType == HotelSearchProviderTypes.HotelBeds)
            {
                return new HotelBedsSearchProvider();
            }
            throw new ApplicationException("Unexpected search provider type.");
        }
    }
}
