using MeteoDesktopSolution.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace MeteoDesktopSolution.Data
{
    internal class DataParser
    {
        public DataParser() { 
        }
        public static async Task<Station[]> getStations() {
            var client = new HttpClient { BaseAddress = new Uri("https://www.euskalmet.euskadi.eus/vamet/stations/stationList/stationList.json") };
            var responseMessage = await client.GetAsync("",HttpCompletionOption.ResponseContentRead);
            var resultArray = await responseMessage.Content.ReadAsStringAsync();
            var personList = JsonConvert.DeserializeObject<List<Station>>(resultArray);
            Debug.WriteLine(personList.ToArray().Length);
            return personList.ToArray();
        }

        public static async void getStationData(String stationId) {
            
            DateTime localDate = DateTime.Now;
            String month = localDate.Month.ToString();
            if (month.Length == 1) {
                month = "0" + month;
            }
            String day = localDate.Day.ToString();
            String year = localDate.Year.ToString();
            String requestUrl = $"https://www.euskalmet.euskadi.eus/vamet/stations/readings/{stationId}/{year}/{month}/{day}/readingsData.json";
            Debug.WriteLine(requestUrl);
            var client = new HttpClient { BaseAddress = new Uri(requestUrl) };
            var responseMessage = await client.GetAsync("", HttpCompletionOption.ResponseContentRead);
            var resultData = await responseMessage.Content.ReadAsStringAsync();
            Debug.WriteLine(resultData);
            dynamic stationReadingsJson = JsonConvert.DeserializeObject(resultData);
            foreach (var obj in stationReadingsJson)
            {
                Debug.WriteLine(obj);
            }
        }

    }
}
