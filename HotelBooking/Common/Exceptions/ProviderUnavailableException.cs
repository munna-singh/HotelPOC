using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Exceptions
{
    public class ProviderUnavailableException : ProviderException
    {
        public ProviderUnavailableException(String providerName, Exception innerException)
            : base(providerName, innerException)
        {
            ProviderName = providerName;
        }

        public ProviderUnavailableException(String providerName, String developerMessage, Exception innerException)
            : base(providerName, developerMessage, innerException)
        {
            ProviderName = providerName;
        }
    }
}
