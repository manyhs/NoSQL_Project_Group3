// Author: Mariana da Costa Reis & Cristhel Mae Solomon
using Microsoft.AspNetCore.Authentication.Cookies;
using MongoDB.Driver;
using NoSQL_Project.Database;
using NoSQL_Project.Demo;
using NoSQL_Project.Features;
using NoSQL_Project.Repositories;
using NoSQL_Project.Repositories.Interfaces;

namespace NoSQL_Project
{
    public class Program
    {
        public static void Main(string[] args)
        {
            DotNetEnv.Env.TraversePath().Load();

            var isDemo = args.Contains("--demo");
            var builder = WebApplication.CreateBuilder(args.Where(a => a != "--demo").ToArray());

            var connectionString = builder.Configuration["Mongo:ConnectionString"];
            var databaseName = builder.Configuration["Mongo:Database"];

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException("No connection string found. Did you create the .env file?");

            builder.Services.AddSingleton<IMongoClient>(new MongoClient(connectionString));
            builder.Services.AddScoped(sp => sp.GetRequiredService<IMongoClient>().GetDatabase(databaseName));

            builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            builder.Services.AddScoped<ITicketRepository, TicketRepository>();
            builder.Services.AddScoped<TicketTransferService>();
            builder.Services.AddScoped<TicketEscalationService>();

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.AccessDeniedPath = "/Account/AccessDenied";
                    options.ExpireTimeSpan = TimeSpan.FromDays(14);
                });

            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                MongoIndexes.CreateAll(scope.ServiceProvider.GetRequiredService<IMongoDatabase>());

                if (isDemo)
                {
                    new ConsoleMenu(
                        scope.ServiceProvider.GetRequiredService<IEmployeeRepository>(),
                        scope.ServiceProvider.GetRequiredService<ITicketRepository>()).Run();
                    return;
                }
            }

            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Dashboard}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
