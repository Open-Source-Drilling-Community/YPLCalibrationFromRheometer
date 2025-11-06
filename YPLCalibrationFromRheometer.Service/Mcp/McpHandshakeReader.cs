using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Http;

namespace YPLCalibrationFromRheometer.Service.Mcp;

internal static class McpHandshakeReader
{
    private const string DefaultProtocolVersion = "0.1";

    public static McpHandshake FromHttpRequest(HttpRequest request)
    {
        var protocolVersion = Extract(request, "protocolVersion", "X-MCP-Protocol-Version") ?? DefaultProtocolVersion;
        var clientName = Extract(request, "client", "X-MCP-Client-Name");
        var clientVersion = Extract(request, "clientVersion", "X-MCP-Client-Version");
        var sessionId = Extract(request, "sessionId", "X-MCP-Session-Id");

        JsonObject? capabilities = null;
        if (request.Headers.TryGetValue("X-MCP-Capabilities", out var capabilityHeader))
        {
            var headerValue = capabilityHeader.FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(headerValue))
            {
                try
                {
                    capabilities = JsonNode.Parse(headerValue) as JsonObject;
                }
                catch (JsonException)
                {
                    // ignore invalid capability payloads; the server will proceed without them
                }
            }
        }

        return new McpHandshake(protocolVersion, clientName, clientVersion, sessionId, capabilities);
    }

    private static string? Extract(HttpRequest request, string queryKey, string headerKey)
    {
        if (request.Query.TryGetValue(queryKey, out var queryValues))
        {
            var queryValue = queryValues.FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(queryValue))
            {
                return queryValue;
            }
        }

        if (request.Headers.TryGetValue(headerKey, out var headerValues))
        {
            var headerValue = headerValues.FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(headerValue))
            {
                return headerValue;
            }
        }

        return null;
    }
}
