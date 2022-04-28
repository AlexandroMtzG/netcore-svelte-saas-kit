using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Serilog;
using Serilog.Events;

namespace NetcoreSaas.WebApi
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            // GlobalConfiguration.Configuration
            //     .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
            //     .UseColouredConsoleLogProvider()
            //     .UseSimpleAssemblyNameTypeSerializer()
            //     .UseRecommendedSerializerSettings()
            //     .UsePostgreSqlStorage("Database=Hangfire.Sample; Integrated Security=True;", new PostgreSqlStorageOptions()
            //     {
            //         QueuePollInterval = TimeSpan.Zero,
            //     });
            
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .MinimumLevel.Debug()
                .WriteTo.Console(
                   LogEventLevel.Verbose,
                   "{NewLine}{Timestamp:HH:mm:ss} [{Level}] ({CorrelationToken}) {Message}{NewLine}{Exception}")
                   .CreateLogger();

            try
            {
                CreateWebHostBuilder(args).Build().Run();
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseSerilog()
                .UseStartup<Startup>();
    }
}
