using System;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ModelContextProtocol;
using ModelContextProtocol.Protocol;

namespace YPLCalibrationFromRheometer.Service.Mcp;

/// <summary>
/// Implements a Model Context Protocol <see cref="ITransport"/> over ASP.NET Core WebSockets.
/// </summary>
internal sealed class WebSocketServerTransport : TransportBase
{
    private const int DefaultBufferSize = 16 * 1024;

    private readonly WebSocket _socket;
    private readonly CancellationTokenSource _shutdownCts = new();
    private readonly SemaphoreSlim _sendLock = new(1, 1);
    private readonly ILogger<WebSocketServerTransport> _logger;
    private readonly Task _receiveLoop;

    public WebSocketServerTransport(WebSocket socket, string endpointName, ILoggerFactory? loggerFactory, string? sessionId)
        : base(endpointName, loggerFactory)
    {
        ArgumentNullException.ThrowIfNull(socket);

        _socket = socket;
        SessionId = sessionId;
        _logger = loggerFactory?.CreateLogger<WebSocketServerTransport>() ?? NullLogger<WebSocketServerTransport>.Instance;

        SetConnected();
        _receiveLoop = Task.Run(ReceiveMessagesAsync);
    }

    public override async Task SendMessageAsync(JsonRpcMessage message, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(message);

        if (!IsConnected)
        {
            throw new InvalidOperationException("Transport is not connected.");
        }

        var payload = JsonSerializer.SerializeToUtf8Bytes(message, McpJsonUtilities.DefaultOptions);

        await _sendLock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            await _socket.SendAsync(payload, WebSocketMessageType.Text, endOfMessage: true, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogError(ex, "Failed to send MCP message over WebSocket transport.");
            throw;
        }
        finally
        {
            _sendLock.Release();
        }
    }

    public override async ValueTask DisposeAsync()
    {
        try
        {
            _shutdownCts.Cancel();
        }
        catch (ObjectDisposedException)
        {
            // already disposed
        }

        await _receiveLoop.ConfigureAwait(false);

        if (_socket.State is WebSocketState.Open or WebSocketState.CloseReceived)
        {
            try
            {
                await _socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Server closing connection.", CancellationToken.None).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Failed to close MCP WebSocket gracefully.");
            }
        }

        _sendLock.Dispose();
        _shutdownCts.Dispose();
        _socket.Dispose();

        SetDisconnected();
    }

    private async Task ReceiveMessagesAsync()
    {
        var buffer = new byte[DefaultBufferSize];
        await using var payloadStream = new MemoryStream(DefaultBufferSize);

        try
        {
            while (!_shutdownCts.IsCancellationRequested && _socket.State == WebSocketState.Open)
            {
                var result = await _socket.ReceiveAsync(buffer, _shutdownCts.Token).ConfigureAwait(false);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    break;
                }

                payloadStream.Write(buffer, 0, result.Count);

                if (!result.EndOfMessage)
                {
                    continue;
                }

                var messageBytes = payloadStream.ToArray();
                payloadStream.SetLength(0);

                try
                {
                    var message = JsonSerializer.Deserialize<JsonRpcMessage>(messageBytes, McpJsonUtilities.DefaultOptions);
                    if (message is null)
                    {
                        _logger.LogWarning("Received a null MCP JSON-RPC message over WebSocket.");
                        continue;
                    }

                    await WriteMessageAsync(message, _shutdownCts.Token).ConfigureAwait(false);
                }
                catch (JsonException ex)
                {
                    var raw = Encoding.UTF8.GetString(messageBytes);
                    _logger.LogWarning(ex, "Failed to parse JSON-RPC message over WebSocket. Payload: {Payload}", raw);
                }
            }
        }
        catch (OperationCanceledException) when (_shutdownCts.IsCancellationRequested)
        {
            // expected during shutdown
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while reading from the MCP WebSocket transport.");
        }
        finally
        {
            SetDisconnected();
        }
    }
}
