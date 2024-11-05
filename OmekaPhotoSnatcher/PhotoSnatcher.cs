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
                string filenameShort = "Item_" + item["id"].ToString("0000");
                int count = item["files"]["count"];
                if (count != 0)
                {
                    GetApiResponse(url, filenameShort).Wait();
                }
                
            }
            
        }
        public static async Task GetApiResponse(string url, string filenameShort)
        {
            try
            {
                using HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                var oResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(responseBody);
                int count = oResponse.Count;
                string filename = filenameShort + ".jpg";

                for (int i = 0; i < count; i++)
                {
                    string originalUrl = oResponse[i]["file_urls"]["original"];

                    if (count > 1)
                    {
                        int x = i + 1;
                        filename = filenameShort + "_" + x + ".jpg";
                    }
                    

                    SaveImage(originalUrl, filename).Wait();

                }
                


            }
            catch (HttpRequestException e)
            {
                if (e.Message == "Response status code does not indicate success: 429 (Too Many Requests).")
                {
                    Logger.WriteLine("\nException Caught!");
                    Logger.WriteLine("Message : " + e.Message);
                    Logger.WriteLine(filenameShort);
                    Logger.SaveLog();
                    Thread.Sleep(1000);
                    GetApiResponse(url, filenameShort).Wait();
                }
                else
                {
                    Logger.WriteLine("\nException Caught!");
                    Logger.WriteLine("Message : " + e.Message);
                    Logger.WriteLine(filenameShort);
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
