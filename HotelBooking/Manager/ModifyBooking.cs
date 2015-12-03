using Common;
using Common.Sabre.Hotels.TravelItineraryReadInfo;
using Common.Sabre.Hotels.ModifyReservation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager
{
    public class ModifyBooking
    {
        public HotelResModifyRS Modify(string pnr)
        {
            var session = SabreSessionManager.Create();
            ReadTravelerInfo readInfo = new ReadTravelerInfo();
            var result = readInfo.ReadInfo(session.SecurityValue.BinarySecurityToken, pnr);

            HotelResModifyRQ req = new HotelResModifyRQ();
            
            HotelResModifyService client = new HotelResModifyService();
            var modifyResult = client.HotelResModifyRQ(req);

            SessionClose close = new SessionClose();
            close.Close(session.SecurityValue.BinarySecurityToken);

            return modifyResult;
        }
    }
}
