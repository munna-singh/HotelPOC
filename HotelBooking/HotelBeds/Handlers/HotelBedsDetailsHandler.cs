using com.hotelbeds.distribution.hotel_api_model.auto.messages;
using com.hotelbeds.distribution.hotel_api_model.auto.model;
using com.hotelbeds.distribution.hotel_api_sdk.helpers;
using Common.Exceptions;
using TE.HotelBeds.Provider;
//using TE.HotelBeds.ServiceCatalogues.HotelCatalog.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TE.Common.Logging;
using TE.Core.ServiceCatalogues.HotelCatalog.Provider;

namespace TE.HotelBeds.Handlers
{
    public class HotelBedsDetailsHandler
    {

        public HotelRateProviderRes Execute(HotelPropertyProviderReq request)
        {
            Logger.Instance.LogFunctionEntry(this.GetType().Name, "Execute");
            Availability avail = new Availability();
            List<Hotel> hotels = new List<Hotel>();

            if (request.CheckInDate <= DateTime.Today)
            {
                throw new ArgumentOutOfRangeException(nameof(request.CheckInDate));
            }
            if (request.CheckInDate >= request.CheckOutDate)
            {
                throw new ArgumentOutOfRangeException(nameof(request.CheckOutDate));
            }
            if (request.NoOfGuest < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(request.NoOfGuest));
            }

            HotelRateProviderRes hotelSearchResults = null;
            using (var hotelBedsworker = new HotelBedsWorker())
            {
                //convert the request dto to the Hot Beds search api payload
                AvailabilityRQ hotelBedsSearchRq = ConvertToHotelBedsSearchRequest(request);
                try
                {
                    var hotelBedsHotels = hotelBedsworker.GetHotelDetails(hotelBedsSearchRq);

                    //hotelSearchResults = ConvertToProviderResponse(hotelBedsHotels);

                    //if (hotelSearchResults == null)
                    //{
                    //    throw new ProviderUnavailableException(ProviderTypes.HotelBeds.ToString(), $"No response to {nameof(AvailabilityRQ)}.", null);
                    //}
                }
                catch (Exception)
                {
                    throw;
                }
            }
            Logger.Instance.LogFunctionExit(this.GetType().Name, "Execute");
            return hotelSearchResults;
        }

        private AvailabilityRQ ConvertToHotelBedsSearchRequest(HotelPropertyProviderReq request)
        {
            Logger.Instance.LogFunctionEntry(this.GetType().Name, "ConvertToHotelBedsSearchRequest");
            AvailabilityRQ hotelBedsAvailabilityRQ = new AvailabilityRQ();

            if (request.HotelCode != null && request.HotelCode.Any())
            {
                var selectedHotel = request.HotelCode.Split(new char[] { ' ' }).ToList();
                List<int> intHotelList = selectedHotel.ConvertAll(s => Int32.Parse(s));

                hotelBedsAvailabilityRQ.hotels = new HotelsFilter()
                {
                    hotel = intHotelList
                };
            }

            hotelBedsAvailabilityRQ.stay = new Stay(request.CheckInDate, request.CheckOutDate, 2, true);

            hotelBedsAvailabilityRQ.occupancies = new List<Occupancy>();
            hotelBedsAvailabilityRQ.occupancies.Add(new Occupancy
            {
                adults = request.NoOfGuest,
                rooms = HotelBedsConstants.TotalRooms,
                children = 0,
                paxes = new List<Pax>()
                    {
                        new Pax
                        {
                             age = HotelBedsConstants.AdultAge,
                             type = com.hotelbeds.distribution.hotel_api_model.auto.common.SimpleTypes.HotelbedsCustomerType.AD
                        }
                    }
            });
            Logger.Instance.LogFunctionExit(this.GetType().Name, "ConvertToHotelBedsSearchRequest");
            return hotelBedsAvailabilityRQ;
        }

        private HotelRateProviderRes ConvertToProviderResponse(AvailabilityRS request)
        {
            Logger.Instance.LogFunctionEntry(this.GetType().Name, "ConvertToProviderResponse");
            HotelRateProviderRes hotelResultRS = new HotelRateProviderRes();
            //if (request.hotels != null)
            //    hotelResultRS.Hotels = request.hotels.hotels;
            //else
            //    hotelResultRS.Hotels = null;
            //Logger.Instance.LogFunctionExit(this.GetType().Name, "ConvertToProviderResponse");
            return hotelResultRS;
        }

    }
}
