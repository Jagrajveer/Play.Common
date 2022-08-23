using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Play.Common.Service.Repositories;
using Play.Common.Service.Entities;
using Play.Common.Service.Settings;

namespace Play.Common.Service.MongoDb;

public static class Extensions
{
    public static IServiceCollection AddMongo(this IServiceCollection serviceCollection)
    {
        BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
        BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));

        serviceCollection.AddSingleton(servicesProvider =>
        {
            var configuration = servicesProvider.GetService<IConfiguration>();
            Debug.Assert(configuration != null, nameof(configuration) + " != null");
            var serviceSettings = configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();
            
            MongoDbSettings mongoDbSettings = configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();
            MongoClient mongoClient = new MongoClient(mongoDbSettings.ConnectionString);

            return mongoClient.GetDatabase(serviceSettings.ServiceName);
        });

        return serviceCollection;
    }

    public static IServiceCollection AddMongoRepository<T>(this IServiceCollection serviceCollection,
        string collectionName) where T : IEntity
    {
        serviceCollection.AddSingleton<IRepository<T>>(serviceProvider =>
        {
            var database = serviceProvider.GetService<IMongoDatabase>();

            Debug.Assert(database != null, nameof(database) + " != null");
            return new MongoRepository<T>(database, collectionName);
        });

        return serviceCollection;
    } 
}