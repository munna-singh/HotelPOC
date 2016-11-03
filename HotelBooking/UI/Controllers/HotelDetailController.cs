using System.Web.Mvc;
using TE.Core.Providers;


namespace UI.Controllers
{
    public class HotelDetailController : Controller
    {
        //
        // GET: /HotelDetail/

       
        public ActionResult GetHotelInfo(FormCollection col)
        {       
             
            var provider = HotelProviderBroker.GetHotelDetailsProvider((HotelSearchProviderTypes)int.Parse("2"));
            var searchResponse = provider.RetrieveHotelInfo(col["hotelCode"]);


            return View("HotelDetail", searchResponse);



        }

    }
}
