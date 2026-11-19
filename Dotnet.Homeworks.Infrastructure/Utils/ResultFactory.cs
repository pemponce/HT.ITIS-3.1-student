using Dotnet.Homeworks.Shared.Dto;

namespace Dotnet.Homeworks.Infrastructure.Utils;

public static class ResultFactory
{
    public static TResponse CreateFailure<TResponse>(string error) where TResponse : Result
    {
        var responseType = typeof(TResponse);

        if (responseType == typeof(Result))
            return (TResponse)(object)new Result(false, error);

        if (responseType.IsGenericType && responseType.GetGenericTypeDefinition() == typeof(Result<>))
        {
            var valueType = responseType.GetGenericArguments()[0];
            var defaultValue = valueType.IsValueType ? Activator.CreateInstance(valueType) : null;

            return (TResponse)Activator.CreateInstance(responseType, defaultValue, false, error)!;
        }

        throw new NotSupportedException($"Неподдерживаемый тип результата {responseType.FullName}");
    }
}
