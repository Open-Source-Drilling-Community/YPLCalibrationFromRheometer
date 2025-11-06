using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace YPLCalibrationFromRheometer.Service.Mcp;

public interface IMcpTool
{
    string Name { get; }

    string Description { get; }

    JsonNode? InputSchema { get; }

    Task<JsonNode?> InvokeAsync(JsonObject? arguments, CancellationToken cancellationToken);
}
