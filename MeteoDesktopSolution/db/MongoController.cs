using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MeteoDesktopSolution.db
{
    internal class MongoController
    {
        private MongoClient dbClient;
        private IMongoCollection<BsonDocument> dbCollection;
        private IMongoDatabase meteoDb;
        public MongoController()
        {
            dbClient = new MongoClient("mongodb://localhost:27017");
            meteoDb = dbClient.GetDatabase("meteoDesktop");
            dbCollection = meteoDb.GetCollection<BsonDocument>("readings");
        }

        public async void insertDocument(BsonDocument newDocument) {
            await dbCollection.InsertOneAsync(newDocument);
            printReadings();
        }

        public List<BsonDocument> getCollection() {
            List<BsonDocument> documents = dbCollection.Find(new BsonDocument()).ToList();
            return documents;
        }

        public void printReadings() {
            List<BsonDocument> documents = dbCollection.Find(new BsonDocument()).ToList();
            foreach (var obj in documents) {
                Debug.WriteLine(obj.ToString());
            }
        }
    }
}
