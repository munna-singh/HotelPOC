
using System;
using System.Collections.Generic;
using TE.Core.ServiceCatalogues.HotelCatalog.Enums;
using TE.Core.ServiceCatalogues.HotelCatalog.Dtos.Controller;
using TE.DataAccessLayer.Models;
using TE.Core.ServiceCatalogues.HotelCatalog.Dtos;
using TE.Core.ServiceCatalogues.HotelCatalog.Provider;
using TE.ThirdPartyProviders.Tourico.TouricoIHotelFlowSvc;
using TE.Core.Hotel.Messaging;
using TE.Core.Hotel.Tourico.Exceptions;
using TE.Common.Exceptions;
using TE.DataAccessLayer.Enums;


namespace TE.Core.Tourico.Hotel.Handler
{
    public class TouricoHotelAvailabilityHandler : TouricoHandlerBase<HotelAvailabilityProviderReq, HotelAvailabilityProviderRes>
    {

        public TouricoHotelAvailabilityHandler(TouricoPipelineManager pipelineManager=null) 
             : base(pipelineManager)
        {

        }

        public override HotelAvailabilityProviderRes Execute(HotelAvailabilityProviderReq request)
        {
            if (request.LocationType == ServiceCatalogues.HotelCatalog.Enums.LocationTypes.Unknown)
            {
                //throw new ArgumentNullException(nameof(request.LocationType));
            }
            if (request.CheckInDate <= DateTime.Today)
            {
                throw new ArgumentOutOfRangeException(nameof(request.CheckInDate));
            }
            if (request.CheckInDate >= request.CheckOutDate)
            {
                throw new ArgumentOutOfRangeException(nameof(request.CheckOutDate));
            }
            if (request.TotalAdults < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(request.TotalAdults));
            }

            using (
                    var touricoWorker = new TouricoWorker(pipelineManager: PipelineManager)
                  )
            {

                //convert request dto to Tourico search request
                SearchHotelsByIdRequest1 TouricoHotelRequest = ConvertToTouricoHotelSearchRequest(request);
                
                HotelAvailabilityProviderRes hotelSearchResults;

                try
                {
                    var touricoRespone = touricoWorker.Execute<SearchHotelsByIdRequest1, SearchHotelsByIdResponse>(TouricoHotelRequest);

                    if(touricoRespone == null)
                    {
                        throw new ProviderUnavailableException(ProviderTypes.Tourico.ToString(), $"No response to {nameof(SearchHotelsByIdRequest1)}.", null);

                    }

                    //convert response to HotelAvailabilityProviderRes
                    hotelSearchResults = this.ConvertToProviderResponse(touricoRespone, request);

                }
                catch (TouricoProviderException e)
                {

                    throw e;
                }
                
                return hotelSearchResults;
            }
        }

        private HotelAvailabilityProviderRes ConvertToProviderResponse(SearchHotelsByIdResponse response, HotelAvailabilityProviderReq request)
        {

            HotelAvailabilityProviderRes providerResp;
            if (response.SearchHotelsByIdResult.HotelList.Length == 0)
            {
                throw new ApplicationException("AvailabilityOptions is null.");
            }
            
            providerResp = new HotelAvailabilityProviderRes();
            
            var hotels = new List<HotelSearchResultItem>();
            foreach (var hotel in response.SearchHotelsByIdResult.HotelList)
            {
                var hotelResult = new HotelSearchResultItem();
                hotelResult.HotelInfo = new TE.Core.ServiceCatalogues.HotelCatalog.Dtos.HotelInfo();

                hotelResult.HotelInfo.Rating = Convert.ToInt16(hotel.starsLevel); //tourico returns in decimal to imply 3.5, 4.5 etc
                hotelResult.HotelInfo.RatingType = RatingTypes.Star;
                hotelResult.HotelInfo.Address = new Shared.Dtos.AddressDto()
                {
                    Address1 = hotel.Location.address,
                    Address2 = "",
                    City = hotel.Location.city,
                    Zip = ""
                };
                hotelResult.HotelInfo.HotelCode = hotel.hotelId.ToString();
                hotelResult.HotelInfo.HotelChainCode = hotel.brandId.ToString();
                hotelResult.HotelInfo.HotelName = hotel.name;
                hotelResult.HotelInfo.CityCode = hotel.Location.searchingCity; //? city code
                hotelResult.HotelInfo.Thumbnails = hotel.thumb;
                
                //Currency and Currency Code
                if (hotel.minAverPublishPrice > 0)
                {
                    hotelResult.Price = new TMoney
                    {
                        Amount = hotel.minAverPublishPrice,
                        Currency = new TCurrency
                                                { CurrencyCode = hotel.currency }
                    };
                }

                hotelResult.HotelInfo.PhoneNumber = null; // Available only in GetHotelDetailsV3 

                hotelResult.HotelInfo.Provider = DataAccessLayer.Enums.ProviderTypes.Tourico;         
                             
                //lat and long
                 hotelResult.HotelInfo.Location = new GeoLocation
                    {
                        Latitude = hotel.Location.latitude,
                        Longitude = hotel.Location.longitude
                    };
               

                hotelResult.HotelInfo.SpecialOffers = null;
                hotelResult.HotelInfo.Amenities = null; // Available only in GetHotelDetailsV3 

                //Room Type info
                hotelResult.HotelInfo.SpecialOffers = hotel.RoomTypes.ToString(); // musanka : remove assigning to SpecialOffers property

                hotels.Add(hotelResult);
                

            }
            
            providerResp.Hotels = hotels;
            return providerResp;
        }

        private SearchHotelsByIdRequest1 ConvertToTouricoHotelSearchRequest(HotelAvailabilityProviderReq request)
        {                                   

            SearchHotelsByIdRequest sRequest = new SearchHotelsByIdRequest();
                        
            sRequest.CheckIn = request.CheckInDate;
            sRequest.CheckOut = request.CheckOutDate;
            sRequest.RoomsInformation = new RoomInfo[] { new RoomInfo {  AdultNum = request.TotalAdults,
                                                                         ChildNum = 0 , ChildAges = new ChildAge[] { new ChildAge {  age = 0 } } }  };
            int i = 0;

            HotelIdInfo[] hIdInfo = new HotelIdInfo[request.HotelCodes.Count];          
            foreach (var hotelcode in request.HotelCodes)
            {
                HotelIdInfo hinfo = new HotelIdInfo();
                hinfo.id = Convert.ToInt32(hotelcode);
                hIdInfo[i] = hinfo;   i++;           
            }

            sRequest.HotelIdsInfo = hIdInfo;

            SearchHotelsByIdRequest1 returnRequest = new SearchHotelsByIdRequest1();          
            returnRequest.AuthenticationHeader = Helper.GetTouricoAuthHeader();
            returnRequest.request = sRequest;

            return returnRequest;

        }
    }
}
