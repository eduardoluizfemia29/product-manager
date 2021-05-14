﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using ProductManager.Domain.Contracts;

namespace ProductManager.MongoDB
{
    public static class Bootstrapper
    {
        public static IServiceCollection UseMongoDb(this IServiceCollection services, IConfiguration configuration)
        {
            var mongoDbSettings = configuration.GetSection(nameof(MongoSettings)).Get<MongoSettings>();

            services.AddSingleton<IMongoClient>(new MongoClient(mongoDbSettings.ConnectionString));
            services.AddTransient(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            //services.AddTransient<IFriendRepository, FriendRepository>();
            services.AddSingleton(mongoDbSettings);

            return services;
        }
    }
}