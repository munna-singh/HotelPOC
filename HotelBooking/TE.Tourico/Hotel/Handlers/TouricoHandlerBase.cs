using TE.Common;
using TE.Core.Hotel.Messaging;

namespace TE.Core.Tourico.Hotel.Handler
{
    public abstract class TouricoHandlerBase<TReq, TRes> : HandlerBase<TReq, TRes>
    {
        protected readonly TouricoPipelineManager PipelineManager;

        protected TouricoHandlerBase(TouricoPipelineManager pipelineManager = null)
        {
            PipelineManager = pipelineManager ?? new TouricoPipelineManager();
        }

    }
}
