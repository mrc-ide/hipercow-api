// Copyright (c) Imperial College London. All rights reserved.
namespace HipercowApi
{
    using System.Diagnostics.CodeAnalysis;
    using HipercowApi.Tools;
    using Microsoft.OpenApi.Models;
    using Prometheus;

    /// <summary>
    /// Hipercow_api main class.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public partial class Program
    {
        /// <summary>
        /// HipercowApi Main method.
        /// </summary>
        /// <param name="args">Command-line arguments to main - not used.</param>
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllers();

            // Learn more about configuring Swagger/OpenAPI at
            // https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

            // Below just arranges the Swagger test so we can add
            // Bearer xxxxx for the JWT using the Authorize button.
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Hipercow API", Version = "v1" });

                // Add security definition for Bearer token
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    Description = "Enter JWT Bearer token",
                });

                // Apply the security requirement globally
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer",
                            },
                        },
                        new string[] { }
                    },
                });
            });
            builder.Services.AddSingleton<IClusterInfoQuery, ClusterInfoQuery>();
            builder.Services.AddSingleton<IClusterLoadQuery, ClusterLoadQuery>();
            builder.Services.AddSingleton<IJobListQuery, JobListQuery>();
            builder.Services.AddSingleton<IClusterHandleCache, ClusterHandleCache>();
            builder.Services.AddSingleton<ISchedulerFactory, SchedulerFactory>();
            builder.Services.AddHostedService<MetricsUpdateService>();
            builder.Services.AddSingleton<ILdapManager, LdapManager>();
            builder.Services.AddSingleton<JwtSupport>();
            var app = builder.Build();
            app.UseRouting();
            app.UseMiddleware<HipercowApi.Middleware.ExceptionHandlingMiddleware>();
            app.UseMiddleware<HipercowApi.Middleware.JwtMiddleware>();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.UseMetricServer();
            app.Run();
        }
    }
}
