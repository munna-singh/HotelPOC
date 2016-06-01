using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TE.HotelBeds.Provider.Messaging
{
    /// <summary>
    /// Persist session details for Hotel Beds SOAP requests.
    /// </summary>
    public class HotelBedsSession
    {
        /// <summary>
        /// Identifies the session's IPCC.
        /// </summary>
        public int HotelBedsPseudoCityCodeId { get; set; }

        /// <summary>
        /// Security token to be used for all messages in the same session.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Conversation ID used for all messages in the same session.
        /// </summary>
        public string ConversationId { get; set; }

        /// <summary>
        /// Pseudo City Code, kept to avoid DB look ups on every request.
        /// </summary>
        public string IPCC { get; set; }
    }
}
