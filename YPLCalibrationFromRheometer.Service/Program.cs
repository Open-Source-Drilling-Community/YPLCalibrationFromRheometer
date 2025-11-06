using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using YPLCalibrationFromRheometer.Service.Mcp.Tools;
using ModelContextProtocol.Protocol;

namespace YPLCalibrationFromRheometer.Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = CreateHostBuilder(args);

            // MCP server registrations
            //var serverVersion = typeof(CouetteRheometerManager).Assembly.GetName().Version?.ToString() ?? "1.0.0";

            //builder.Services.AddMcpServer(options =>
            //{
            //    options.ServerInfo = new Implementation
            //    {
            //        Name = "UnitConversionService",
            //        Version = serverVersion
            //    };
            //    options.Capabilities = new ServerCapabilities
            //    {
            //        Tools = new ToolsCapability()
            //    };
            //}).WithHttpTransport();

            //builder.Services.AddLegacyMcpTool<PingMcpTool>();
            // end MCP server

            var app = builder.Build();

            //app.MapMcp("/mcp");
            //app.MapMcpWebSocket("/mcp/ws");

            app.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureLogging((context, logging) =>
            {
                logging.ClearProviders();
                logging.AddConfiguration(context.Configuration.GetSection("Logging"));
                logging.AddDebug();
                logging.AddConsole();
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
    }
}
