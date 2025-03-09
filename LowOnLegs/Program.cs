
using LowOnLegs.API.Hubs;
using LowOnLegs.Core.Interfaces;
using LowOnLegs.Data;
using LowOnLegs.Data.Repositories;
using LowOnLegs.Data.Repositories.Interfaces;
using LowOnLegs.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace LowOnLegs
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });


            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                {
                    builder.WithOrigins("http://localhost:4200") // Adres Angulara
                           .AllowAnyMethod()
                           .AllowAnyHeader()
                           .AllowCredentials();
                });
            });
            builder.Services.AddSignalR();

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddSingleton<IMatchStateManager, MatchStateManager>();
            builder.Services.AddScoped<IMatchService, MatchService>();
            builder.Services.AddScoped<IPlayerService, PlayerService>();
            builder.Services.AddScoped<IPlayerRepository, PlayerRepository>();
            builder.Services.AddScoped<IMatchRepository, MatchRepository>();
           


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.UseCors("CorsPolicy");

            app.MapHub<ScoreboardHub>("/scoreboardhub");

            app.Run();
        }
    }
}
