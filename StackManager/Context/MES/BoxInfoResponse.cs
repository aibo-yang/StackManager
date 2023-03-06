using System.Collections.Generic;
using Newtonsoft.Json;

namespace StackManager.Context.MES
{
    // {"result":"OK","description":{"carton_qty":24,"if_carton_full":"Y","pallet":"P2U212205060100","if_pallet_full":"Y","pallet_carton_qty":1,"line":"N21","mo":["3152204926"]}} 

    [JsonObject(MemberSerialization.OptIn)]
    class BoxInfoDescription
    {
        [JsonProperty(PropertyName = "carton_qty")]
        public int CartonQty { get; set; }

        [JsonProperty(PropertyName = "if_carton_full")]
        public string CartonIsFull { get; set; }

        [JsonProperty(PropertyName = "pallet")]
        public string PalletNo { get; set; }

        [JsonProperty(PropertyName = "if_pallet_full")]
        public string PalletIsFull { get; set; }

        [JsonProperty(PropertyName = "pallet_carton_qty")]
        public int PalletCartonQty { get; set; }

        [JsonProperty(PropertyName = "line")]
        public string LineNo { get; set; }

        [JsonProperty(PropertyName = "model")]
        public string ProductName { get; set; }

        [JsonProperty(PropertyName = "mo")]
        public ICollection<string> OrderNo { get; set; }
    }

    [JsonObject(MemberSerialization.OptIn)]
    class BoxInfoResponse
    {
        [JsonProperty(PropertyName = "result")]
        public string Result { get; set; }

        [JsonProperty(PropertyName = "description")]
        public BoxInfoDescription Description { get; set; }
    }
}
