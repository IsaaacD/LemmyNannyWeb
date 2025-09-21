using LemmyWeb.Hubs;
using Microsoft.AspNetCore.HttpLogging;

namespace LemmyWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddApplicationInsightsTelemetry();
            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddSignalR();
            builder.Services.AddControllers();
            builder.Services.AddMemoryCache();

            builder.Services.AddHttpLogging(o => { 
                o.LoggingFields = HttpLoggingFields.All;
                o.RequestBodyLogLimit = 4096;
            });
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseHttpLogging();
            app.UseAuthorization();
            app.MapControllers();
            app.MapRazorPages();
            app.MapHub<ProcessedHub>("/processed");
            app.Run();
        }
    }
}
