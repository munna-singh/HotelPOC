using Common;
using Common.Sabre.Hotels.TravelItineraryReadInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager
{
    public class RetrievePnrDetails
    {
        public TravelItineraryReadRS GetDetails(string pnrIdentifier)
        {
            var session = SabreSessionManager.Create();

            var t = new ReadTravelerInfo().ReadInfo(session.SecurityValue.BinarySecurityToken, pnrIdentifier);

            SessionClose close = new SessionClose();
            close.Close(session.SecurityValue.BinarySecurityToken);

            return t;
        }
    }
}
