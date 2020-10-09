using MongoDB.Bson;
using Data.Entity;
using System.Collections.Generic;
using System.Linq.Expressions;
using System;

namespace Data
{
    public interface IRepository<T>
    {
        void AddOrUpdate(T entity, ObjectId userId);
        T GetFirst(Expression<Func<T, bool>> query);
        T GetFirstById(ObjectId id);
        IEnumerable<T> GetList(Expression<Func<T, bool>> query);
        EntityFilter<T> GetList(Expression<Func<T, bool>> query, int page, int pageSize, Expression<Func<T, object>> shortField, bool isOrderByDesc);
        void Delete(ObjectId id, ObjectId userId);
        void HardDelete(ObjectId id);
    }
}
