using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace YPLCalibrationFromRheometer.Service.Mcp;

internal static class McpActionResultConverter
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    public static JsonObject FromActionResult<T>(ActionResult<T> actionResult)
    {
        return Build(actionResult.Result, actionResult.Value);
    }

    public static JsonObject FromActionResult(ActionResult actionResult)
    {
        return Build(actionResult, null);
    }

    private static JsonObject Build(IActionResult? result, object? value)
    {
        var response = new JsonObject();
        var (status, payload) = Extract(result, value);

        response["status"] = status;

        if (payload is not null)
        {
            response["data"] = payload switch
            {
                JsonNode node => node.DeepClone(),
                _ => JsonSerializer.SerializeToNode(payload, payload.GetType(), SerializerOptions)
            };
        }

        return response;
    }

    private static (int Status, object? Payload) Extract(IActionResult? result, object? value)
    {
        if (result is ObjectResult objectResult)
        {
            var status = objectResult.StatusCode ?? StatusCodes.Status200OK;
            var payload = objectResult.Value ?? value;
            return (status, payload);
        }

        if (result is StatusCodeResult statusCodeResult)
        {
            return (statusCodeResult.StatusCode, value);
        }

        if (result is null)
        {
            var status = value is null ? StatusCodes.Status204NoContent : StatusCodes.Status200OK;
            return (status, value);
        }

        switch (result)
        {
            case EmptyResult:
                return (StatusCodes.Status204NoContent, value);
            default:
                if (result is StatusCodeResult genericStatus)
                {
                    return (genericStatus.StatusCode, value);
                }

                return (StatusCodes.Status200OK, value);
        }
    }
}
