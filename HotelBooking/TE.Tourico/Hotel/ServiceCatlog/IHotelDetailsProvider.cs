using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TE.Core.Tourico.Hotel.Dtos;
using TE.Core.ServiceCatalogues.HotelCatalog.Dtos;


namespace TE.Core.Tourico.Hotel.ServiceCatlog
{
   public interface IHotelDetailsProvider
    {
        HotelInfo RetrieveHotelInfo(string hotelCode);
       
    }
}
