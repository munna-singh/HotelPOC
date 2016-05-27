using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using TE.Common.Logging;

namespace Common.Logging
{
    public class LogicalOperation : IDisposable
    {
        private Guid fromActivityId;
        private string Operation;

        public LogicalOperation(String operation, params string[] attributes)
        {
            this.Operation = operation;

            if (attributes != null && attributes.Any())
            {
                Operation += " " + String.Join(",", attributes);
            }

            Trace.CorrelationManager.ActivityId = Guid.NewGuid();

            this.fromActivityId = Trace.CorrelationManager.ActivityId;
            var newGuid = Guid.NewGuid();

            Logger.Instance.Warn("TransferTo", newGuid.ToString(), Operation);

            Trace.CorrelationManager.ActivityId = newGuid;
        }


        public void Dispose()
        {
            var current = Trace.CorrelationManager.ActivityId;

            Trace.CorrelationManager.ActivityId = fromActivityId;

            Logger.Instance.Warn("TransferFrom", current.ToString(), Operation);
        }
    }
}
