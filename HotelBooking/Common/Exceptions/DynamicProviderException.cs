using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Exceptions
{
    [Description("These are for exceptions thrown by the provider. Must be given copy and 'business' checked, otherwise it will be treated as critical.")]
    public abstract class DynamicProviderException : ProviderException
    {
        public enum MessageSeverity
        {
            Error = 1,

            Warning,

            Message
        }

        protected class ExceptionMessage
        {
            public string Code;

            public string Message;

            public MessageSeverity Severity;
        }

        public DynamicProviderException(string providerName, Exception innerException)
            : base(providerName, innerException)
        {
        }

        public DynamicProviderException(string providerName, string developerMessage, Exception innerException)
            : base(providerName, developerMessage, innerException)
        {
        }

        public DynamicProviderException(string providerName, string providerCode, string providerMessage, string developerMessage, Exception innerException)
            : base(providerName, providerCode, providerMessage, developerMessage, innerException)
        {
        }

        protected IEnumerable<ExceptionMessage> messages;

        /// <summary>
        /// Checks if the response has a known error/warning/message.
        /// </summary>
        /// <param name="errorType">Expected error.</param>
        /// <param name="onlyErrors">Only return true if an error occured, ignore warnings/messages.</param>
        /// <returns>True if the error/warning/message occured.</returns>
        public abstract bool HasKnownError(Enum errorType, bool onlyErrors = true);
    }
}
