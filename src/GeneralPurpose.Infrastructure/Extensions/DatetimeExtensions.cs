namespace GeneralPurpose.Infrastructure.Extensions;

public static class DatetimeExtensions
{
    private static readonly DateTime UnixEpoch = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    
    public static long ToUnixTimeStamp(this DateTime dateTime)
    {
        var utcDateTime = dateTime.Kind == DateTimeKind.Utc ? dateTime : dateTime.ToUniversalTime();
        return (long)(utcDateTime - UnixEpoch).TotalSeconds;
    }
}