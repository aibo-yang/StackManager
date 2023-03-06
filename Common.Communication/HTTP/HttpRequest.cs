using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.GZip;
using Newtonsoft.Json;

namespace Common.Communication.HTTP
{
    public class HttpRequest
    {
        public static async Task<string> PostJsonAsync(string url, string json, Dictionary<string, string> header = null)
        {
            var client = new HttpClient();
            var content = new StringContent(json);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            if (header != null)
            {
                client.DefaultRequestHeaders.Clear();
                foreach (var item in header)
                {
                    client.DefaultRequestHeaders.Add(item.Key, item.Value);
                }
            }

            var response = await client.PostAsync(url, content);

            //response.EnsureSuccessStatusCode();
            if (!response.IsSuccessStatusCode)
            {
                return string.Empty;
            }

            return await response.Content.ReadAsStringAsync();
        }

        public static async Task<string> PostAsync(string url, string data, Dictionary<string, string> header = null, bool Gzip = false)
        {
            var client = new HttpClient(new HttpClientHandler() { UseCookies = false });
            var content = new StringContent(data);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            if (header != null)
            {
                client.DefaultRequestHeaders.Clear();
                foreach (var item in header)
                {
                    client.DefaultRequestHeaders.Add(item.Key, item.Value);
                }
            }
            var response = await client.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                return string.Empty;
            }

            //response.EnsureSuccessStatusCode();
            var responseBody = "";

            if (Gzip)
            {
                var inputStream = new GZipInputStream(await response.Content.ReadAsStreamAsync());
                responseBody = new StreamReader(inputStream).ReadToEnd();
            }
            else
            {
                responseBody = await response.Content.ReadAsStringAsync();
            }

            return responseBody;
        }

        public static async Task<string> GetAsync(string url, Dictionary<string, string> header = null, bool Gzip = false)
        {
            var client = new HttpClient(new HttpClientHandler() { UseCookies = false });
            if (header != null)
            {
                client.DefaultRequestHeaders.Clear();
                foreach (var item in header)
                {
                    client.DefaultRequestHeaders.Add(item.Key, item.Value);
                }
            }

            var response = await client.GetAsync(url);

            //response.EnsureSuccessStatusCode();//用来抛异常的
            if (!response.IsSuccessStatusCode)
            {
                return string.Empty;
            }

            var responseBody = "";
            if (Gzip)
            {
                GZipInputStream inputStream = new GZipInputStream(await response.Content.ReadAsStreamAsync());
                responseBody = new StreamReader(inputStream).ReadToEnd();
            }
            else
            {
                responseBody = await response.Content.ReadAsStringAsync();

            }
            return responseBody;
        }

        public static async Task<T> PostObjectAsync<T, T2>(string url, T2 obj)
        {
            var json = JsonConvert.SerializeObject(obj);
            var responseBody = await PostJsonAsync(url, json);
            return JsonConvert.DeserializeObject<T>(responseBody);
        }

        public static async Task<T> PostObjectWithHeadersAsync<T, T2>(string url, T2 obj, Dictionary<string, string> header = null)
        {
            var json = JsonConvert.SerializeObject(obj);
            // var responseBody = await PostJsonAsync(url, json);
            var responseBody = await PostAsync(url, json, header, false);
            return JsonConvert.DeserializeObject<T>(responseBody);
        }

        public static async Task<T> GetObjectAsync<T>(string url)
        {
            var responseBody = await GetAsync(url);
            return JsonConvert.DeserializeObject<T>(responseBody);
        }
    }
}
