using Common;
using Common.Sabre.Hotels.RateDetails;
using Manager;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UI.Controllers
{
    public class PriceBreakupController : Controller
    {
        //
        // GET: /PriceBreakup/
        [Authorize]
        public ActionResult Index(FormCollection collection)
        {
            HotelRateDescriptionRS priceBreakup = null;
            HotelSelectDto select = new HotelSelectDto();
            select.StartDate = Request.QueryString["startDate"];
            select.EndDate = Request.QueryString["endDate"];
            select.TotalTravellers = Request.QueryString["totalTravellers"];
            select.HotelCode = Request.QueryString["hotelCode"];
            select.RPHNumber = Request.QueryString["propertyRphNumber"];

            var session = SabreSessionManager.Create();
            select.SessionId = session.SecurityValue.BinarySecurityToken;
            var hotelDesc = new HotelPropertyDescription()
               .HotelDescription(select);

            //Get pricing information
            if (hotelDesc.RoomStay != null && hotelDesc.RoomStay.RoomRates != null)
            {
                HotelPricing pricing = new HotelPricing();
                select.RPHNumber = select.RPHNumber;
                priceBreakup = pricing.GetPricing(select);  
            }

            SessionClose closeSession = new SessionClose();
            closeSession.Close(select.SessionId);

            return View(priceBreakup);
        }

    }
}
