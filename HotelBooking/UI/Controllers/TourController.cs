using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Common.carFlowSvc;
using Common.hotelflowSvc;
using Route = Common.carFlowSvc.Route;
using UI.Models;
using Common.Tourflowsvc;
using DotNetOpenAuth.AspNet.Clients;
using UI.Controllers;
using UI.TourReference;
using ActivitiesSelectedOptionsResponse = Common.Tourflowsvc.ActivitiesSelectedOptionsResponse;
using Activity = Common.Tourflowsvc.Activity;
using ActivityId = Common.Tourflowsvc.ActivityId;
using ActivityInfo = UI.TourReference.ActivityInfo;
using ActivityPreBookRequest = Common.Tourflowsvc.ActivityPreBookRequest;
using AuthenticationHeader = Common.Tourflowsvc.AuthenticationHeader;
using BasisType = Common.Tourflowsvc.BasisType;
using BookActivityRequest = Common.Tourflowsvc.BookActivityRequest;
using BookRequest = Common.Tourflowsvc.BookRequest;
using Category = Common.Tourflowsvc.Category;
using GetActivityDetailsRequest = Common.Tourflowsvc.GetActivityDetailsRequest;
using GetRGInfoRequest = Common.Tourflowsvc.GetRGInfoRequest;
using OffsetType = Common.Tourflowsvc.OffsetType;
using OrderInfo = Common.Tourflowsvc.OrderInfo;
using PassengerType = Common.Tourflowsvc.PassengerType;
using PreBookOption = Common.Tourflowsvc.PreBookOption;
using PreBookRequest = Common.Tourflowsvc.PreBookRequest;
using ResultsInfo = Common.Tourflowsvc.ResultsInfo;
using RGInfoRequest = Common.Tourflowsvc.RGInfoRequest;
using WSPaymentType = Common.Tourflowsvc.WSPaymentType;


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
            var category = new Common.Tourflowsvc.Category[] { };
            if (Session[sessionName] == null)
            {
                Common.Tourflowsvc.SearchActivityByAirPortCodeRequest request =
                    new Common.Tourflowsvc.SearchActivityByAirPortCodeRequest();
                request.FromDate = Convert.ToDateTime(collection["StartDate"]);
                request.ToDate = Convert.ToDateTime(collection["EndDate"]);

                request.Filters = new Common.Tourflowsvc.DestinationResultsFilters()
                {
                    MinAdults = new Common.Tourflowsvc.MinAdultsFilter { Value = 0 },
                    MinChildren = new Common.Tourflowsvc.MinChildrenFilter { Value = 0 },
                    MinUnits = new Common.Tourflowsvc.MinUnitsFilter { Value = 0 },

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
                category = (Common.Tourflowsvc.Category[])Session[sessionName];
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
                var activityDetails = (Category[])Session[sessionId];
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

            var response = client.GetActivityDetails(authentication, new[] { activityIdInt });



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
                NumOfAdults =int.Parse(NumOfAdults),
                NumOfChildren = int.Parse(NumOfChildren),
                NumOfUnits = int.Parse(NumOfUnits),
                OptionId = 1985214, //int.Parse(OptionId)
            };
            prebookReq.bookActivityOptions = prebookOptions;
            Common.Tourflowsvc.ActivityBookFlowClient client = new Common.Tourflowsvc.ActivityBookFlowClient();
            var response = client.ActivityPreBook(authentication, prebookReq);
            Session["ActivityPreBook"] = response.ActivitiesSelectedOptions;
            return View(response.ActivitiesSelectedOptions);

        }

        [Authorize]
        [HttpPost]
        [MultipleButton(Name = "action", Argument = "BookActivity")]
        public ActionResult BookActivity()
        {       

            var preBookResult = (ActivitiesSelectedOptionsResponse)Session["ActivityPreBook"];

            //call the web service
            BookActivityRequest request = new BookActivityRequest();
            Common.Tourflowsvc.AuthenticationHeader authentication = new Common.Tourflowsvc.AuthenticationHeader();
            authentication.LoginName = "Tra105";
            authentication.Password = "111111";

            BookRequest bookRequest = new BookRequest();


            OrderInfo orderInfo = new OrderInfo();
            orderInfo.DeltaPrice = new Common.Tourflowsvc.DeltaPrice() { value = 2, basisType = BasisType.Percent };
            orderInfo.rgRefNum = "0";
            orderInfo.requestedPrice = preBookResult.totalPrice;
            orderInfo.currency = preBookResult.currency;
            orderInfo.paymentType = WSPaymentType.Obligo;
            orderInfo.recordLocatorId = 0;
            orderInfo.confirmationEmail = null;
            orderInfo.agentRefNumber = "12345";
            bookRequest.orderInfo = orderInfo;

            var activitySelectedOptions = new Common.Tourflowsvc.ActivitiesSelectedOptions();
            var activityInfoFirst = new Common.Tourflowsvc.ActivityInfo();
            activityInfoFirst.ActivityPricing = new Common.Tourflowsvc.ActivityPrice()
            {
                PriceBreakdown = new Common.Tourflowsvc.ActivityPriceBreakdown()
                {
                    Adult = new Common.Tourflowsvc.AdultPriceBreakdown()
                    {
                        numbers =preBookResult.ActivitiesInfo.First().ActivityPricing.PriceBreakdown.Adult.numbers, 
                        amount = preBookResult.ActivitiesInfo.First().ActivityPricing.PriceBreakdown.Adult.amount,
                    },
                    Child = preBookResult.ActivitiesInfo.First().ActivityPricing.PriceBreakdown.Child,
                    Unit = preBookResult.ActivitiesInfo.First().ActivityPricing.PriceBreakdown.Unit,
                },
                price = preBookResult.ActivitiesInfo.First().ActivityPricing.price,
                currency = preBookResult.ActivitiesInfo.First().ActivityPricing.currency,
                tax = preBookResult.ActivitiesInfo.First().ActivityPricing.tax
            };
            activityInfoFirst.CancellationPolicy = new Common.Tourflowsvc.CancellationPolicy()
            {
                CancellationPenalties = new Common.Tourflowsvc.CancellationPenalty[]
                {
                    new Common.Tourflowsvc.CancellationPenalty()
                    {
                        Deadline = new Common.Tourflowsvc.ClxDeadline()
                        {
                            offsetUnit= preBookResult.ActivitiesInfo.First().CancellationPolicy.CancellationPenalties.First().Deadline.offsetUnit,
                            unitsFromCheckIn =preBookResult.ActivitiesInfo.First().CancellationPolicy.CancellationPenalties.First().Deadline.unitsFromCheckIn,
                        },
                        Penalty = new Common.Tourflowsvc.PricePenalty()
                        {
                            basisType = preBookResult.ActivitiesInfo.First().CancellationPolicy.CancellationPenalties.First().Penalty.basisType,
                            value=preBookResult.ActivitiesInfo.First().CancellationPolicy.CancellationPenalties.First().Penalty.value,
                        }
                    }
                }
            };
            var activityAddition = preBookResult.ActivitiesInfo.First().ActivityAdditions;
            activityInfoFirst.ActivityAdditions = new Common.Tourflowsvc.OrderAddition()
            {
                TextAdditions = new Common.Tourflowsvc.TextAddition[]
                {
                    new Common.Tourflowsvc.TextAddition() {value = activityAddition.TextAdditions[0].value, additionTypeID = activityAddition.TextAdditions[0].additionTypeID, additionType = activityAddition.TextAdditions[0].additionType},
                    new Common.Tourflowsvc.TextAddition() {value = activityAddition.TextAdditions[1].value, additionTypeID = activityAddition.TextAdditions[0].additionTypeID, additionType = activityAddition.TextAdditions[0].additionType},
                    new Common.Tourflowsvc.TextAddition() {value = activityAddition.TextAdditions[2].value, additionTypeID = activityAddition.TextAdditions[0].additionTypeID, additionType = activityAddition.TextAdditions[0].additionType},
                    new Common.Tourflowsvc.TextAddition() {value = activityAddition.TextAdditions[3].value, additionTypeID = activityAddition.TextAdditions[0].additionTypeID, additionType = activityAddition.TextAdditions[0].additionType},
                }
            };
            activityInfoFirst.Passengers = new Common.Tourflowsvc.PassengerInfo[]
            {
                new Common.Tourflowsvc.PassengerInfo() { mobilePhone = "+1 765873839", isMainContact = true, lastName="sh", type=PassengerType.Adult, seqNumber= 1, firstName = "sa"},
                new Common.Tourflowsvc.PassengerInfo() { mobilePhone = "+1 765873830", isMainContact = false, lastName="pi", type=PassengerType.Adult, seqNumber= 2, firstName = "Bi"},
            };

            activityInfoFirst.activityId = preBookResult.ActivitiesInfo.First().activityId; 
            activityInfoFirst.date = preBookResult.ActivitiesInfo.First().date;  
            activityInfoFirst.optionId = preBookResult.ActivitiesInfo.First().optionId;
            Common.Tourflowsvc.ActivityInfo[] activityInfos = new Common.Tourflowsvc.ActivityInfo[] { activityInfoFirst };
            activitySelectedOptions.ActivitiesInfo = activityInfos;
            bookRequest.reservations = activitySelectedOptions;

            Common.Tourflowsvc.ActivityBookFlowClient client = new Common.Tourflowsvc.ActivityBookFlowClient();
            var response = client.BookActivity(authentication, bookRequest);
            Session["BookActivity"] = response.ResGroup;



            return View();

        }

        [Authorize]
        [HttpPost]
       [MultipleButton(Name = "action", Argument = "GetBookingDetail")]
        public ActionResult GetBookingDetail(string rgId)
        {
            var BookResult = (Common.Tourflowsvc.BookResults)Session["BookActivity"];
            var Rgid = Request.Form[" Record locator"];
            Common.Tourflowsvc.ActivityBookFlowClient client = new Common.Tourflowsvc.ActivityBookFlowClient();
            Common.Tourflowsvc.AuthenticationHeader authentication = new Common.Tourflowsvc.AuthenticationHeader();
            authentication.LoginName = "Tra105";
            authentication.Password = "111111";
            var getRGInfoRequest = new GetRGInfoRequest();
            getRGInfoRequest.RGInfoRequestMessage = new Common.Tourflowsvc.RGInfoRequest();
            RGInfoRequest rGInfoRequest = new RGInfoRequest();
            rGInfoRequest.RGId = BookResult.ResGroup.rgId; 
            var response = client.GetRGInfo(authentication, rGInfoRequest);
            Session["GetRGInfo"] = response.ResGroup;
            return View(response.ResGroup);
          

        }

    }
}





