using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using TE.Core.ServiceCatalogues.HotelCatalog.Provider;
using Newtonsoft.Json;
using TE.Core.Providers.Tourico.Hotel.Provider;


namespace UI.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/<controller>
        public  HotelAvailabilityProviderRes Get()
        {

            TouricoHotelSearchProvider searchProvider = new TouricoHotelSearchProvider();

            HotelAvailabilityProviderReq request = new HotelAvailabilityProviderReq();

            request.CheckInDate = DateTime.Now.AddDays(3).Date;
            request.CheckOutDate = DateTime.Now.AddDays(5).Date;

            request.HotelCodes = new System.Collections.Generic.List<string> { "8198" };
            //TODO: 
            //request.TotalAdults = 2;

            HotelAvailabilityProviderRes res = searchProvider.Search(request);
           
            return res;
        }

      
    }
}