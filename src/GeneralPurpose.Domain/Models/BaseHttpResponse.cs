using System.Net;
using System.Text.Json.Serialization;

namespace GeneralPurpose.Domain.Models;

public abstract class BaseHttpResponse<TResponse>
{
    private static readonly long OneSecond = TimeSpan.FromSeconds(1.0).Ticks;

    [JsonIgnore]
    public HttpStatusCode StatusCode { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Message { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public TResponse? Data { get; set; }

    // No need for this API
    [JsonIgnore]
    public DateTime CurrentTime
    {
        get
        {
            var currentTime = DateTime.UtcNow;
            currentTime = currentTime.AddTicks(-(currentTime.Ticks % BaseHttpResponse<TResponse>.OneSecond));
            return currentTime;
        }
    }

    [JsonIgnore]
    public bool IsSuccessStatusCode => (int)StatusCode >= 200 && (int)StatusCode <= 299;

    protected BaseHttpResponse(HttpStatusCode statusCode, string? message)
    {
        StatusCode = statusCode;
        Message = string.IsNullOrEmpty(message) ? this.GetDefaultHttpMessage() : message;
    }

    protected BaseHttpResponse(TResponse? response, HttpStatusCode statusCode, string? message)
        : this(statusCode, message)
    {
        Data = response;
    }

    private string GetDefaultHttpMessage()
    {
        return StatusCode switch
        {
            HttpStatusCode.OK => "Success",
            HttpStatusCode.Created => "Created",
            _ => StatusCode.ToString()
        };
    }
}

public class HttpResponse : BaseHttpResponse<object>
{
    public HttpResponse(HttpStatusCode statusCode, string? message = null)
        : base(statusCode, message)
    {
    }

    public HttpResponse(object? data, HttpStatusCode statusCode = HttpStatusCode.OK, string? message = null)
        : base(data, statusCode, message)
    {
    }
}

public class HttpResponse<TResponse> : BaseHttpResponse<TResponse>
{
    public HttpResponse() : base(HttpStatusCode.OK, null)
    {
    }

    public HttpResponse(HttpStatusCode statusCode, string? message = null) : base(statusCode, message!)
    {
    }

    public HttpResponse(TResponse data, HttpStatusCode statusCode = HttpStatusCode.OK, string? message = null)
        : base(data, statusCode, message!)
    {
    }
}