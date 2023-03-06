using System.Collections.Generic;
using Newtonsoft.Json;

namespace StackManager.Context.MES
{
    [JsonObject(MemberSerialization.OptIn)]
    class BoxInfoRequest
    {
        [JsonProperty(PropertyName = "factory")]
        public string Factory { get; set; } = "WJ2";

        [JsonProperty(PropertyName = "testType")]
        public string TestType { get; set; } = "GET_CARTON_PACKING_INFO";

        [JsonProperty(PropertyName = "routingData")]
        public string RoutingData { get; set; } = "MPU220504660174";

        [JsonProperty(PropertyName = "testData")]
        public ICollection<string> TestData { get; set; } = new List<string>();
    }
}
