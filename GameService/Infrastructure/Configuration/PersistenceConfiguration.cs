using Contract.Enum.MetaDomain.Effect;
using Contract.Enum.MetaDomain.Item;
using Domain.DomainException;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using ResponseCode;

namespace Infrastructure.Configuration
{
    public static class PersistenceConfiguration
    {
        #region Attributes
        #endregion

        #region Properties
        #endregion

        #region Methods
        public static IServiceCollection AddPersistenceConfiguration(
            this IServiceCollection services)
        {
            // SQL SERVER
            var sqlConnection = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING");
            if (string.IsNullOrWhiteSpace(sqlConnection))
                throw new InternalException(
                    InfrastructureCode.PersistenceConfigurationCode.SqlConnectionStringMissing,
                    "Critical infrastructure configuration missing. Environment variable 'SQL_CONNECTION_STRING' was not found.");

            services.AddDbContext<RelationalDB>(options =>
                options.UseSqlServer(sqlConnection));

            // MONGODB
            BsonSerializer.RegisterSerializer(new EnumSerializer<EquipmentSlot>(BsonType.String));
            BsonSerializer.RegisterSerializer(new EnumSerializer<AttributeType>(BsonType.String));

            var mongoConnection = Environment.GetEnvironmentVariable("MONGO_CONNECTION_STRING");
            if (string.IsNullOrWhiteSpace(mongoConnection))
                throw new InternalException(
                    InfrastructureCode.PersistenceConfigurationCode.MongoConnectionStringMissing,
                    "Critical infrastructure configuration missing. Environment variable 'MONGO_CONNECTION_STRING' was not found.");

            var mongoDbName = Environment.GetEnvironmentVariable("MONGO_DATABASE_NAME");
            if (string.IsNullOrWhiteSpace(mongoDbName))
                throw new InternalException(
                    InfrastructureCode.PersistenceConfigurationCode.MongoDatabaseNameMissing,
                    "Critical infrastructure configuration missing. Environment variable 'MONGO_DATABASE_NAME' was not found.");

            services.AddSingleton<IMongoClient>(_ =>
                new MongoClient(mongoConnection));

            services.AddScoped<NonRelationalDB>(sp =>
            {
                var client = sp.GetRequiredService<IMongoClient>();
                var database = client.GetDatabase(mongoDbName);

                return new NonRelationalDB(database);
            });

            return services;
        }
        #endregion
    }
}