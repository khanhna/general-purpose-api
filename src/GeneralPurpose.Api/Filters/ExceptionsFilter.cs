using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GeneralPurpose.Api.Filters;

public class ExceptionsFilter(ILogger<ExceptionsFilter> logger) : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        var responseResult = new GeneralPurpose.Domain.Models.HttpResponse(HttpStatusCode.InternalServerError)
        {
            Message = context.Exception.Message,
        };
        context.Result = new ObjectResult(responseResult)
        {
            StatusCode = (int)responseResult.StatusCode
        };
        
        logger.LogError(context.Exception, "Http process encounter exception!");
        
        context.ExceptionHandled = true;
    }
}