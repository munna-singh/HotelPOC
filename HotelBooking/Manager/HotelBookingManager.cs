using Common;
using Common.Sabre.Hotels.EndTransaction;
using Common.Sabre.Hotels.TravelItineraryReadInfo;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager
{
    public class HotelBookingManager
    {
        public EndTransactionRS Book(HotelSelectDto hotelSelectDto)
        {
            var session = SabreSessionManager.Create();
            hotelSelectDto.SessionId = session.SecurityValue.BinarySecurityToken;

            try
            {
                var hotelDescription = new HotelPropertyDescription()
                    .HotelDescription(hotelSelectDto);

                var addTravelerInfo = new AddTravelerInfo()
                    .AddTraveler(session.SecurityValue.BinarySecurityToken);

                var bookHotel = new BookHotel()
                    .Book(session.SecurityValue.BinarySecurityToken, hotelSelectDto.propertyRphNumber);

                var readTravelerInfo = new ReadTravelerInfo()
                    .ReadInfo(session.SecurityValue.BinarySecurityToken);

                var endTransaction = new EndTransaction().End(session.SecurityValue.BinarySecurityToken);

                return endTransaction;
                //var pnrDetails = new ReadTravelerInfo().ReadInfo(session.SecurityValue.BinarySecurityToken, endTransaction.ItineraryRef.ID);
                //return pnrDetails;
            }
            catch
            {
                throw;
            }
            finally
            {
                SessionClose close = new SessionClose();
                close.Close(session.SecurityValue.BinarySecurityToken);
            }
        }
    }
}
