using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Common.carFlowSvc;
using TE.Platform.Api.TravelServices.Hotel.Dtos;
using Route = Common.carFlowSvc.Route;
using UI.Models;


namespace UI.Controllers
{
    public class CarController : Controller
    {
        private string sessionPick = string.Empty;
        private string sessionDrop = string.Empty;
        //
        // GET: /Car/

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
            List<CarBookingsModel> carBookingsModel = new List<CarBookingsModel>();
            if (!string.IsNullOrEmpty(bookingData))
            {
                int recordsCount = bookingData.Count(x => x == ';');
                var recordsList = bookingData.Split(';');
                for (int i = 0; i < recordsCount; i++)
                {
                    if (!string.IsNullOrEmpty(recordsList[i]))
                    {
                        var record = recordsList[i].Split(',');
                        carBookingsModel.Add(new CarBookingsModel()
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
            return View(carBookingsModel.ToArray());
        }

        [Authorize]
        [HttpPost]
        [MultipleButton(Name = "action", Argument = "GetCars")]
        public ActionResult GetCars(FormCollection collection)
        {
            sessionPick = collection["pickUpLocation"].Substring(0, 3);
            sessionDrop = collection["dropLocation"].Substring(0, 3);
            string sessionName = "SearchResult" + sessionPick + sessionDrop + collection["pickUpDate"];
            var searchCarInfo = new SearchCarInfo[] { };
            if (Session[sessionName] == null)
            {
                SearchCarsRequest request = new SearchCarsRequest();
                Route route = new Route();
                route.PickUp = collection["pickUpLocation"].Substring(0, 3);
                route.DropOff = collection["dropLocation"].Substring(0, 3);
                //request.Route.PickUp = "MCO";
                //request.Route.DropOff = "MCO";
                request.Route = route;
                request.PickUpDate = Convert.ToDateTime(collection["pickUpDate"]);
                request.DropOffDate = Convert.ToDateTime(collection["dropDate"]);
                request.PickUpHour = Convert.ToInt32(collection["ddlPickUpHour"]);
                request.DropOffHour = Convert.ToInt32(collection["ddlDropHour"]);
                request.VehicleType = Convert.ToInt32(collection["ddlVehicleType"]);
                request.CarCompany = Convert.ToInt32(collection["ddlCarCompany"]);
                request.TotalPax = Convert.ToInt32(collection["ddlTotalPax"]);


                var resultsInfo = new ResultsInfo();

                CarServiceClient carSvc = new CarServiceClient();
                var result =
                    carSvc.SearchCarsByAirportCode(
                        new LoginHeader { UserName = "Tra105", Password = "111111", Culture = "en-US", Version = "1" },
                        request, out searchCarInfo, out resultsInfo);

                Session[sessionName] = searchCarInfo;

            }
            else
            {
                searchCarInfo = (SearchCarInfo[])Session[sessionName];
            }
            ViewBag.SessionId = sessionName;
            return View(searchCarInfo);

        }

        [Authorize]
        [HttpPost]
        //[MultipleButton(Name = "action", Argument = "Tourico")]
        public ActionResult GetCarDetails(string productId)
        {
            var prdId = Request.Form["productId"];
            var sessionId = Request.Form["sessionId"];

            var searchCarDetailInfo = new SearchCarInfo();
            if (Session[sessionId] != null)
            {
                var cars = (SearchCarInfo[])Session[sessionId];
                searchCarDetailInfo = cars.Select(x => x).First(p => p.productId == productId);
            }

            Session["SessionCarPrograms"] = searchCarDetailInfo;
            var resultInfo = new ResultsInfo();
            CarServiceClient carSvc = new CarServiceClient();
            var companyRules =
                carSvc.GetRulesAndRestrictions(
                    new LoginHeader { UserName = "Tra105", Password = "111111", Culture = "en-US", Version = "1" },
                    searchCarDetailInfo.carCompanyId, out resultInfo);

            CarSearchModel carSearchModel = new CarSearchModel();
            carSearchModel.searchCarInfo = searchCarDetailInfo;
            carSearchModel.companyrules = companyRules;
            return View(carSearchModel);

        }

        [Authorize]
        [HttpPost]
        //[MultipleButton(Name = "action", Argument = "Tourico")]
        public ActionResult BookCar(string programId)
        {
            var prdId = Request.Form["productId"];
            var sessionId = Request.Form["sessionId"];
            var carPrograms = (SearchCarInfo)Session["SessionCarPrograms"];
            var carProgram = new CarProgram();
            if (carPrograms != null)
            {
                carProgram = carPrograms.RouteOptions.First().ProgramList.CarProgram.First(p => p.id == programId);
            }

            var bookCarRequest = new BookCarRequest
            {
                SelectedProgram = programId,
                recordLocatorId = 0,
                DriverInfo = new Driver() { age = 30, firstName = "FName", lastName = "LName" },
                PaymentType = "Obligo",
                RequestedPrice = (decimal)carProgram.price,
                DeltaPrice = (decimal)(carProgram.price * 10 / 100),
                Currency = carProgram.currency
            };

            var resultsInfo = new ResultsInfo();
            CarServiceClient carSvc = new CarServiceClient();
            var bookRespone =
                carSvc.BookCar(new LoginHeader { UserName = "Tra105", Password = "111111", Culture = "en-US", Version = "1" }, bookCarRequest, out resultsInfo);

            string fileLoc = @"C:\Temp\BookingHistory.txt";
            
            System.IO.File.AppendAllText(fileLoc, (bookRespone.rgid + "," + bookRespone.Reservation.pickUpDate + "," + bookRespone.Reservation.toDate + "," + bookRespone.Reservation.status + ";"));
           
            return View(bookRespone);

        }

        [Authorize]
        [HttpPost]
        //[AcceptVerbs(HttpVerbs.Post)]
        [MultipleButton(Name = "action", Argument = "CancelCar")]
        public ActionResult CancelCar(string reservationId)
        {
            var resultsInfo = new ResultsInfo();
            CarServiceClient carSvc = new CarServiceClient();
            var resultInfo = new ResultsInfo();
            var cancelResponse =
                carSvc.CancelCar(new LoginHeader { UserName = "Tra105", Password = "111111", Culture = "en-US", Version = "1" }, Convert.ToInt64(reservationId), out resultInfo);
            CarCancelModel carCancelModel = new CarCancelModel();
            carCancelModel.CancellationStatus = cancelResponse;
            return View(carCancelModel);

        }

        [Authorize]
        [HttpPost]
        [MultipleButton(Name = "action", Argument = "GetBookingDetail")]
        public ActionResult GetBookingDetail(string rgId)
        {

            CarServiceClient carSvc = new CarServiceClient();
            var getRGInfoRequest = new GetRGInfoRequest()
            {
                nRGID = Convert.ToInt32(rgId),
                Notifications = new Notifications(),
                SendDrivingDirections = true
            };
            var resultsInfo = new ResultsInfo();
            var rgInfoResponse =
                carSvc.GetRGInfo(
                    new LoginHeader { UserName = "Tra105", Password = "111111", Culture = "en-US", Version = "1" },
                    getRGInfoRequest, out resultsInfo);

            return View(rgInfoResponse);

        }

    }
}
