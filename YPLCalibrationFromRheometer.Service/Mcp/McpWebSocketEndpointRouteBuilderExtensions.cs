using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ModelContextProtocol.AspNetCore;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;

namespace YPLCalibrationFromRheometer.Service.Mcp;

/// <summary>
/// Adds WebSocket transport support alongside the standard HTTP endpoints provided by ModelContextProtocol.AspNetCore.
/// </summary>
public static class McpWebSocketEndpointRouteBuilderExtensions
{
    public static IEndpointConventionBuilder MapMcpWebSocket(this IEndpointRouteBuilder endpoints, string pattern = "/mcp/ws")
    {
        var route = endpoints.MapGet(pattern, HandleWebSocketAsync);
        route.WithName("McpWebSocket");
        return route;
    }

    private static async Task HandleWebSocketAsync(HttpContext context)
    {
        var loggerFactory = context.RequestServices.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("MCP.WebSocket");

        if (!context.WebSockets.IsWebSocketRequest)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsync("Expected a WebSocket request.");
            return;
        }

        var httpOptions = context.RequestServices.GetService<IOptions<HttpServerTransportOptions>>();
        if (httpOptions?.Value.Stateless == true)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsync("WebSocket transport is not available when the MCP server is configured for stateless operation.");
            return;
        }

        var optionsFactory = context.RequestServices.GetRequiredService<IOptionsFactory<McpServerOptions>>();
        var serverOptions = optionsFactory.Create(Options.DefaultName);

        var cancellationToken = context.RequestAborted;

        if (httpOptions?.Value.ConfigureSessionOptions is { } configureSessionOptions)
        {
            await configureSessionOptions(context, serverOptions, cancellationToken);
        }

        var handshake = McpHandshakeReader.FromHttpRequest(context.Request);

        if (!string.IsNullOrWhiteSpace(handshake.ClientName) && !string.IsNullOrWhiteSpace(handshake.ClientVersion))
        {
            serverOptions.KnownClientInfo = new Implementation
            {
                Name = handshake.ClientName!,
                Version = handshake.ClientVersion!
            };
        }

        WebSocket webSocket;
        try
        {
            webSocket = await context.WebSockets.AcceptWebSocketAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to accept MCP WebSocket connection.");
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            return;
        }

        await using var transport = new WebSocketServerTransport(webSocket, "websocket", loggerFactory, handshake.SessionId);

        try
        {
            await using var server = McpServer.Create(transport, serverOptions, loggerFactory, context.RequestServices);
            context.Features.Set(server);

            if (httpOptions?.Value.RunSessionHandler is { } runSessionHandler)
            {
                await runSessionHandler(context, server, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                await server.RunAsync(cancellationToken).ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            logger.LogDebug("MCP WebSocket session aborted.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled error while running MCP WebSocket session.");
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        }
        finally
        {
            context.Features.Set<McpServer?>(null);
        }
    }
}
