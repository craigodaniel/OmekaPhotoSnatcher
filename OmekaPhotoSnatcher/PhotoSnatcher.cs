using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OmekaPhotoSnatcher
{
    public class PhotoSnatcher
    {
        
        static readonly HttpClient client = new HttpClient();

        public static void Main(string filePath)
        {
            Logger.WriteLine("==========================================================");
            Logger.WriteLine("Reading file " + filePath + "...\n");
            Logger.SaveLog();
            string jsonStringCollection = File.ReadAllText(filePath);
            var oJson = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(jsonStringCollection);
            foreach (var item in oJson) 
            {
                string url = item["files"]["url"];
                string filename = "Item_" + item["id"].ToString("0000") + ".jpg";
                
                GetApiResponse(url, filename).Wait();
            }
            
        }
        public static async Task GetApiResponse(string url, string filename)
        {
            try
            {
                using HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                var oResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(responseBody);
                string originalUrl = oResponse[0]["file_urls"]["original"];

                SaveImage(originalUrl, filename).Wait();


            }
            catch (HttpRequestException e)
            {
                if (e.Message == "Response status code does not indicate success: 429 (Too Many Requests).")
                {
                    Thread.Sleep(1000);
                    GetApiResponse(url, filename).Wait();
                }
                else
                {
                    Logger.WriteLine("\nException Caught!");
                    Logger.WriteLine("Message : " + e.Message);
                    Logger.WriteLine(filename);
                    Logger.SaveLog();
                }
                
            }
        }

        public static async Task SaveImage(string url, string filename)
        {

            using (var client = new HttpClient())
            {
                try
                {
                    var response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    using (var stream = await response.Content.ReadAsStreamAsync())
                    using (var fileStream = new FileStream(filename, FileMode.Create))
                    {
                        await stream.CopyToAsync(fileStream);
                    }

                    Logger.WriteLine("Image downloaded successfully: " + filename);
                    Logger.SaveLog();
                }
                catch (HttpRequestException ex)
                {
                    Logger.WriteLine("Failed to download image: " + ex.Message);
                    Logger.SaveLog();
                }
            }
        }

    }
}
