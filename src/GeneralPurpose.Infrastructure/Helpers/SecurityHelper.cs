using System.Security.Cryptography;
using System.Text;
using GeneralPurpose.Infrastructure.Extensions;

namespace GeneralPurpose.Infrastructure.Helpers;

public static class SecurityHelper
{
    public static bool IsValidSignature(string targetString, string keyMaterial, DateTime currentTime) =>
        GenerateSignature(keyMaterial, currentTime) == targetString;

    public static string GenerateSignature(string keyMaterial, DateTime currentTime)
    {
        var encoding = new ASCIIEncoding();
        var keyBytes = encoding.GetBytes(FunStudioKeyGenerate(keyMaterial, currentTime));
        var textBytes = encoding.GetBytes(FunStudioPlainTextGenerate(keyMaterial, currentTime));

        using var hash = new HMACSHA256(keyBytes);
        var hashBytes = hash.ComputeHash(textBytes);
        return Convert.ToBase64String(hashBytes);
    }

    private static string FunStudioKeyGenerate(string keyMaterial, DateTime currentTime) =>
        $"{keyMaterial}${currentTime.ToUnixTimeStamp()}";
    
    private static string FunStudioPlainTextGenerate(string keyMaterial, DateTime currentTime) =>
        $"{currentTime.ToUnixTimeStamp()}!(*@{keyMaterial}";
}