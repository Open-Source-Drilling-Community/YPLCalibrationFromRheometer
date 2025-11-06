using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;

namespace YPLCalibrationFromRheometer.Service.Mcp;

internal static class McpServiceCollectionExtensions
{
    public static IServiceCollection AddLegacyMcpTool<TTool>(this IServiceCollection services)
        where TTool : class, IMcpTool
    {
        services.AddSingleton<TTool>();
        services.AddSingleton<IMcpTool>(sp => sp.GetRequiredService<TTool>());
        services.AddSingleton<McpServerTool>(sp =>
        {
            var tool = sp.GetRequiredService<TTool>();
            var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
            return new LegacyMcpServerToolAdapter(tool, loggerFactory);
        });

        return services;
    }
}
