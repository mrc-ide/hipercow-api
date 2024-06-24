// Copyright (c) Imperial College London. All rights reserved.
namespace Hipercow_api
{
    using System.Diagnostics.CodeAnalysis;
    using Hipercow_api.Tools;

    /// <summary>
    /// Hipercow_api main class.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public partial class Program
    {
        /// <summary>
        /// Hipercow_api Main method.
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

            ClusterHandleCache.InitialiseHandles(DideConstants.GetDideClusters());
            app.Run();
        }
    }
}
