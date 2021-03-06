using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using ProductManager.Api.Swagger;
using ProductManager.CrossCutting.Notification;
using ProductManager.MongoDB;
using Serilog;
using System;

namespace ProductManager.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                /*.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<CreateFriendCommandValidator>())*/;

            services.AddCors(options => options.AddPolicy("CorsPolicy", policy =>
            {
                policy
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod().Build();
            }));

            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .MinimumLevel.Information()
                .CreateLogger();

            services.AddScoped<INotificationContext, NotificationContext>();

            var assemblyPath = GetType().Assembly.Location;
            var assembly = AppDomain.CurrentDomain.Load("ProductManager.Domain");

            services.UseMongoDb(Configuration);
            services.AddSwagger(Configuration, assemblyPath);
            services.AddMediatR(assembly);
            services.AddAutoMapper(assembly);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            IdentityModelEventSource.ShowPII = true;

            app.UseCors("CorsPolicy");

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.ConfigureSwagger(Configuration);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}