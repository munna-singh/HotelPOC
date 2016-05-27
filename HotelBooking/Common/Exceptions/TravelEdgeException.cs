using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Exceptions
{
    /// <summary>
    /// Base class for any critical or unclassified error. User will not know what to do with this until it has been given copy.
    /// </summary>
    [Description("Any critical or unclassified error. User will not know what to do with this until it has been classified & given user text")]
    public class TravelEdgeException : Exception
    {
        [Obsolete]
        public String UserDisplayMessage { get; set; }

        public Enum ErrorCode { get; set; }

        [Obsolete]
        public TravelEdgeException(String userDisplayMessage, String developerMessage, Exception innerException = null)
            : base(developerMessage ?? "User Message: " + userDisplayMessage, innerException)
        {
            UserDisplayMessage = userDisplayMessage;
        }

        public TravelEdgeException(String developerMessage, Exception innerException = null)
            : base(developerMessage + " " + innerException?.Message, innerException)
        {
        }

        public TravelEdgeException(Enum errorCode, String developerMessage, Exception innerException = null)
            : base(developerMessage, innerException)
        {
            this.ErrorCode = errorCode;
        }
    }
}
