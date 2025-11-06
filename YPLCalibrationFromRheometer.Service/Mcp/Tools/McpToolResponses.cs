using System.Text.Json.Nodes;

namespace YPLCalibrationFromRheometer.Service.Mcp.Tools;

internal static class McpToolResponses
{
    public static JsonNode CreateValidationError(string message)
    {
        return new JsonObject
        {
            ["status"] = 400,
            ["error"] = message
        };
    }

}
