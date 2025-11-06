using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;

namespace YPLCalibrationFromRheometer.Service.Mcp;

/// <summary>
/// Adapts the legacy <see cref="IMcpTool"/> abstraction to the ModelContextProtocol server tool contract.
/// </summary>
internal sealed class LegacyMcpServerToolAdapter : McpServerTool
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    private readonly IMcpTool _tool;
    private readonly ILogger _logger;
    private readonly Tool _protocolTool;
    private readonly IReadOnlyList<object> _metadata = Array.Empty<object>();

    public LegacyMcpServerToolAdapter(IMcpTool tool, ILoggerFactory loggerFactory)
    {
        ArgumentNullException.ThrowIfNull(tool);
        ArgumentNullException.ThrowIfNull(loggerFactory);

        _tool = tool;
        _logger = loggerFactory.CreateLogger(tool.GetType());

        _protocolTool = new Tool
        {
            Name = tool.Name,
            Description = tool.Description
        };

        if (tool.InputSchema is JsonNode schemaNode)
        {
            _protocolTool.InputSchema = JsonSerializer.SerializeToElement(schemaNode, SerializerOptions);
        }
    }

    public override Tool ProtocolTool => _protocolTool;

    public override IReadOnlyList<object> Metadata => _metadata;

    public override async ValueTask<CallToolResult> InvokeAsync(
        RequestContext<CallToolRequestParams> request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var arguments = ConvertArguments(request.Params?.Arguments);

        try
        {
            var result = await _tool.InvokeAsync(arguments, cancellationToken).ConfigureAwait(false);

            return new CallToolResult
            {
                StructuredContent = result
            };
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "MCP tool {ToolName} failed while handling request.", _tool.Name);

            return new CallToolResult
            {
                IsError = true,
                Content =
                {
                    new TextContentBlock
                    {
                        Text = $"Tool '{_tool.Name}' failed: {ex.Message}"
                    }
                }
            };
        }
    }

    private JsonObject? ConvertArguments(IReadOnlyDictionary<string, JsonElement>? arguments)
    {
        if (arguments is null || arguments.Count == 0)
        {
            return null;
        }

        var result = new JsonObject();

        foreach (var (key, element) in arguments)
        {
            try
            {
                result[key] = JsonNode.Parse(element.GetRawText());
            }
            catch (JsonException jsonEx)
            {
                _logger.LogWarning(jsonEx, "Failed to parse argument '{ArgumentKey}' for tool {ToolName}. Passing raw JSON text.", key, _tool.Name);
                result[key] = JsonValue.Create(element.GetRawText());
            }
        }

        return result;
    }
}
