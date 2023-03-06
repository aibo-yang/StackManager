using Common.Communication;
using Microsoft.Extensions.Logging;
using Prism.Events;
using StackManager.Context.PLC;

namespace StackManager.Workers
{
    public class StackingCommunicationWorker : PLCCommunicationWorker
    {
        public StackingCommunicationWorker(ILogger<StackingCommunicationWorker> logger, IEventAggregator eventAggregator)
            : base(logger, eventAggregator, nameof(StackingCommunicationWorker),
                  (int)DeviceId.FlowlinePLC,
                  "192.168.10.2",
                  new DeviceData[]
                  {
                      new StackingRequest(),
                      new StackingResponse(),
                      new StackingDevices(),
                  })
        {
        }
    }
}
