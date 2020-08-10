using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace MLibrary.Business.REST
{
    public static class RestService
    {
        public static async Task<string> Post<T>(T preparedData,string url,Dictionary<string,string> headers)
        {
            string data = "";

            using (var client = new HttpClient())
            {
               

                string json = JsonConvert.SerializeObject(preparedData);
                var buffer = System.Text.Encoding.UTF8.GetBytes(json);

                var byteContent = new ByteArrayContent(buffer);

                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                foreach (var item in headers.Keys)
                {
                    byteContent.Headers.Add(item, headers[item]);
                }

                var response = await client.PostAsync(url, byteContent);

                if (response.IsSuccessStatusCode)
                {
                    data = await response.Content.ReadAsStringAsync();
                    

                    
                }
            }

            return data;
        }

        public static async Task<string> Get<T>(string url)
        {
            string data = "";

            using (var client = new HttpClient())
            {


                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    data = await response.Content.ReadAsStringAsync();



                }
            }

            return data;
        }
    }
}
