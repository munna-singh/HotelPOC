
using Common;
using Manager;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TE.Core.ServiceCatalogues.HotelCatalog.Provider;
using TE.Tourico.Hotel;

namespace UI.Controllers
{
    
    public class HotelDetailsController : Controller
    {
        public string selectedHotelCode = null;

        //
        // GET: /HotelDetails/
        [Authorize]
        public ActionResult Index(string hotelCode)
        {
            selectedHotelCode = hotelCode;
            HotelSearchDto searchCriteria = TempData["HotelSearchDto"] as HotelSearchDto;
            
            //Call manager class and there make a call to provider based on selected ddl value(Provider)
            var provider = HotelProviderBroker.GetHotelSearchProvider((HotelSearchProviderTypes)int.Parse(searchCriteria.Provider));
            var searchResponse = provider.RetrieveHotelRates(ConvertToProviderRequest(searchCriteria));


            return View();
        }

        private HotelPropertyProviderReq ConvertToProviderRequest(HotelSearchDto hotelDto)
        {
            HotelPropertyProviderReq providerReq = new HotelPropertyProviderReq();
            if (hotelDto != null)
            {
                providerReq.CheckInDate = Convert.ToDateTime(hotelDto.StartDate);
                providerReq.CheckOutDate = Convert.ToDateTime(hotelDto.EndDate);
                providerReq.HotelCode = selectedHotelCode;
                providerReq.NoOfGuest = Convert.ToInt16(hotelDto.TotalGuest);

            }
            return providerReq;
        }

        public ActionResult TouricoHotelDetails(FormCollection collection)
        {
            TouricoHotelSearchProvider searchProvider = new TouricoHotelSearchProvider();

            var hotelInfo = searchProvider.RetrieveHotelInfo(collection["hotelCode"]);
            ViewBag.StartDate = collection["startDate"];
            ViewBag.EndDate = collection["endDate"];
            ViewBag.RoomTypes = collection["roomTypes"];
            return View(hotelInfo);
        }

    }
}

