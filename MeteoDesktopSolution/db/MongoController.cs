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

        public void insertDocument(BsonDocument newDocument) {
            dbCollection.InsertOne(newDocument);
        }

        public List<BsonDocument> getCollection() {
            List<BsonDocument> documents = dbCollection.Find(new BsonDocument()).ToList();
            return documents;
        }
    }
}
