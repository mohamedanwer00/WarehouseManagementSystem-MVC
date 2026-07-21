namespace WarehousePL.Web;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services
        builder.Services.AddDistributedMemoryCache();
        builder.Services.AddJsonLocalization();
        builder.Services.AddControllersWithViews()
                        .AddViewLocalization();

        // DbContext
        builder.Services.AddDbContext<WarehouseDbContext>(options =>
        {
            options.UseSqlServer(
                builder.Configuration.GetConnectionString("DefaultConnection"),
                sql => sql.MigrationsAssembly("WarehouseDAL"));
        });

        //mapester
        MappingConfig.RegisterMappings();
        // Identity
        builder.Services.AddIdentity<User, Role>(options =>
        {
            options.SignIn.RequireConfirmedAccount = false;
        })
        .AddEntityFrameworkStores<WarehouseDbContext>()
        .AddDefaultTokenProviders();

        // Repositories
        builder.Services.AddScoped<IGenericRepository<Category>, CategoryRepository>();
        builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
        builder.Services.AddScoped<IBranchRepository, BranchRepository>();
        builder.Services.AddScoped<IWarehouseRepository, WarehouseRepository>();
        builder.Services.AddScoped<ICashBoxRepository, CashBoxRepository>();
        builder.Services.AddScoped<IUnitRepository, UnitRepository>();
        builder.Services.AddScoped<IProductRepository, ProductRepository>();
        builder.Services.AddScoped<IProductUnitRepository, ProductUnitRepository>();
        builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
        builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
        builder.Services.AddScoped<IOpeningStockRepository, OpeningStockRepository>();
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

        var app = builder.Build();

        // إنشاء قاعدة البيانات وتطبيق الـ Migrations
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;

            var context = services.GetRequiredService<WarehouseDbContext>();

            var roleManager = services.GetRequiredService<RoleManager<Role>>();
            var userManager = services.GetRequiredService<UserManager<User>>();

            WarehouseDAL.Data.SeedData.DefalutRoles
                .SeedAsync(roleManager)
                .GetAwaiter()
                .GetResult();

            WarehouseDAL.Data.SeedData.DefalutUsers
                .SeedSuperAdminUserAsync(userManager, roleManager)
                .GetAwaiter()
                .GetResult();
        }

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }
        app.UseRequestLocalization(Localization.localizationOptions());

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapStaticAssets();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}")
            .WithStaticAssets();

        app.Run();
    }
}
