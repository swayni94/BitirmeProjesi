using Data.Entity;
using MongoDB.Driver;
using MongoDB.Bson;
using System;
using System.Linq.Expressions;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Data
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        private IMongoCollection<T> collection;
        private ILogger<Repository<T>> _logger;

        public Repository(string connectionString, string dataBaseName, ILogger<Repository<T>> logger)
        {
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(dataBaseName);
            collection = database.GetCollection<T>(typeof(T).Name);
            _logger = logger;
        }

        public void AddOrUpdate(T entity, ObjectId userId)
        {
            try
            {
                if (entity.Id == ObjectId.Empty)
                {
                    entity.CreatedDateTime = DateTime.UtcNow;
                    entity.UpdatedDateTime = null;
                    entity.UpdatedUserId = null;
                    entity.CreatedUserId = userId;
                    entity.IsDeleted = false;
                    entity.IsActive = true;
                    collection.InsertOne(entity);
                }
                else
                {
                    entity.UpdatedUserId = userId;
                    entity.IsDeleted = false;
                    entity.UpdatedDateTime = DateTime.UtcNow;
                    collection.ReplaceOne(w => w.Id == entity.Id, entity);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public T GetFirst(Expression<Func<T, bool>> query)
        {
            try
            {
                return collection.Find(query).FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public IEnumerable<T> GetList(Expression<Func<T, bool>> query)
        {
            try
            {
                return collection.Find(query).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public EntityFilter<T> GetList(Expression<Func<T, bool>> query, int page, int pageSize, Expression<Func<T, object>> shortField, bool isOrderByDesc)
        {
            var retval = new EntityFilter<T> { Total = collection.Find(query).CountDocuments() };

            var finding = collection.Find(query).Skip(page * pageSize).Limit(page);
            finding = isOrderByDesc ? finding.SortByDescending(shortField) : finding.SortBy(shortField);
            if (retval.Total > 0)
            {
                retval.DataList = finding.ToList();
            }

            return retval;
        }

        public void HardDelete(ObjectId id)
        {
            try
            {
                collection.DeleteOne(Builders<T>.Filter.Eq(w => w.Id, id));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public void Delete(ObjectId id, ObjectId userId)
        {
            try
            {
                collection.UpdateOneAsync(Builders<T>.Filter.Eq(w => w.Id, id), Builders<T>.Update.Set(w => w.IsDeleted, true).Set(w => w.UpdatedUserId, userId).Set(w => w.UpdatedDateTime, DateTime.UtcNow));

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public T GetFirstById(ObjectId id)
        {
            try
            {
                return collection.Find(w => !w.IsDeleted && w.Id == id).FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
    }
}
