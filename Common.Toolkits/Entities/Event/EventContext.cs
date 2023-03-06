using System.Collections.Generic;

namespace Common.Toolkits.Entities
{
    public class EventContext
    {
        public int EventId { get; set; }
        public int DeviceId { get; set; }
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
}
