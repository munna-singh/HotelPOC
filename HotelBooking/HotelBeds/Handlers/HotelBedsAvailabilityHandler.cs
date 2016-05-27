using AutoMapper;
using com.hotelbeds.distribution.hotel_api_model.auto.messages;
using com.hotelbeds.distribution.hotel_api_sdk.helpers;
using com.hotelbeds.distribution.hotel_api_model.auto.model;
using HotelBeds.ServiceCatalogues.HotelCatalog.Provider;
using HotelBeds.ServiceCatalogues.HotelCatalog.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HotelBeds.Dtos;
using TE.Common.Logging;
using Common.Exceptions;
using HotelBeds.ServiceCatalogues.HotelCatalog.Enums;
using HotelBeds.Provider.Exceptions;
using HotelBeds.Models;
using HotelBeds.Provider;

namespace HotelBeds.Handlers
{
    public class HotelBedsAvailabilityHandler
    {
        public HotelAvailabilityProviderRes Execute(HotelAvailabilityProviderReq request)
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
            if (request.TotalAdults < HotelBedsConstants.TotalAdults)
            {
                throw new ArgumentOutOfRangeException(nameof(request.TotalAdults));
            }
            //Check if #of nights are more than 30, if so throw exception           
            var duration = (request.CheckOutDate - request.CheckInDate).TotalDays;

            if (duration > HotelBedsConstants.NightsDuration)
            {
                throw new ArgumentOutOfRangeException(nameof(duration));
            }

            HotelAvailabilityProviderRes hotelSearchResults;
            using (var hotelBedsworker = new HotelBedsWorker())
            {
                //convert the request dto to the HotelBeds search api payload
                AvailabilityRQ hotelBedsSearchRq = ConvertToHotelBedsSearchRequest(request);

                try
                {
                    var hotelBedsHotels = hotelBedsworker.GetAvailability<AvailabilityRQ, AvailabilityRS>(hotelBedsSearchRq);
                    //Check if we need to check .hotels == null
                    if (hotelBedsHotels.hotels == null)
                    {
                        //throw new ProviderUnavailableException(ProviderTypes.HotelBeds.ToString(), $"No response to {nameof(AvailabilityRQ)}.", null);
                        throw new ProviderUnavailableException(ProviderTypes.HotelBeds.ToString(), hotelBedsHotels.error.message.ToString(), null);
                    }
                    hotelSearchResults = ConvertToProviderResponse(hotelBedsHotels);
                }
                catch (HotelBedsProviderException e)
                {
                    if (e.HasKnownError(HotelBedsProviderException.HotelKnownError.NoListings))
                    {
                        hotelSearchResults = new HotelAvailabilityProviderRes()
                        { Hotels = new List<HotelSearchResultItem>() };

                    }
                    else
                    {
                        throw;
                    }
                }
            }
            Logger.Instance.LogFunctionExit(this.GetType().Name, "Execute");
            return hotelSearchResults;
        }

        private AvailabilityRQ ConvertToHotelBedsSearchRequest(HotelAvailabilityProviderReq request)
        {
            AvailabilityRQ hotelBedsAvailabilityRQ = new AvailabilityRQ();

            if (request.HotelCodes != null && request.HotelCodes.Any())
            {
                hotelBedsAvailabilityRQ.hotels = new HotelsFilter()
                {
                    hotel = request.HotelCodes.Select(int.Parse).ToList()
                };
            }
            else
            {
                hotelBedsAvailabilityRQ.geolocation = new com.hotelbeds.distribution.hotel_api_model.auto.model.GeoLocation
                {
                    latitude = Convert.ToDouble(request.GeoLocation.Latitude),
                    longitude = Convert.ToDouble(request.GeoLocation.Longitude),
                    radius = HotelBedsConstants.RadiusLimit,
                    unit = com.hotelbeds.distribution.hotel_api_model.util.UnitMeasure.UnitMeasureType.km
                };
            }

            hotelBedsAvailabilityRQ.filter = new Filter
            {
                minCategory = request.MinRating,
                maxCategory = request.MaxRating
            };

            hotelBedsAvailabilityRQ.stay = new Stay(request.CheckInDate, request.CheckOutDate, 0, true);

            hotelBedsAvailabilityRQ.occupancies = new List<Occupancy>();
            hotelBedsAvailabilityRQ.occupancies.Add(new Occupancy
            {
                adults = request.TotalAdults,
                rooms = request.TotalRooms,
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

            return hotelBedsAvailabilityRQ;
        }

        private HotelAvailabilityProviderRes ConvertToProviderResponse(AvailabilityRS request)
        {
            HotelAvailabilityProviderRes hotelResultRS = new HotelAvailabilityProviderRes();

            var hotels = new List<HotelSearchResultItem>();
            if (request?.hotels?.hotels != null && request.hotels.hotels.Any())
            {
                foreach (var htl in request.hotels.hotels)
                {
                    var searchResult = new HotelSearchResultItem()
                    {
                        HotelInfo = new HotelInfo()
                        {
                            HotelName = htl.name,
                            HotelCode = htl.code.ToString(),
                            Description = htl.zoneCode + ", " + htl.zoneName,
                            Rating = Convert.ToInt16(htl.categoryName.Split(HotelBedsConstants.SpaceSeperator)[0]),

                            Location = new ServiceCatalogues.HotelCatalog.Dtos.GeoLocation()
                            {
                                Latitude = Convert.ToDecimal(htl.latitude),
                                Longitude = Convert.ToDecimal(htl.longitude)
                            }
                        },
                        Price = new Models.TMoney()
                        {
                            Amount = htl.minRate,
                            Currency = new TCurrency()
                            {
                                CurrencyCode = htl.currency
                            }
                        }

                    };
                    hotels.Add(searchResult);
                }
                hotelResultRS.Hotels = hotels;
            }
            else
                hotelResultRS.Hotels = null;

            return hotelResultRS;
        }


    }
}
