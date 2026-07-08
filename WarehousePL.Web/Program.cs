using Microsoft.Extensions.FileProviders;
using WarehouseDAL.Data.Contexts;
using WarehouseBLL.Mapping;
using AutoMapper;
using WarehouseDAL.Repositories.Interfaces;
using WarehouseDAL.Repositories.Implememtation;
using Microsoft.EntityFrameworkCore;
using WarehouseDAL.Entities;

namespace WarehousePL.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews().AddViewLocalization();

            // DbContext registration
            builder.Services.AddDbContext<WarehouseDbContext>((sp, options) =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            // AutoMapper - register mapper instance using the project's MappingProfile
            builder.Services.AddAutoMapper(X => X.AddProfile(new MappingProfile()));

            // Register repositories and UnitOfWork
            builder.Services.AddScoped<IGenericRepository<Category>, CategoryRepository>();
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            


            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
