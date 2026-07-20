using Application.Interfaces.Repository.Base;
using Domain.Abstraction;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace Infrastructure.Repository.Base
{
    public class MongoGenericRepository<T> : IMongoGenericRepository<T>
        where T : class, ISnapshot
    {
        #region Attributes
        private readonly IMongoCollection<T> collection;
        #endregion

        #region Properties
        #endregion

        public MongoGenericRepository(
            NonRelationalDB context)
        {
            var collectionName = typeof(T).Name;
            collection = context.GetCollection<T>(collectionName);
        }

        #region Methods
        public async Task<T?> GetByIdAsync(
            string id)
        {
            var filter = Builders<T>.Filter.Eq(x => x.ID, id);
            return await collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await collection.Find(_ => true).ToListAsync();
        }

        public async Task AddAsync(
            T entity)
        {
            await collection.InsertOneAsync(entity);
        }

        public async Task UpdateAsync(
            T entity)
        {
            var filter = Builders<T>.Filter.Eq(x => x.ID, entity.ID);

            await collection.ReplaceOneAsync(
                filter,
                entity,
                new ReplaceOptions
                {
                    IsUpsert = true
                });
        }

        public async Task UpdateManyAsync(
            IEnumerable<T> entities)
        {
            var models = entities.Select(entity =>
            {
                var filter =
                    Builders<T>.Filter.Eq(x => x.ID, entity.ID);

                return new ReplaceOneModel<T>(filter, entity)
                {
                    IsUpsert = true
                };
            });

            await collection.BulkWriteAsync(models);
        }

        public async Task DeleteAsync(
            string id)
        {
            var filter = Builders<T>.Filter.Eq(x => x.ID, id);
            await collection.DeleteOneAsync(filter);
        }
        #endregion
    }
}