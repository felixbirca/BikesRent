using BikesRent.BusinessLogicLayer;
using BikesRent.DataAccessLayer;
using BikesRent.DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace BikesRent.Web;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();
        
        SQLitePCL.Batteries.Init();
        // dotnet ef migrations add InitialCreate --project ../BikesRent/DataAccessLayer.csproj --startup-project . --context BikesDbContext
        // dotnet ef database update 
        builder.Services.AddDbContext<BikesDbContext>(options =>
            options.UseLazyLoadingProxies().UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
        
        builder.Services.AddScoped<IEntityRepository, EntityRepository>();

        builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();
        builder.Services.AddScoped<IBikeService, BikeService>();
        builder.Services.AddScoped<IUserService, UserService>();
        
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();

        builder.Services.AddLogging();
        builder.Services.AddMemoryCache();
        builder.Services.AddSingleton<ICache, MemoryCacheService>();
        
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}