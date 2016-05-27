using com.hotelbeds.distribution.hotel_api_model.auto.messages;
using com.hotelbeds.distribution.hotel_api_sdk;
using com.hotelbeds.distribution.hotel_api_sdk.helpers;
using com.hotelbeds.distribution.hotel_api_model;
using HotelBeds.ServiceCatalogues.HotelCatalog.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.hotelbeds.distribution.hotel_api_model.auto.model;
using System.Configuration;
using Common.Logging;
using TE.Common.Logging;
using HotelBeds.Provider.Messaging;
//using DataAccessLayer.Repositories;
//using DataAccessLayer.Models;

namespace HotelBeds
{
    public class HotelBedsWorker : IDisposable
    {

        //protected readonly HotelBedsPipelineManager PipelineManager;

        public HotelBedsSession Session { get; private set; }

        public void Close()
        {
            Logger.Instance.LogFunctionEntry(this.GetType().Name, "Close");

            if (this.Session == null)
            {
                throw new ApplicationException("No open session.");
            }

            this.Session = null;

            Logger.Instance.LogFunctionExit(this.GetType().Name, "Close");
        }

        public void Dispose()
        {
            if (this.Session != null)
            {
                this.Close();
            }
        }

        private void Init()
        {
            Logger.Instance.LogFunctionEntry(this.GetType().Name, "Init");

            if (this.Session != null)
            {
                throw new ApplicationException("Session already exists, init not required.");
            }

            if (this.Session == null)
            {
                throw new ApplicationException("Could not open session.");
            }

            Logger.Instance.LogFunctionExit(this.GetType().Name, "Init");
        }

        public AvailabilityRS GetAvailability<TReq, TRes>(AvailabilityRQ request)
        {
            Logger.Instance.LogFunctionEntry(this.GetType().Name, "GetAvailability");
            List<Hotel> hotels = new List<Hotel>();
            HotelApiClient client = new HotelApiClient();
            StatusRS status = client.status();
            AvailabilityRS responseAvail = null;
            try
            {
                //if (status?.error != null)
                if (status != null && status.error == null)
                {
                    responseAvail = client.doAvailability(request);
                }
            }
            catch (Exception)
            {
                throw;
            }

            Logger.Instance.LogFunctionExit(this.GetType().Name, "GetAvailability");
            return responseAvail;
        }

        public AvailabilityRS GetHotelDetails(AvailabilityRQ request)
        {
            Logger.Instance.LogFunctionEntry(this.GetType().Name, "GetHotelDetails");
            AvailabilityRS responseHotelDetail = new AvailabilityRS();
            List<Hotel> hotels = new List<Hotel>();
            HotelApiClient client = new HotelApiClient();
            StatusRS status = client.status();
            AvailabilityRS responseAvail = null;
            try
            {
                if (status != null && status.error == null)
                {
                    Availability avail = new Availability();
                    avail.checkIn = Convert.ToDateTime(request.stay.checkIn);
                    avail.checkOut = Convert.ToDateTime(request.stay.checkOut);
                    avail.includeHotels = new List<int>();
                    foreach (var item in request.hotels.hotel)
                    {
                        avail.includeHotels.Add(item);
                    }
                    AvailRoom room = new AvailRoom();
                    foreach (var occupant in request.occupancies)
                    {
                        room.adults = Convert.ToInt32(occupant.adults);
                        room.numberOfRooms = Convert.ToInt32(occupant.rooms);
                    }
                    room.details = new List<RoomDetail>();
                    for (int i = 0; i < room.adults; i++)
                        room.adultOf(30);

                    avail.rooms.Add(room);
                    AvailabilityRQ availabilityRQ = avail.toAvailabilityRQ();
                    if (availabilityRQ == null)
                        throw new Exception("Availability RQ can't be null", new ArgumentNullException());
                    responseAvail = client.doAvailability(availabilityRQ);
                    return responseAvail;
                }
            }
            catch (Exception)
            {
                throw;
            }

            Logger.Instance.LogFunctionExit(this.GetType().Name, "GetHotelDetails");
            return responseHotelDetail;
        }
    }
}
