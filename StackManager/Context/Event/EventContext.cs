using System.Collections.Generic;

namespace StackManager.Context.Event
{
    public class EventContext
    {
        public int EventId { get; set; }
        public int DeviceId { get; set; }
        public EventType EventType { get; set; } = EventType.None;
        public string Source { get; set; }
        public string Target { get; set; }
        public Dictionary<string, object> Parameters { get; } = new Dictionary<string, object>();

        public EventContext Setter(object value, string key = "__key_of_dict__")
        {
            if (Parameters.TryGetValue(key, out _))
            {
                Parameters[key] = value;
            }
            else
            {
                Parameters.Add(key, value);
            }

            return this;
        }

        public T Getter<T>(string key = "__key_of_dict__")
        {
            if (Parameters.TryGetValue(key, out object obj))
            {
                return (T)obj;
            }
            return default;
        }
    }

    public enum EventType : int
    {
        None = 0,

        #region Common
        PostMessage,
        TimerUpdated,
        SelfTrigger,
        RemoteTrigger,
        ProfileChanged,
        #endregion

        #region Scanner
        ScannerRequest,
        InputScannerResponse,
        OutputScannerResponse,
        #endregion

        #region View
        UpdateView,
        ReportPQM,
        #endregion
    }
}
