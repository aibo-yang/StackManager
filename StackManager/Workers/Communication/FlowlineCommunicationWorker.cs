using Common.Communication;
using Microsoft.Extensions.Logging;
using Prism.Events;
using StackManager.Context.PLC;

namespace StackManager.Workers
{
    public class FlowlineCommunicationWorker : PLCCommunicationWorker
    {
        public FlowlineCommunicationWorker(ILogger<FlowlineCommunicationWorker> logger, IEventAggregator eventAggregator)
            : base(logger, eventAggregator, nameof(FlowlineCommunicationWorker),
                  (int)DeviceId.FlowlinePLC,
                  "192.168.10.60",
                  new DeviceData[]
                  {
                      new FlowlineRequest(),
                      new FlowlineResponse(),
                      new FlowlineDevices(),
                  })
        {
        }
    }
}
