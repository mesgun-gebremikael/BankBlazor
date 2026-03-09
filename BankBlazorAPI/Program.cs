using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BankBlazorAPI.Models;
using BankBlazorAPI.BankModels;

namespace BankBlazorAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Adda services to the container.
            builder.Services.AddControllers();
            builder.Services.AddHttpClient();

            builder.Services.AddDbContext<BankBlazorContext>(options =>
     options.UseSqlServer(builder.Configuration.GetConnectionString("BankBlazor")));

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("DevCors", policy =>
                {
                    policy
                        .AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseCors("DevCors");

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}