using Serilog;
using Serilog.Events;

namespace GeneralPurpose.Api.Extensions;

public static class LoggerSinkConfigurationExtensions
{
    public static LoggerConfiguration WriteToSeq(this LoggerConfiguration config,
        IConfigurationSection configurationSection)
    {
        var seqSection = configurationSection.GetSection("Seq");
        var url = seqSection["path"];
        var apiKey = seqSection["apiKey"];
        var logLevel = seqSection["logLevel"];
        var logLevelEnum = string.IsNullOrEmpty(logLevel) ? LogEventLevel.Information :
            Enum.TryParse<LogEventLevel>(logLevel, out var configLogLevel) ? configLogLevel : LogEventLevel.Information; 

        if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(apiKey))
            return config;

        config = config.WriteTo.Seq(url, logLevelEnum, apiKey: apiKey);
        return config;
    }
}