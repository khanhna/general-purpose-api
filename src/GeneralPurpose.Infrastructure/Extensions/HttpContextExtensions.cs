using GeneralPurpose.Domain.Constants;
using Microsoft.AspNetCore.Http;

namespace GeneralPurpose.Infrastructure.Extensions;

public static class HttpContextExtensions
{
    public static string GetClientMachineHeader(this HttpContext? httpContext)
    {
        if (httpContext?.Request == null)
        {
            return string.Empty;
        }

        var headerCorrelation = string.Empty;

        if (httpContext.Request.Headers.TryGetValue(CommonConstants.ClientMachineHeader, out var values))
            headerCorrelation = values.FirstOrDefault();

        return headerCorrelation ?? string.Empty;
    }
}