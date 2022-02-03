using MeteoDesktopSolution.Data;
using MeteoDesktopSolution.Model;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeteoDesktopSolution.db
{
    internal class MongoController
    {
        private MongoClient dbClient;
        private IMongoCollection<BsonDocument> readingCollection;
        private IMongoCollection<BsonDocument> stationCollection;
        private IMongoDatabase meteoDb;
        private static MongoController mongoController;
        private bool intervalUpdatingStarted = false;
        

        public MongoController()
        {
            dbClient = new MongoClient("mongodb://localhost:27017");
            meteoDb = dbClient.GetDatabase("meteoDesktop");
            readingCollection = meteoDb.GetCollection<BsonDocument>("readings");
            stationCollection = meteoDb.GetCollection<BsonDocument>("stations");
            //readingCollection.DeleteMany(Builders<BsonDocument>.Filter.Empty);
            //stationCollection.DeleteMany(Builders<BsonDocument>.Filter.Empty);
        }

        public static MongoController getMongoController() {
            if (mongoController == null) {
                mongoController = new MongoController();
            }
            return mongoController;
        }

        public async void insertReading(BsonDocument newDocument) {
            await readingCollection.InsertOneAsync(newDocument);
            printReadings();
        }

        public async void insertStations(List<Station> stationList) {
            List<BsonDocument> bsonList = new List<BsonDocument>();
            foreach (Station station in stationList) {
                BsonDocument bsonStation = new BsonDocument();
                bsonStation.Add("id", station.id);
                bsonStation.Add("name", station.name);
                bsonStation.Add("nameEus", station.nameEus);
                bsonStation.Add("municipality", station.municipality);
                bsonStation.Add("province", station.province);
                bsonStation.Add("altitude", station.altitude);
                bsonStation.Add("x", station.x);
                bsonStation.Add("y", station.y);
                bsonStation.Add("stationType", station.stationType);
                bsonList.Add(bsonStation);
                await stationCollection.ReplaceOneAsync(
                filter: new BsonDocument("_id", station.id),
                options: new ReplaceOptions { IsUpsert = true },
                replacement: bsonStation);
            }
            if (!intervalUpdatingStarted) {
                updateReadingsEvery10Mins(stationList);
                intervalUpdatingStarted = true;
            }
            printStations();
        }

        public async void updateReadingsEvery10Mins(List<Station> stationList) {
            Thread thr = new Thread(async () => {
                while (true)
                {
                    foreach (Station station in stationList)
                    {
                        try
                        {
                            await DataParser.getStationData(station.id, station.name);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine("Background thread failed gathering station last data, station: " + station.id + " - " + station.name);
                        }

                    }
                    Thread.Sleep((1000 * 60) * 5);
                }
            });
            thr.Name = "Mythread";
            thr.IsBackground = true;
            thr.Start();
        }

        public List<BsonDocument> getReadings() {
            List<BsonDocument> readings = readingCollection.Find(new BsonDocument()).ToList();
            return readings;
        }

        public List<BsonDocument> getStations() {
            List<BsonDocument> stations = stationCollection.Find(new BsonDocument()).ToList();
            return stations;
        }

        public Dictionary<String, String> getLastReading(String stationId) {
            Dictionary<String, String> stationDictionary = new Dictionary<String, String>();
            try {
                List<String> keyList = new List<string>();
                var filter = Builders<BsonDocument>.Filter.Eq("id", stationId);
                var sort = Builders<BsonDocument>.Sort.Descending("date");
                BsonDocument lastReading = readingCollection.Find(filter).Sort(sort).First();
                IEnumerable<string> names = lastReading.Names;
                foreach (String name in names)
                {
                    if (name != "_id" && name != "date" && name != "id")
                    {
                        String stringValue = lastReading.GetValue(name).ToString();
                        if (stringValue.Length > 5)
                        {
                            stringValue = stringValue.Substring(0, stringValue.IndexOf('.') + 3);
                        }
                        stationDictionary.Add(name, stringValue);
                    }
                }
            } catch (InvalidOperationException ex) {
                MessageBox.Show("No hay datos de la baliza en la base de datos");
            }
            return stationDictionary;
        }

        public void printReadings() {
            List<BsonDocument> documents = readingCollection.Find(new BsonDocument()).ToList();
            foreach (var obj in documents) {
                Debug.WriteLine(obj.ToString());
            }
        }

        public void printStations() {
            List<BsonDocument> stations = stationCollection.Find(new BsonDocument()).ToList();
            foreach (var obj in stations)
            {
                Debug.WriteLine(obj.ToString());
            }
        }
    }
}
