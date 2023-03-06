using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Common.Communication.HTTP;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Serilog;

namespace Demo.IOT
{
    public class MesProfile
    {
        public string BaseUri = "http://10.146.192.40:10101/TDC/DELTA_DEAL_TEST_DATA_I";
        //public string BaseUri = "http://CNDGNMESWESP002:10101/TDC/DELTA_DEAL_TEST_DATA_I";
        public string Uri => $"{BaseUri}?sign={Sign}";
        public string Sign { get; set; } = string.Empty;
        public string TokenID { get; set; } = "894A0F0DF8494799E0530CCA940AC604";
        public string SecretKey { get; set; } = "894A0F0DF84A4799E0530CCA940AC604";
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class CartonInfoRequest
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

    [JsonObject(MemberSerialization.OptIn)]
    public class CartonInfoResponse
    {
        [JsonProperty(PropertyName = "result")]
        public string Result { get; set; }

        [JsonProperty(PropertyName = "description")]
        public object Description { get; set; }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            var barcode = "ERROR\r";
            var regex = new Regex(@"(MPU\d+)");
            var match = regex.Match(barcode);
            if (match.Success)
            {
                barcode = match.Groups[1].Value;
            }

            var appConfig = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json", true, true)
               .Build();

            var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss_ffff");

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(appConfig)
                .WriteTo.File(path: $"Logs\\log{timestamp}.txt", outputTemplate: "{Message}{NewLine}", shared: true)
                .WriteTo.Console(outputTemplate: "{Message}{NewLine}")
                .CreateLogger();

            while (true)
            {
                var boxCode = Console.ReadLine();
                if (string.IsNullOrEmpty(boxCode))
                {
                    break;
                }

                boxCode = boxCode.Trim();

                var mesProfile = new MesProfile();
                var req = new CartonInfoRequest() { RoutingData = boxCode};
                var raw = $"{mesProfile.SecretKey}{JsonConvert.SerializeObject(req)}";
                mesProfile.Sign = string.Join("", MD5.Create().ComputeHash(Encoding.ASCII.GetBytes(raw)).Select(x => x.ToString("X2")));

                var headers = new Dictionary<string, string>
                {
                    { "TokenID", mesProfile.TokenID },
                };

                var res = HttpRequest.PostJsonAsync($"{mesProfile.Uri}", $"{JsonConvert.SerializeObject(req)}", new Dictionary<string, string>() { { "TokenID", mesProfile.TokenID } }).Result;

                Console.WriteLine(mesProfile.Uri);
                Console.WriteLine(raw);
                Console.WriteLine(res);
            }

            Console.ReadLine();
        }
    }
}
