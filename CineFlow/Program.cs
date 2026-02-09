namespace CineFlow
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configure Services
            ConfigureServices(builder);

            var app = builder.Build();

            // Configure Pipeline
            ConfigurePipeline(app);

            // Initialize Database
            await InitializeDatabaseAsync(app);

            app.Run();
        }

        private static void ConfigureServices(WebApplicationBuilder builder)
        {
            // Basic Services
            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();

            // Logging Configuration
            ConfigureLogging(builder);

            // Database Configuration
            ConfigureDatabase(builder);

            // Application Services
            ConfigureApplicationServices(builder);

            // Authentication
            //ConfigureAuthentication(builder);
            ConfigureAuthorization(builder);

            // Session & Cache
            ConfigureSessionAndCache(builder);

            // Security
            ConfigureSecurity(builder);
        }

        private static void ConfigureLogging(WebApplicationBuilder builder)
        {
            builder.Services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddConsole();
                loggingBuilder.AddDebug();
                loggingBuilder.AddFilter("Microsoft.AspNetCore.Antiforgery", LogLevel.Warning);
            });
        }

        private static void ConfigureDatabase(WebApplicationBuilder builder)
        {
            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });
        }

        private static void ConfigureApplicationServices(WebApplicationBuilder builder)
        {
            // HTTP Context
            builder.Services.AddHttpContextAccessor();

            // Custom Services
            builder.Services.AddScoped<IMovieCategoryService, MovieCategoryService>();
            builder.Services.AddScoped<ICinemaService, CinemaService>();
            builder.Services.AddScoped<IActorService, ActorService>();
            builder.Services.AddScoped<IMovieService, MovieService>();
            builder.Services.AddScoped<IBookingService, BookingService>();

            builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

            builder.Services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[] { "en", "ar-EG" };
                options.SetDefaultCulture(supportedCultures[0]);
                options.AddSupportedCultures(supportedCultures);
                options.AddSupportedUICultures(supportedCultures);
            });

            // MVC with Filters
            //builder.Services.AddMvc(options =>
            //{
            //    options.Filters.AddService<RequirePasswordChangeFilter>();
            //});
        }
        private static void ConfigureAuthorization(WebApplicationBuilder builder)
        {
            //builder.Services.AddAuthorizationPolicies();
        }
        private static void ConfigureSessionAndCache(WebApplicationBuilder builder)
        {
            // Session Configuration
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(15);
                options.Cookie.Name = "InfoInfrastructure.Session";
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.IsEssential = true;
            });

            // Memory Cache
            builder.Services.AddMemoryCache();
        }

        private static void ConfigureSecurity(WebApplicationBuilder builder)
        {
            // Antiforgery Configuration
            builder.Services.AddAntiforgery(options =>
            {
                options.HeaderName = "X-CSRF-TOKEN";
                options.Cookie.Name = "InfoInfrastructure.Antiforgery";
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            });
        }

        private static void ConfigurePipeline(WebApplication app)
        {
            // Exception Handling
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler(errorApp =>
                {
                    errorApp.Run(async context =>
                    {
                        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                        var exception = exceptionHandlerPathFeature?.Error;

                        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
                        logger.LogError(exception, "Unhandled exception occurred");

                        context.Response.Redirect($"/Error?message={Uri.EscapeDataString(exception?.Message ?? "Unknown error")}");
                    });
                });
                app.UseHsts();
            }

            app.Use(async (context, next) =>
            {
                context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
                context.Response.Headers.Append("X-Frame-Options", "SAMEORIGIN");
                context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
                context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");

                await next();
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            var localizationOptions = app.Services.GetService<IOptions<RequestLocalizationOptions>>().Value;
            app.UseRequestLocalization(localizationOptions);

            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();

            app.MapControllerRoute(
                name: "default",
                pattern: "{area=Dashboard}/{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();
        }

        private static async Task InitializeDatabaseAsync(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;

            try
            {

                // Ensure Database is created
                var context = services.GetRequiredService<AppDbContext>();
                await context.Database.EnsureCreatedAsync();

                // Seed Roles
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                //var roles = new[]
                //{

                //}

                //foreach (var role in roles)
                //{
                //    if (!await roleManager.RoleExistsAsync(role))
                //    {
                //        await roleManager.CreateAsync(new IdentityRole(role));
                //    }
                //}

                ////Seed database
                //AppDbInitializer.Seed(app);
                //AppDbInitializer.SeedUsersAndRolesAsync(app).Wait();
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred while initializing the database.");

            }
        }
    }
}
