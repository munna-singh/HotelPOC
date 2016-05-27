using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Exceptions
{
    [Description("Base class for all errors thrown by the provider. Treat as critical, user cannot fix.")]
    public class ProviderException : TravelEdgeException
    {
        public String ProviderName { get; set; }
        public String ProviderCode { get; set; }
        public String ProviderMessage { get; set; }

        public ProviderException(String providerName, Exception innerException)
            : base(null, innerException)
        {
            ProviderName = providerName;
        }

        public ProviderException(String providerName, String developerMessage, Exception innerException)
            : base(developerMessage, innerException)
        {
            ProviderName = providerName;
        }

        public ProviderException(String providerName, String providerCode, String providerMessage, String developerMessage, Exception innerException)
            : base(developerMessage, innerException)
        {
            this.ProviderCode = providerCode;
            this.ProviderMessage = providerMessage;
            this.ProviderName = providerName;
        }
    }
}
