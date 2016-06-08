using System;
using TE.Common;
using TE.Core.ServiceCatalogues.HotelCatalog.Dtos;
using TE.Core.ServiceCatalogues.HotelCatalog.Provider;
using TE.Core.Tourico.Hotel;
using TE.ThirdPartyProviders.Tourico.TouricoIHotelFlowSvc;


namespace TE.Tourico.Hotel
{
    public class TouricoRetrieveHotelPropertyHandler: HandlerBase<HotelPropertyProviderReq, HotelPropertyProviderRes>
    {
        public TouricoRetrieveHotelPropertyHandler()
        {
        }

        public override HotelPropertyProviderRes Execute(HotelPropertyProviderReq request)
        {
            using (var touricoWorker = new TouriWorker())
            {
                return GetRates(touricoWorker, request);
            }
           
        }

        private HotelPropertyProviderRes GetRates(TouriWorker touricoWorker, HotelPropertyProviderReq request)
        {
            //Create Request
            var hotelDescriptionReq = this.TransformHotelPropertyDescriptionRequest(request);

            //Call the API
            //var hotelDescrptionRes = touricoWorker
            //    .Execute<HotelPropertyDescriptionRQ, HotelPropertyDescriptionRS>(hotelDescriptionReq);

            TouriWorker tw = new TouriWorker();
            var hotelDescrptionRes = tw.Execute(hotelDescriptionReq);

            //Tranform Response
            return this.TransformHotelPropertyDescriptionResponse(
                hotelDescrptionRes,
                request.CheckInDate,
                request.CheckOutDate);
        }

        private HotelID[] TransformHotelPropertyDescriptionRequest(HotelPropertyProviderReq request)
        {
            return new HotelID[] { new HotelID { id = Convert.ToInt32(request.HotelCode) } };
        }

        private HotelPropertyProviderRes TransformHotelPropertyDescriptionResponse(TWS_HotelDetailsV3 response, DateTime checkInDate, DateTime checkOutDate)
        {
            var hotelPropertyProviderRes = new HotelPropertyProviderRes();

            hotelPropertyProviderRes.HotelDetails = TransformHotelPropertyDetails(response);
            hotelPropertyProviderRes.HotelInfo = TransformHotelPropertyInfo(response);

            return hotelPropertyProviderRes;
        }

        private HotelDetail TransformHotelPropertyDetails(TWS_HotelDetailsV3 response)
        {
            var hotelDetail = new HotelDetail();             

            //Basic Property Information
            var basicPropertyInfo = response.Location[0];      
           
                hotelDetail.Latitude = basicPropertyInfo.latitude;
                hotelDetail.Longitude = basicPropertyInfo.longitude;
            
            //Total No-of floors / Rooms - in Tourico - msanka ?         
                hotelDetail.NumberOfFloors
                    = basicPropertyInfo.HotelRow.rooms;

            //Hotel Address Information                         
                hotelDetail.HotelFullAddress.Add(basicPropertyInfo.address);
                hotelDetail.HotelFullAddress.Add(basicPropertyInfo.zip);


            //Hotel Award Providers
            //None for Tourico

            //Hotel Contact Information
            hotelDetail.HotelFaxNumber
                        = basicPropertyInfo.HotelRow.hotelFax;                        
          
            //Hotel Property Option   -- Aminities from Tourico
            foreach (var aminity in basicPropertyInfo.HotelRow.GetAmenitiesRows())
            {
                foreach (var am in aminity.GetAmenityRows())
                {
                    hotelDetail.MiscellaneousServices.Add(am.name);
                }                
            }
                
            //hotelDetail.HotelOptions 

            //Hotel Property Types
          
            //Hotel Attractions              

            //Cancellation Info      
            
            //Location/Facilities/Food/Payment info 
                foreach (var descRow in basicPropertyInfo.HotelRow.GetDescriptionsRows())
                {
                    foreach(var longdescRow in descRow.GetLongDescriptionRows())
                    {
                        hotelDetail.Description.Add(longdescRow.FreeTextLongDescription); //description row

                        foreach (var dRow in longdescRow.GetDescriptionRows())
                        {
                            switch (dRow.category)
                            {
                                case "Location":
                                    hotelDetail.LocationDetails.Add(dRow.value);
                                    break;
                                case "Facilities":
                                    hotelDetail.HotelFacilities.Add(dRow.value); //Hotel Facilities
                                    break;
                                case "Rooms":
                                    hotelDetail.HotelRoomServices.Add(dRow.value); //Available Room Service
                                    break;

                                case "Sports/Entertainment":
                                    hotelDetail.HotelRecreationServices.Add(dRow.value);
                                    break;
                                case "Meals":
                                    hotelDetail.DiningDetails.Add(dRow.value); //Dining Info
                                    break;
                                case "Payment":
                                    hotelDetail.DepositPolicies.Add(dRow.value); //Deposit policy
                                    break;
                            }
                        }
                    }

                }

            //Direction Details
            var directionInfo = response.RefPoints[0];
                    hotelDetail.Directions.Add(directionInfo.type);
                    hotelDetail.Directions.Add(directionInfo.name);
                    hotelDetail.Directions.Add(directionInfo.direction);
                    hotelDetail.Directions.Add(directionInfo.distance.ToString());
                    hotelDetail.Directions.Add(directionInfo.unit);

            //Hotel Guarantee Details               
            //Marketing Info
            //Miscellaneous Services 
            //HotelPolicies   
            //Available Safety Services
            //Hotel Services               
            //Available Transportation details



            return hotelDetail;
        }


        
        private TE.Core.ServiceCatalogues.HotelCatalog.Dtos.HotelInfo TransformHotelPropertyInfo(TWS_HotelDetailsV3 response)
        {
            var hotelInfo = new TE.Core.ServiceCatalogues.HotelCatalog.Dtos.HotelInfo();

            var basicPropertyInfo = response.Hotel[0];

            //Hotel code
            hotelInfo.HotelCode = basicPropertyInfo.hotelID.ToString();

            //Hotel Name
            hotelInfo.HotelName = basicPropertyInfo.name;

            //Chain Code      
#pragma warning disable CS0472 // The result of the expression is always the same since a value of this type is never equal to 'null'
            //if (!(basicPropertyInfo.brandId == null))
#pragma warning restore CS0472 // The result of the expression is always the same since a value of this type is never equal to 'null'
            hotelInfo.HotelChainCode = "";// basicPropertyInfo.brandId.ToString();
            
            //description
            hotelInfo.Description = basicPropertyInfo.GetDescriptionsRows()[0].GetLongDescriptionRows()[0].FreeTextLongDescription;

            //image url
            hotelInfo.HeroImageUrl = basicPropertyInfo.thumb;
                       
            //City Code
            hotelInfo.CityCode = basicPropertyInfo.GetLocationRows()[0].destinationCode;

            //Hotel Award Provider           
            hotelInfo.Rating = Convert.ToInt16(basicPropertyInfo.starLevel);


            //Hotel Contact Information
            hotelInfo.PhoneNumber = basicPropertyInfo.hotelPhone;

            //Address
                     
            var hotelAddress = new Core.Shared.Dtos.AddressDto()
            {
                Address1 = response.Location[0].address,
                Address2 = "",
                City = response.Location[0].city,
                Zip = response.Location[0].zip                
            };

            hotelInfo.Address = hotelAddress;

            //Geo Code        

            hotelInfo.Location = new GeoLocation()
            {
                Latitude = response.Location[0].latitude,
                Longitude = response.Location[0].longitude
                };
            
            return hotelInfo;
        }

    }
}