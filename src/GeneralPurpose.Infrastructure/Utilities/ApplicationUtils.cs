namespace GeneralPurpose.Infrastructure.Utilities;

public static class ApplicationUtils
{
    public static bool IsDevelopmentEnvironment(string? env) => env == "dev";
}