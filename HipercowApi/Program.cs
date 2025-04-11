// Copyright (c) Imperial College London. All rights reserved.
namespace HipercowApi
{
    using System.Diagnostics.CodeAnalysis;
    using HipercowApi.Tools;
    using Microsoft.Hpc.Scheduler;

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

            // Add services to the container.
            builder.Services.AddControllers();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddSingleton<IClusterInfoQuery, ClusterInfoQuery>();
            builder.Services.AddSingleton<IClusterLoadQuery, ClusterLoadQuery>();
            builder.Services.AddSingleton<IClusterHandleCache, ClusterHandleCache>();
            builder.Services.AddSingleton<IUtils, Utils>();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
