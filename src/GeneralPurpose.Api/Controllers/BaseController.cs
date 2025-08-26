using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace GeneralPurpose.Api.Controllers;

[Controller]
public class BaseController : ControllerBase
{
    public override OkObjectResult Ok(object? value)
    {
        return base.Ok(new GeneralPurpose.Domain.Models.HttpResponse(value));
    }
    
    protected BadRequestObjectResult BadRequest(string message, object? error = null) =>
        base.BadRequest(new GeneralPurpose.Domain.Models.HttpResponse(error, HttpStatusCode.BadRequest, message));
    
    protected ObjectResult ObjectResult(HttpStatusCode statusCode, string? message, object? response = null) =>
        new(message) { StatusCode = (int)statusCode, Value = new GeneralPurpose.Domain.Models.HttpResponse(response, statusCode, message) };

    protected ObjectResult ForwardGeneralResponse(Domain.Models.HttpResponse response)
        => new(response) { StatusCode = (int)response.StatusCode };
    
    protected ObjectResult ForwardGeneralResponse<T>(Domain.Models.HttpResponse<T> response)
        => new(response) { StatusCode = (int)response.StatusCode };
}