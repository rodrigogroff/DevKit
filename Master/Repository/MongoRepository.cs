using Entities.Database;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Master.Repository
{
    #region - sample -
    /*
    public interface IMongoRepository
    {
        User GetUserByCPF(IMongoDatabase database, string cpf);

        List<User> GetUsersByCNPJ(IMongoDatabase database, string cnpj);

        #region - generic functions - 

        void CreateColletion(IMongoDatabase database, string collectionName);
        void InsertRecord<T>(IMongoDatabase database, string collectionName, T record);
        List<T> LoadRecords<T>(IMongoDatabase database, string collectionName);
        IMongoCollection<T> GetCollection<T>(IMongoDatabase database, string collectionName);
        void UpsertRecord<T>(IMongoDatabase database, string collectionName, Guid id, T record);
        void DeleteRecord<T>(IMongoDatabase database, string collectionName, Guid id);

        #endregion
    }

    public class MongoRepository : IMongoRepository
    {
        public User GetUserByCPF(IMongoDatabase database, string cpf)
        {
            var userCollection = GetCollection<User>(database, "user");
            var filter = Builders<User>.Filter.Eq("cpf", cpf);

            return userCollection.Find(filter).FirstOrDefault();
        }

        public List<User> GetUsersByCNPJ(IMongoDatabase database, string cnpj)
        {
            var userCollection = GetCollection<User>(database, "user");
            var filter = Builders<User>.Filter.Eq("customerCNPJ", cnpj);

            return userCollection.Find(filter).ToList();
        }

        #region - generic functions - 

        public void CreateColletion(IMongoDatabase database, string collectionName)
        {
            database.CreateCollection(collectionName);
        }

        public void InsertRecord<T>(IMongoDatabase database, string collectionName, T record)
        {
            var collection = database.GetCollection<T>(collectionName);

            collection.InsertOne(record);
        }

        public List<T> LoadRecords<T>(IMongoDatabase database, string collectionName)
        {
            var collection = database.GetCollection<T>(collectionName);

            return collection.Find(new BsonDocument()).ToList();
        }

        public IMongoCollection<T> GetCollection<T>(IMongoDatabase database, string collectionName)
        {
            return database.GetCollection<T>(collectionName);
        }

        public void UpsertRecord<T>(IMongoDatabase database, string collectionName, Guid id, T record)
        {
            var collection = database.GetCollection<T>(collectionName);

            collection.ReplaceOne( new BsonDocument("_id", id), record, new UpdateOptions { IsUpsert = true });
        }

        public void DeleteRecord<T>(IMongoDatabase database, string collectionName, Guid id)
        {
            var collection = database.GetCollection<T>(collectionName);
            var filter = Builders<T>.Filter.Eq("Id", id);

            collection.DeleteOne(filter);
        }
       
        #endregion
    }
*/
    #endregion


}
