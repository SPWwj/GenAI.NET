using System.Text.Json;

namespace GenerativeCS.Utilities;

internal static class FunctionInvoker
{
    private static JsonSerializerOptions JsonOptions { get; } = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };

    internal static async Task<object> InvokeAsync(Delegate function, JsonElement arguments, CancellationToken cancellationToken = default)
    {
        var parsedArguments = new List<object?>();
        foreach (var parameter in function.Method.GetParameters())
        {
            if (parameter.ParameterType == typeof(CancellationToken))
            {
                parsedArguments.Add(cancellationToken);
                continue;
            }

            if (arguments.TryGetProperty(parameter.Name!.ToSnakeCase(), out var argument))
            {
                try
                {
                    var argumentValue = JsonSerializer.Deserialize(argument.GetRawText(), parameter.ParameterType, JsonOptions);
                    parsedArguments.Add(argumentValue);
                }
                catch
                {
                    return new { IsSuccess = false, Error = "Argument does not match parameter type.", Parameter = parameter.Name!.ToSnakeCase(), Type = parameter.ParameterType };
                }
            }
            else if (parameter.IsOptional && parameter.DefaultValue != null)
            {
                parsedArguments.Add(parameter.DefaultValue);
            }
            else
            {
                return new { IsSuccess = false, Error = "Value is missing for required parameter.", Parameter = parameter.Name!.ToSnakeCase() };
            }
        }

        var invocationResult = function.DynamicInvoke([.. parsedArguments]);
        if (invocationResult is Task task)
        {
            await task.ConfigureAwait(false);

            var taskResultProperty = task.GetType().GetProperty("Result");
            if (taskResultProperty != null)
            {
                invocationResult = taskResultProperty.GetValue(task);
            }
        }

        if (invocationResult == null)
        {
            return new { IsSuccess = true };
        }

        return invocationResult;
    }
}