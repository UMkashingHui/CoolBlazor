using CoolWebApi.Utils.Contexts;

namespace CoolWebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();

            // var host = CreateHostBuilder(args).Build();

            // using (var scope = host.Services.CreateScope())
            // {
            //     var services = scope.ServiceProvider;

            //     try
            //     {
            //         var context = services.GetRequiredService<CoolBlazorDbContext>();

            //         // if (context.Database.IsSqlServer())
            //         // {
            //         //     context.Database.Migrate();
            //         // }
            //     }
            //     catch (Exception ex)
            //     {
            //         var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

            //         logger.LogError(ex, "An error occurred while migrating or seeding the database.");

            //         throw;
            //     }
            // }

            // await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder
    (string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}