using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Exceptions
{
    public class PipelineFilterException : Exception
    {
        protected PipelineFilterException(string message)
            : base(message)
        {
        }

        protected PipelineFilterException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
