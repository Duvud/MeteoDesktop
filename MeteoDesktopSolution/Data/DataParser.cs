using MeteoDesktopSolution.db;
using MeteoDesktopSolution.Model;
using MongoDB.Bson;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

        internal MongoController MongoController
        {
            get => default;
            set
            {
            }
        }

        public static async Task<Station[]> getStations() {
            MongoController mongoController = MongoController.getMongoController();
            var client = new HttpClient { BaseAddress = new Uri("https://www.euskalmet.euskadi.eus/vamet/stations/stationList/stationList.json") };
            var responseMessage = await client.GetAsync("",HttpCompletionOption.ResponseContentRead);
            var resultArray = await responseMessage.Content.ReadAsStringAsync();
            List<Station> stationList = JsonConvert.DeserializeObject<List<Station>>(resultArray);
             stationList.RemoveAll(delegate(Station station){
                return station.stationType != "METEOROLOGICAL";
            });
            mongoController.insertStations(stationList);
            return stationList.ToArray();
        }

        public static double getLastData(List<string> dataJsonTimeList, JObject dataJson) {
            dataJsonTimeList.Sort();
            double lastData = Double.Parse(dataJson[dataJsonTimeList[dataJsonTimeList.Count() - 1].ToString()].ToString());
            return lastData;
        }

        public static async Task<IDictionary<String, double>> getStationData(String stationId, String stationName) {
            MongoController dbController = MongoController.getMongoController();
            IDictionary<String, Double> lastReadingsMap = new Dictionary<String, Double>();
            DateTime localDate = DateTime.Now;
            String month = localDate.Month.ToString();
            String day = localDate.Day.ToString(); 
            if (month.Length == 1) {
                month = "0" + month;
            }
            if (day.Length == 1) {
                day = "0" + day;
            }
            String year = localDate.Year.ToString();
            String requestUrl = $"https://www.euskalmet.euskadi.eus/vamet/stations/readings/{stationId}/{year}/{month}/{day}/readingsData.json";
            Debug.WriteLine(requestUrl);
            var client = new HttpClient { BaseAddress = new Uri(requestUrl) };
            var responseMessage = await client.GetAsync("", HttpCompletionOption.ResponseContentRead);
            var resultData = await responseMessage.Content.ReadAsStringAsync();
            dynamic stationReadingsJson = JsonConvert.DeserializeObject(resultData);
            BsonDocument newReading = new BsonDocument {};
            newReading.Add("id", stationId);
            newReading.Add("date", localDate.ToString());
            foreach (var obj in stationReadingsJson)
            {
                foreach (JObject stationTypeJson in obj) {
                    String dataType = stationTypeJson["name"].ToString();
                    JObject preDataJson = JObject.Parse(stationTypeJson["data"].ToString());
                    IList<string> keys = preDataJson.Properties().Select(p => p.Name).ToList();
                    JObject dataJson = JObject.Parse(preDataJson[keys[0]].ToString());
                    List<string> dataJsonTimeList = dataJson.Properties().Select(p => p.Name).ToList();
                    dataJsonTimeList.Sort();
                    double lastData = getLastData(dataJsonTimeList, dataJson);
                    switch (dataType) {
                        case "temperature":
                            lastReadingsMap.Add("temperature", lastData);
                            newReading.Add("temperature", lastData);
                            break;
                        case "precipitation":
                            lastReadingsMap.Add("precipitation", lastData);
                            newReading.Add("precipitation", lastData);
                            break;
                        case "humidity":
                            lastReadingsMap.Add("humidity", lastData);
                            newReading.Add("humidity", lastData);
                            break;
                        case "mean_speed":
                            lastReadingsMap.Add("speed", lastData);
                            newReading.Add("speed", lastData);
                            break;
                    }
                }
            }
            dbController.insertReading(newReading);
            return lastReadingsMap;
        }
    }
}
