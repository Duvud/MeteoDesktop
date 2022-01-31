using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace MeteoDesktopSolution.db
{
    internal class MongoController
    {
        private MongoClient dbClient;
        private IMongoCollection<BsonDocument> dbCollection;
        private IMongoDatabase meteoDb;
        private static MongoController mongoController;
        public MongoController()
        {
            dbClient = new MongoClient("mongodb://localhost:27017");
            meteoDb = dbClient.GetDatabase("meteoDesktop");
            dbCollection = meteoDb.GetCollection<BsonDocument>("readings");
            //dbCollection.DeleteMany(Builders<BsonDocument>.Filter.Empty);
        }

        public static MongoController getMongoController() {
            if (mongoController == null) {
                mongoController = new MongoController();
            }
            return mongoController;
        }

        public async void insertDocument(BsonDocument newDocument) {
            await dbCollection.InsertOneAsync(newDocument);
            printReadings();
        }

        public List<BsonDocument> getCollection() {
            List<BsonDocument> documents = dbCollection.Find(new BsonDocument()).ToList();
            return documents;
        }

        public Dictionary<String, String> getLastReading(String stationId) {
            Dictionary<String, String> stationDictionary = new Dictionary<String, String>();
            try {
                List<String> keyList = new List<string>();
                var filter = Builders<BsonDocument>.Filter.Eq("id", stationId);
                var sort = Builders<BsonDocument>.Sort.Descending("date");
                BsonDocument lastReading = dbCollection.Find(filter).Sort(sort).First();
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
            List<BsonDocument> documents = dbCollection.Find(new BsonDocument()).ToList();
            foreach (var obj in documents) {
                Debug.WriteLine(obj.ToString());
            }
        }
    }
}
