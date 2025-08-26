namespace GeneralPurpose.Domain.Constants;

public class CommonConstants
{
    public const char DefaultClientDelimiter = ' ';
    public const string CustomReplacementDynamicOrderBy = "__Custom$Replace$Expression__";
    public static readonly Guid SystemUserId = Guid.Empty;
    
    public const string ClientProcessIdentifierLogging = "general-purpose-api";
    public const string ClientMachineHeader = "x-client-machine";
}