using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Common.carFlowSvc;
using Common.Tourflowsvc;
using Route = Common.carFlowSvc.Route;
using UI.Models;
using Common.Tourflowsvc;
using DotNetOpenAuth.AspNet.Clients;
using UI.Controllers;
using GetRGInfoRequest = Common.Tourflowsvc.GetRGInfoRequest;
using ResultsInfo = Common.Tourflowsvc.ResultsInfo;


namespace UI.Controllers
{
    public class TourController : Controller
    {
        private string sessiondestination = string.Empty;



        //
        // GET: /Activity/

        public ActionResult Index()
        {

            string fileLoc = @"C:\Temp\Tour BookingHistory.txt";
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
                Common.Tourflowsvc.AuthenticationHeader authentication = new AuthenticationHeader();
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

        [Authorize]
        [HttpPost]
        [MultipleButton(Name = "action", Argument = "GetActivityDetails")]
        public ActionResult GetActivityDetails(string paramActivityId)
        {
            //get the activityId selected by user from the UI
            ActivityId activityIdInt = new ActivityId()
            {
                id = int.Parse(paramActivityId)
            };

            //if session contains the activity details of this activityId
            var sessionId = Request.Form["sessionId"];
            var singleActivity = new Activity();
            if (Session[sessionId] != null)
            {
                var activityDetails = (Category[]) Session[sessionId];
                singleActivity =
                    activityDetails.Select(x => x.Activities.Where(y => y.activityId == activityIdInt.id))
                        .First()
                        .First();
            }


            //Call the webservice GetActivityDetails 

            GetActivityDetailsRequest request = new GetActivityDetailsRequest();
            request.ActivitiesIds[0] = activityIdInt;
            Common.Tourflowsvc.AuthenticationHeader authentication = new Common.Tourflowsvc.AuthenticationHeader();
            authentication.LoginName = "Tra105";
            authentication.Password = "111111";


            Common.Tourflowsvc.ActivityBookFlowClient client = new Common.Tourflowsvc.ActivityBookFlowClient();

            var response = client.GetActivityDetails(authentication, new[] {activityIdInt});



            Session["ActivityDetails"] = response.ActivitiesDetails;

            return View(response.ActivitiesDetails.FirstOrDefault());
        }

        [Authorize]
        [HttpPost]
        [MultipleButton(Name = "action", Argument = "ActivityPreBok")]
        public ActionResult ActivityPreBok(string paramActivityId)
        {
            var activityId = Request.Form[" ActivityId"];
            var Date = Request.Form["PreBookDate"];
            var NumOfAdults = Request.Form["NumOfAdults"];
            var NumOfChildren = Request.Form["NumOfChildren"];
            var NumOfUnits = Request.Form["NumOfUnits"];
            var OptionId = Request.Form["OptionId"];

            //call the web service
            ActivityPreBookRequest activityPRebookReq = new ActivityPreBookRequest();
            Common.Tourflowsvc.AuthenticationHeader authentication = new Common.Tourflowsvc.AuthenticationHeader();
            authentication.LoginName = "Tra105";
            authentication.Password = "111111";
            PreBookRequest prebookReq = new PreBookRequest();
            PreBookOption[] prebookOptions = new PreBookOption[1];
            prebookOptions[0] = new PreBookOption()
            {
                ActivityId = int.Parse(activityId),
                Date = DateTime.Parse(Date),
                NumOfAdults = int.Parse(NumOfAdults),
                NumOfChildren = int.Parse(NumOfChildren),
                NumOfUnits = int.Parse(NumOfUnits),
                OptionId = int.Parse(OptionId)
            };
            prebookReq.bookActivityOptions = prebookOptions;
            Common.Tourflowsvc.ActivityBookFlowClient client = new Common.Tourflowsvc.ActivityBookFlowClient();
            var response = client.ActivityPreBook(authentication, prebookReq);
            return View(response.Info);

        }
        //[Authorize]
        //[HttpPost]
        //[MultipleButton(Name = "action", Argument = "BookActivity")]
        //public ActionResult BookActivity(string paramActivityId)
        //{
        //    var activityId = Request.Form[" ActivityId"];
        //    var Date = Request.Form["PreBookDate"];
        //    var NumOfAdults = Request.Form["NumOfAdults"];
        //    var NumOfChildren = Request.Form["NumOfChildren"];
        //    var NumOfUnits = Request.Form["NumOfUnits"];
        //    var OptionId = Request.Form["OptionId"];

        //    //call the web service
        //    ActivityPreBookRequest activityPRebookReq = new ActivityPreBookRequest();
        //    Common.Tourflowsvc.AuthenticationHeader authentication = new Common.Tourflowsvc.AuthenticationHeader();
        //    authentication.LoginName = "Tra105";
        //    authentication.Password = "111111";
        //    PreBookRequest prebookReq = new PreBookRequest();
        //    PreBookOption[] prebookOptions = new PreBookOption[1];
        //    prebookOptions[0] = new PreBookOption()
        //    {
        //        ActivityId = int.Parse(activityId),
        //        Date = DateTime.Parse(Date),
        //        NumOfAdults = int.Parse(NumOfAdults),
        //        NumOfChildren = int.Parse(NumOfChildren),
        //        NumOfUnits = int.Parse(NumOfUnits),
        //        OptionId = int.Parse(OptionId)
        //    };
        //    prebookReq.bookActivityOptions = prebookOptions;
        //    Common.Tourflowsvc.ActivityBookFlowClient client = new Common.Tourflowsvc.ActivityBookFlowClient();
        //    var response = client.ActivityPreBook(authentication, prebookReq);
        //    return View(response.Info);

        //}
        //[Authorize]
        //[HttpPost]
        //[MultipleButton(Name = "action", Argument = "GetBookingDetail")]
        //public ActionResult GetBookingDetail(string rgId)
        //{

        //    Common.Tourflowsvc.ActivityBookFlowClient client = new Common.Tourflowsvc.ActivityBookFlowClient();
        //    var getRGInfoRequest = new Common.Tourflowsvc.GetRGInfoRequest()
        //    {
        //        nRGID = Convert.ToInt32(rgId),
        //        Notifications = new Notifications(),
        //        SendDrivingDirections = true
        //    };
        //    var resultsInfo = new Common.Tourflowsvc.ResultsInfo();
        //    var rgInfoResponse =
        //        t.GetRGInfo(
        //            new LoginHeader { UserName = "Tra105", Password = "111111", Culture = "en-US", Version = "1" },
        //            getRGInfoRequest, out resultsInfo);

        //    return View(rgInfoResponse);

        //}


    }
}




