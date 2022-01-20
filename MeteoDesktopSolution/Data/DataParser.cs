﻿using MeteoDesktopSolution.Model;
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

    }
}
