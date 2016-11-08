using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Common.carFlowSvc;
using Route = Common.carFlowSvc.Route;
using UI.Models;
using Common.Tourflowsvc;
using UI.Controllers;
using UI.TourReference;


namespace UI.Controllers
{
    public class TourController : Controller
    {
        private string sessiondestination = string.Empty;



        //
        // GET: /Activity/

        public ActionResult Index()
        {

            string fileLoc = @"C:\Temp\BookingHistory.txt";
            string bookingData = string.Empty;
            if (System.IO.File.Exists(fileLoc))
            {
                using (TextReader tr = new StreamReader(fileLoc))
                {
                    bookingData = tr.ReadToEnd();
                }
            }
            List<TourBookingModel> tourBookingsModel = new List<TourBookingModel>();
            if (!string.IsNullOrEmpty(bookingData))
            {
                int recordsCount = bookingData.Count(x => x == ';');
                var recordsList = bookingData.Split(';');
                for (int i = 0; i < recordsCount; i++)
                {
                    if (!string.IsNullOrEmpty(recordsList[i]))
                    {
                        var record = recordsList[i].Split(',');
                        tourBookingsModel.Add(new TourBookingModel()
                        {
                            ConfirmationNo = record[0],
                            StartDate =
                                $"{Convert.ToDateTime(record[1]):MM/dd/yyyy}",
                            EndDate =
                                $"{Convert.ToDateTime(record[2]):MM/dd/yyyy}",
                            status = record[3]
                        });
                    }
                }
            }
            return View(tourBookingsModel.ToArray());
        }

        [Authorize]
        [HttpPost]
        [MultipleButton(Name = "action", Argument = "GetActivity")]
        public ActionResult GetActivity(FormCollection collection)
        {
            sessiondestination = collection["destinationLocation"].Substring(0, 3);


            string sessionName = "SearchResult" + sessiondestination + collection["StartDate"];
            var category = new Common.Tourflowsvc.Category[] {};
            if (Session[sessionName] == null)
            {
                Common.Tourflowsvc.SearchActivityByAirPortCodeRequest request =
                new Common.Tourflowsvc.SearchActivityByAirPortCodeRequest();
                request.FromDate = Convert.ToDateTime(collection["StartDate"]);
                request.ToDate = Convert.ToDateTime(collection["EndDate"]);

                request.Filters = new Common.Tourflowsvc.DestinationResultsFilters()
                {
                    MinAdults = new Common.Tourflowsvc.MinAdultsFilter {Value = 0},
                    MinChildren = new Common.Tourflowsvc.MinChildrenFilter {Value = 0},
                    MinUnits = new Common.Tourflowsvc.MinUnitsFilter {Value = 0},
                   
                };
                request.destination = sessiondestination;
                request.cityName = collection["City"];
                Common.Tourflowsvc.AuthenticationHeader authentication = new Common.Tourflowsvc.AuthenticationHeader();
                authentication.LoginName = "Tra105";
                authentication.Password = "111111";

                Common.Tourflowsvc.ActivityBookFlowClient client = new Common.Tourflowsvc.ActivityBookFlowClient();
                var response = client.SearchActivityByAirPortCode(authentication, request);


                Session[sessionName] = response.Categories;

            }
            else
            {
                category = (Common.Tourflowsvc.Category[]) Session[sessionName];
            }
            ViewBag.SessionId = sessionName;
            return View(category);

        }
        //[Authorize]
        //[HttpPost]
        //[MultipleButton(Name = "action", Argument = "GetActivityDetails")]
        //public ActionResult GetCarDetails(string ActivityId)
        //{
        //    var prdId = Request.Form["ActivityId"];
        //    var sessionId = Request.Form["sessionId"];

        //    return View()
        //}

    }
}





