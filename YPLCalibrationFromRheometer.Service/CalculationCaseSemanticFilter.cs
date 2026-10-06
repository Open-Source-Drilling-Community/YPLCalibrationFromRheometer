using System;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace YPLCalibrationFromRheometer.Service;

/// <summary>Publishes the reviewed 0.15 calculation-case vocabulary for the legacy YPL wire model.</summary>
public sealed class CalculationCaseSemanticFilter : ISchemaFilter, IOperationFilter
{
    private const string Case = "urn:osdc:semantic:calculation-case";

    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type.Name != "YPLCalibration") return;
        schema.Extensions["x-osdc-semantic"] = Metadata(Case);
        Annotate(schema, "RheogramInput", "urn:osdc:semantic:calculation-specification", "urn:osdc:semantic:calculation-input");
        foreach (string property in new[] { "YPLModelKelessidis", "YPLModelMullineux", "YPLModelLevenbergMarquardt" })
            Annotate(schema, property, "urn:osdc:semantic:calculation-result", "urn:osdc:semantic:server-derived-calculation-result");
    }

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (context.MethodInfo.DeclaringType?.Name != "YPLCalibrationsController") return;
        string role = context.MethodInfo.Name switch
        {
            "Post" => "urn:osdc:semantic:immediate-calculation-submission",
            "Put" => "urn:osdc:semantic:immediate-calculation-replacement",
            _ when context.MethodInfo.Name.StartsWith("Get", StringComparison.Ordinal) =>
                "urn:osdc:semantic:calculation-case-retrieval",
            _ => string.Empty
        };
        if (role.Length != 0) operation.Extensions["x-osdc-semantic"] = Metadata(Case, role);
    }

    private static void Annotate(OpenApiSchema schema, string property, string concept, string role)
    {
        if (schema.Properties.TryGetValue(property, out OpenApiSchema value))
        {
            if (value.Reference is { } reference)
            {
                value.Reference = null;
                value.AllOf.Add(new OpenApiSchema { Reference = reference });
            }
            value.Extensions["x-osdc-semantic"] = Metadata(concept, role);
        }
    }

    private static OpenApiObject Metadata(string concept, string role = null)
    {
        var value = new OpenApiObject
        {
            ["catalogue"] = new OpenApiString("urn:osdc:semantic-catalogue"),
            ["catalogueVersion"] = new OpenApiString("0.15.0"),
            ["concept"] = new OpenApiString(concept),
            ["curationStatus"] = new OpenApiString("Reviewed"),
            ["assertionSource"] = new OpenApiString("provider-binding-registry"),
            ["requiredContext"] = new OpenApiArray()
        };
        if (role != null) value["role"] = new OpenApiString(role);
        return value;
    }
}
