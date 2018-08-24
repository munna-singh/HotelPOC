using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Common.Tourflowsvc;
using Route = Common.carFlowSvc.Route;
using UI.Models;
using Common.Tourflowsvc;
using DotNetOpenAuth.AspNet.Clients;
using Newtonsoft.Json;
using UI.Controllers;


namespace UI.Controllers
{
    public class TourController : Controller
    {
        private string sessiondestination = string.Empty;

        ActivityBookFlowClient client = new ActivityBookFlowClient();

        //
        // GET: /Activity/

        public ActionResult Index()
        {

            string fileLoc = @"C:\Temp\TourBookingHistory.txt";
            string bookingData = string.Empty;
            if (System.IO.File.Exists(fileLoc))
            {
                using (TextReader tr = new StreamReader(fileLoc))
                {
                    bookingData = tr.ReadToEnd();
                }
            }
            List<BookResults> tourBookingsModel = new List<BookResults>();
            if (!string.IsNullOrEmpty(bookingData))
            {
                int recordsCount = bookingData.Count(x => x == ';');
                var recordsList = bookingData.Split(';');
                for (int i = 0; i < recordsCount; i++)
                {
                    if (!string.IsNullOrEmpty(recordsList[i]))
                    {
                        var record = recordsList[i].Split(',');
                        ResGroup resgroup = new ResGroup
                        {
                            rgId = int.Parse(record[0])
                        };
                        tourBookingsModel.Add(new BookResults()
                        {
                            ResGroup = resgroup
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
            //Based on lat and long get the airport code
            string url = string.Format("http://iatageo.com/getCode/{0}/{1}", collection["lati"], collection["long"]);
            var getRequest = WebRequest.Create(url);
            getRequest.ContentType = "application/json; charset=utf-8";
            string text;
            var jsonresponse = (HttpWebResponse)getRequest.GetResponse();

            // ReSharper disable once AssignNullToNotNullAttribute
            using (var sr = new StreamReader(jsonresponse.GetResponseStream()))
            {
                text = sr.ReadToEnd();
            }

            dynamic jsonResp = JsonConvert.DeserializeObject(text);
            
            sessiondestination = jsonResp["IATA"];
            string sessionName = "SearchResult" + sessiondestination + collection["StartDate"];
            var category = new Category[] { };
            SearchActivityByAirPortCodeRequest request = new SearchActivityByAirPortCodeRequest();
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
            var response = client.SearchActivityByAirPortCode(CreateSession(), request);
            Session[sessionName] = response.Categories;
            category = (Category[])Session[sessionName];
            ViewBag.SessionId = sessionName;
            return View(category);

        }
        [Authorize]
        [HttpPost]
        //[MultipleButton(Name = "action", Argument = "GetActivityDetails")]
        public ActionResult GetActivityDetails(FormCollection collection)
        {
            //get the activityId selected by user from the UI
            ActivityId activityIdInt = new ActivityId()
            {
                id = int.Parse(collection["productId"])
            };
            string optionConcatId = collection["optionId"];
            string[] optionIds = optionConcatId.Split(' ');
            ViewBag.optionIds = optionIds;
            string optionType = collection["optionType"];
            ViewBag.optionType = optionType;
            //if session contains the activity details of this activityId
            var sessionId = collection["sessionId"];
            var singleActivity = new Activity();
            if (Session[sessionId] != null)
            {
                var activityDetails = (Category[])Session[sessionId];
                //    singleActivity =
                //       activityDetails.Select(x => x.Activities.Where(y => y.activityId == activityIdInt.id)).First().First();
            }
            //Call the webservice GetActivityDetails 
            ActivityId[] activities = new ActivityId[1];
            activities[0] = activityIdInt;
            var response = client.GetActivityDetails(CreateSession(), activities);
            Session["ActivityDetails"] = response.ActivitiesDetails;
            return View(response.ActivitiesDetails.FirstOrDefault());
        }

        [Authorize]
        [HttpPost]
        //[MultipleButton(Name = "action", Argument = "ActivityPreBook")]
        public ActionResult ActivityPreBook(FormCollection collection)
        {
            var activityId = collection["productId"];
            var Date = collection["pickUpDate"];
            var optionType = collection["optionType"];
            string optionConcatId = collection["optionId"];
            string[] optionIds = optionConcatId.Split(' ');
            ViewBag.optionIds = optionIds;
            ViewBag.optionType = optionType;
            //call the web service
            ActivityPreBookRequest activityPRebookReq = new ActivityPreBookRequest();
            PreBookRequest prebookReq = new PreBookRequest();
            PreBookOption[] prebookOptions = new PreBookOption[optionIds.Length];
            for (var i = 0; i < optionIds.Length; i++)
            {
                if (optionType == "PerPerson")
                {

                    var NumOfAdults = collection["ddlTotalAdults"];
                    var NumOfChildren = collection["ddlTotalChildren"];
                    prebookOptions[i] = new PreBookOption()
                    {
                        ActivityId = int.Parse(activityId),
                        Date = DateTime.Parse(Date),
                        NumOfAdults = int.Parse(NumOfAdults),
                        NumOfChildren = int.Parse(NumOfChildren),
                        NumOfUnits = 0,
                        OptionId = int.Parse(optionIds[i])
                    };
                }
                else if (optionType == "PerUnit")
                {
                    var NumOfUnits = collection["ddlTotalUnits"];
                    prebookOptions[i] = new PreBookOption()
                    {
                        ActivityId = int.Parse(activityId),
                        Date = DateTime.Parse(Date),
                        NumOfAdults = 0,
                        NumOfChildren = 0,
                        NumOfUnits = int.Parse(NumOfUnits),
                        OptionId = int.Parse(optionIds[i])
                    };

                }
            }

            prebookReq.bookActivityOptions = prebookOptions;
            var response = client.ActivityPreBook(CreateSession(), prebookReq);
            Session["preBookResponse"] = response.ActivitiesSelectedOptions;
            return View(response.ActivitiesSelectedOptions);
        }

        [Authorize]
        [HttpPost]
        //[MultipleButton(Name = "action", Argument = "BookActivity")]
        public ActionResult BookActivity()
        {

            var preBookResult = (ActivitiesSelectedOptionsResponse)Session["preBookResponse"];

            //call the web service
            BookActivityRequest request = new BookActivityRequest();
            Common.Tourflowsvc.AuthenticationHeader authentication = new Common.Tourflowsvc.AuthenticationHeader();
            authentication.LoginName = "Tra105";
            authentication.Password = "111111";

            BookRequest bookRequest = new BookRequest();


            OrderInfo orderInfo = new OrderInfo();
            orderInfo.DeltaPrice = new Common.Tourflowsvc.DeltaPrice() { value = 200, basisType = BasisType.Percent };
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
            if (preBookResult.ActivitiesInfo.First().ActivityPricing.PriceBreakdown.Adult != null && (preBookResult.ActivitiesInfo.First().ActivityPricing.PriceBreakdown.Child != null))
            {
                activityInfoFirst.ActivityPricing = new Common.Tourflowsvc.ActivityPrice()
                {
                    PriceBreakdown = new Common.Tourflowsvc.ActivityPriceBreakdown()
                    {
                        Adult = new Common.Tourflowsvc.AdultPriceBreakdown()
                        {
                            numbers = preBookResult.ActivitiesInfo.First().ActivityPricing.PriceBreakdown.Adult.numbers,
                            amount = preBookResult.ActivitiesInfo.First().ActivityPricing.PriceBreakdown.Adult.amount,
                        },
                        Child = new Common.Tourflowsvc.ChildPriceBreakdown()
                        {
                            numbers = preBookResult.ActivitiesInfo.First().ActivityPricing.PriceBreakdown.Child.numbers,
                            amount = preBookResult.ActivitiesInfo.First().ActivityPricing.PriceBreakdown.Child.amount,
                        },
                        Unit = new Common.Tourflowsvc.UnitPriceBreakdown()
                        {
                            numbers = 0,
                            amount = 0,
                        },
                    },
                    price = preBookResult.ActivitiesInfo.First().ActivityPricing.price,
                    currency = preBookResult.ActivitiesInfo.First().ActivityPricing.currency,
                    tax = preBookResult.ActivitiesInfo.First().ActivityPricing.tax
                };
            }
            else if (preBookResult.ActivitiesInfo.First().ActivityPricing.PriceBreakdown.Unit != null)
            {
                activityInfoFirst.ActivityPricing = new Common.Tourflowsvc.ActivityPrice()
                {
                    PriceBreakdown = new Common.Tourflowsvc.ActivityPriceBreakdown()
                    {
                        Adult = new Common.Tourflowsvc.AdultPriceBreakdown()
                        {
                            numbers = 0,
                            amount = 0,
                        },
                        Child = new Common.Tourflowsvc.ChildPriceBreakdown()
                        {
                            numbers = 0,
                            amount = 0,
                        },
                        Unit = new Common.Tourflowsvc.UnitPriceBreakdown()
                        {
                            numbers = preBookResult.ActivitiesInfo.First().ActivityPricing.PriceBreakdown.Unit.numbers,
                            amount = preBookResult.ActivitiesInfo.First().ActivityPricing.PriceBreakdown.Unit.amount,
                        },
                    },
                    price = preBookResult.ActivitiesInfo.First().ActivityPricing.price,
                    currency = preBookResult.ActivitiesInfo.First().ActivityPricing.currency,
                    tax = preBookResult.ActivitiesInfo.First().ActivityPricing.tax
                };
            }

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
            //activityInfoFirst.ActivityAdditions = new Common.Tourflowsvc.OrderAddition();
            OrderAddition orderAddition = new OrderAddition();
            List<TextAddition> textAdditionList = new List<TextAddition>();
            foreach (var item in activityAddition.TextAdditions)
            {
                textAdditionList.Add(new Common.Tourflowsvc.TextAddition() { value = string.IsNullOrEmpty(item.value) ? "ABC" : item.value, additionTypeID = item.additionTypeID, additionType = item.additionType });
            }
            orderAddition.TextAdditions = textAdditionList.ToArray();
            activityInfoFirst.ActivityAdditions = orderAddition;

            activityInfoFirst.Passengers = new Common.Tourflowsvc.PassengerInfo[]
            {
                new Common.Tourflowsvc.PassengerInfo() { mobilePhone = "+1 765873839", isMainContact = true, lastName="sh", type=PassengerType.Adult, seqNumber= 1, firstName = "sa"},
                new Common.Tourflowsvc.PassengerInfo() { mobilePhone = "+1 765873830", isMainContact = false, lastName="pi", type=PassengerType.Child, seqNumber= 2, firstName = "Bi"},
            };

            activityInfoFirst.activityId = preBookResult.ActivitiesInfo.First().activityId;
            activityInfoFirst.date = preBookResult.ActivitiesInfo.First().date;
            activityInfoFirst.optionId = preBookResult.ActivitiesInfo.First().optionId;
            Common.Tourflowsvc.ActivityInfo[] activityInfos = new Common.Tourflowsvc.ActivityInfo[] { activityInfoFirst };
            activitySelectedOptions.ActivitiesInfo = activityInfos;
            bookRequest.reservations = activitySelectedOptions;

            Common.Tourflowsvc.ActivityBookFlowClient client = new Common.Tourflowsvc.ActivityBookFlowClient();
            var response = client.BookActivity(authentication, bookRequest);
            string fileLoc = @"C:\Temp\TourBookingHistory.txt";

            System.IO.File.AppendAllText(fileLoc, (response.ResGroup.rgId + "," + response.ResGroup.Reservations[0].date + "," + "," + response.ResGroup.Reservations[0].status + ";"));
            Session["BookActivity"] = response.ResGroup;
            return View(response);

        }

        [Authorize]
        [HttpPost]
        //[MultipleButton(Name = "action", Argument = "GetBookingDetail")]
        public ActionResult GetBookingDetail(FormCollection collection)
        {
            var Rgid = collection["productId"];
            Common.Tourflowsvc.ActivityBookFlowClient client = new Common.Tourflowsvc.ActivityBookFlowClient();
            Common.Tourflowsvc.AuthenticationHeader authentication = new Common.Tourflowsvc.AuthenticationHeader();
            authentication.LoginName = "Tra105";
            authentication.Password = "111111";
            var getRGInfoRequest = new GetRGInfoRequest();
            getRGInfoRequest.RGInfoRequestMessage = new Common.Tourflowsvc.RGInfoRequest();
            RGInfoRequest rGInfoRequest = new RGInfoRequest();
            rGInfoRequest.RGId = int.Parse(Rgid);
            var response = client.GetRGInfo(authentication, rGInfoRequest);
            Session["GetRGInfo"] = response.ResGroup;
            return View(response);


        }




        private Common.Tourflowsvc.AuthenticationHeader CreateSession()
        {
            Common.Tourflowsvc.AuthenticationHeader authentication = new AuthenticationHeader();
            authentication.LoginName = "Tra105";
            authentication.Password = "111111";
            return authentication;
        }
    }
}




