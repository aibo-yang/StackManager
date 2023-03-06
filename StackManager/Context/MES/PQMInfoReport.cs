using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace StackManager.Context.MES
{
    [JsonObject(MemberSerialization.OptIn)]
    class ParamInfo
    {
        [JsonProperty(PropertyName = "paramCode")]
        public string ParamCode { get; set; }

        [JsonProperty(PropertyName = "paramValue")]
        public string ParamValue { get; set; }
    }

    [JsonObject(MemberSerialization.OptIn)]
    class PQMReportInfo
    {
        public PQMReportInfo()
        {
            // 产品生产过程中是否发生设备故障和品质问题 1-又发生 0-没有发生
            ParamList.Add(new ParamInfo { ParamCode="CT_M", ParamValue = "0" });
            ParamList.Add(new ParamInfo { ParamCode="CT_Q", ParamValue = "0" });
        }

        [Display(Name = "设备编号")]
        [JsonProperty(PropertyName = "interfaceID")]
        public string InterfaceID { get; set; }

        //[Display(Name = "设备型号")]
        //[JsonProperty(PropertyName = "equipType")]
        //public string EquipType { get; set; }

        [Display(Name = "设备状态[0-正常 1-故障报警中 2-暂停 3-待机(无生产) 4-待料 5-产出满料 6-材料低位预警 7-执行换线中]")]
        [JsonProperty(PropertyName = "status")]
        public int Status { get; set; }

        [Display(Name = "详细故障代码")]
        [JsonProperty(PropertyName = "statusCode")]
        public string StatusCode { get; set; }

        [Display(Name = "良品数")]
        [JsonProperty(PropertyName = "passQty")]
        public int PassQty { get; set; }

        [Display(Name = "不良数")]
        [JsonProperty(PropertyName = "failQty")]
        public int FailQty { get; set; }

        [Display(Name = "报警次数")]
        [JsonProperty(PropertyName = "errorCnt")]
        public int ErrorCnt { get; set; }

        [Display(Name = "报警时长[0.1秒]")]
        [JsonProperty(PropertyName = "errorTimes")]
        public double ErrorTimes { get; set; }

        [Display(Name = "单台产品生产时长[0.1秒]")]
        [JsonProperty(PropertyName = "cycleTime")]
        public double CycleTime { get; set; }

        [Display(Name = "运行时长[0.1秒]")]
        [JsonProperty(PropertyName = "runningTime")]
        public double RunningTime { get; set; }

        [Display(Name = "等待时长[0.1秒]")]
        [JsonProperty(PropertyName = "waitingTime")]
        public double WaitingTime { get; set; }

        [Display(Name = "开机自检结果[1-OK 2-FAIL]")]
        [JsonProperty(PropertyName = "selfCheck")]
        public int SelfCheck { get; set; }

        [Display(Name = "投入数")]
        [JsonProperty(PropertyName = "inputQty")]
        public int InputQty { get; set; }

        [Display(Name = "条码")]
        [JsonProperty(PropertyName = "barcode")]
        public string Barcode { get; set; }

        [Display(Name = "当前执行机种")]
        [JsonProperty(PropertyName = "model")]
        public string Model { get; set; }

        //[Display(Name = "收集时间")]
        //[JsonProperty(PropertyName = "collectDate")]
        //public string CollectDate { get; set; }

        [Display(Name = "差异化参数")]
        [JsonProperty(PropertyName = "paramList")]
        public ICollection<ParamInfo> ParamList { get; set; } = new List<ParamInfo>();
    }
}
