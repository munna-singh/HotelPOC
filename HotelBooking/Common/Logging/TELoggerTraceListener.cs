using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TE.Common.Logging;

namespace Common.Logging
{
    public class TELoggerTraceListener : System.Diagnostics.TraceListener
    {
        //private readonly log4net.ILog _log;

        public TELoggerTraceListener()
        {
        }

        public override void Write(string message)
        {
            Logger.Instance.Debug("TELoggerTraceListener", "Write", message);
        }

        public override void WriteLine(string message)
        {
            Logger.Instance.Debug("TELoggerTraceListener", "WriteLine", message);
        }
    }
}
