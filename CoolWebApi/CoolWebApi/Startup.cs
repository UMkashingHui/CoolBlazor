using System.Security.Claims;
using AspNetCore.Identity.Mongo;
using BlazorHero.CleanArchitecture.Server.Extensions;
using Hangfire;
using Hangfire.Mongo;
using Hangfire.Mongo.Migration.Strategies;
using Hangfire.Mongo.Migration.Strategies.Backup;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Localization;
using MyBlazorApp.Server.Extensions;
using CoolWebApi.Config;
using CoolWebApi.Models.Identity;
using CoolWebApi.Utils.Extensions;
using CoolWebApi.Utils.Filters;

namespace CoolWebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private readonly IConfiguration _configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            var mongoConnection = _configuration.GetSection("MongoDbConfiguration").Get<MongoDbConfiguration>().ConnectionString;
            var migrationOptions = new MongoMigrationOptions
            {
                MigrationStrategy = new DropMongoMigrationStrategy(),
                BackupStrategy = new CollectionMongoBackupStrategy()
            };
            // Refer to CleanArchiteture

            // services.AddForwarding(_configuration);
            services.AddLocalization(options =>
            {
                options.ResourcesPath = "Resources";
            });
            services.AddCurrentUserService();
            // services.AddSerialization();
            services.AddDatabase(mongoConnection);
            // services.AddServerStorage(); //TODO - should implement ServerStorageProvider to work correctly!
            // services.AddScoped<ServerPreferenceManager>();
            services.AddServerLocalization();

            services.AddMongoDbIdentity(_configuration);
            // services.AddIdentity();

            services.AddJwtAuthentication(services.GetApplicationSettings(_configuration));
            // services.AddSignalR();
            services.AddApplicationLayer();
            services.AddApplicationServices();
            services.AddRepositories();
            // services.AddExtendedAttributesUnitOfWork();
            services.AddSharedInfrastructure(_configuration);
            services.RegisterSwagger();
            // services.AddInfrastructureMappings();
            // services.AddHangfire(x => x.UseSqlServerStorage(_configuration.GetConnectionString("DefaultConnection")));
            services.AddHangfire(config =>
                {
                    config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170);
                    config.UseSimpleAssemblyNameTypeSerializer();
                    config.UseRecommendedSerializerSettings();
                    config.UseMongoStorage(mongoConnection, "Hangfire", new MongoStorageOptions { MigrationOptions = migrationOptions });

                });
            services.AddHangfireServer();
            services.AddControllers().AddValidators();
            // services.AddExtendedAttributesValidators();
            // services.AddExtendedAttributesHandlers();
            // services.AddRazorPages();
            services.AddApiVersioning(config =>
            {
                config.DefaultApiVersion = new ApiVersion(1, 0);
                config.AssumeDefaultVersionWhenUnspecified = true;
                config.ReportApiVersions = true;
            });
            services.AddLazyCache();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();

            // Add functionality to inject IOptions<T>
            services.AddOptions();
            services.Configure<MongoDbConfiguration>(_configuration.GetSection("MongoDbConfiguration"));

            // AutoMapper
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IStringLocalizer<Startup> localizer)
        {
            app.UseForwarding(_configuration);
            app.UseSession();
            // app.UseExceptionHandling(env);
            app.UseHttpsRedirection();
            // app.UseMiddleware<ErrorHandlerMiddleware>();
            // app.UseBlazorFrameworkFiles(); // WebAssembley
            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"Files")),
                RequestPath = new PathString("/Files")
            });
            // app.UseRequestLocalizationByCulture();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseHangfireDashboard("/jobs", new DashboardOptions
            {
                DashboardTitle = localizer["CoolBlazor Jobs"],
                Authorization = new[] { new HangfireAuthorizationFilter() }
            });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello From ASP.NET Core Web API");
                });
                endpoints.MapGet("/Resource1", async context =>
                {
                    await context.Response.WriteAsync("Hello From ASP.NET Core Web API Resource1");
                });
                endpoints.MapControllerRoute(
                  name: "Admin",
                  pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllerRoute(
                  name: "default",
                  pattern: "{controller=Home}/{action=Index}/{id?}");
            });
            app.ConfigureSwagger();
            app.Initialize(_configuration);

            // Configure the HTTP request pipeline.
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }


            app.Use(async (context, next) =>
            {
                await next.Invoke();
                var data = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            });

            app.Initialize(_configuration);

        }


    }
}

