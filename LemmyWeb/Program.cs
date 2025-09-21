using LemmyWeb.Controllers;
using LemmyWeb.Hubs;
using LemmyWeb.Models;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Extensions.Caching.Memory;

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
            //var scope = app.Services.CreateScope();
            //var memCache = scope.ServiceProvider.GetService<IMemoryCache>();
            //memCache.Set(BotWebhookController.POSTS_FROM_LEMMY, new List<Post> { new Post { data = new PostData { apId = "https://fake", body = "test" } } });
            //memCache.Set(BotWebhookController.COMMENTS_FROM_LEMMY, new List<Comment> { new Comment { data = new CommentData { apId = "https://fake", content = "test" } } });
            //scope.Dispose();
            app.UseRouting();
            app.UseHttpLogging();
            app.UseAuthorization();
            app.MapControllers();
            app.MapRazorPages();
            app.MapHub<ProcessedHub>("/processed");
            app.MapHub<LemmyNannyBotHub>("/lemmynanny");
            app.Run();
        }
    }
}
