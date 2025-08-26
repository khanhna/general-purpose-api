using System.Security.Claims;
using GeneralPurpose.Domain.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace GeneralPurpose.Infrastructure.Services;

public interface IIdentityService
{
    Guid GetRequiredUserIdentity();
    Guid GetUserIdentity();
}

public class IdentityService(IHttpContextAccessor context) : IIdentityService
{
    private readonly HttpContext? _context = context.HttpContext;
    
    public Guid GetRequiredUserIdentity()
    {
        if (!(_context?.User.Identity?.IsAuthenticated ?? false))
            throw new UnauthorizedAccessException("Not Authenticated");

        return GetUserIdentity();
    }

    public Guid GetUserIdentity()
    {
        if(!(_context?.User.Identity?.IsAuthenticated ?? false))
            return CommonConstants.SystemUserId;

        string? rawUserId;
        if (_context?.User.Identity?.AuthenticationType == IdentityConstants.ApplicationScheme)
            rawUserId = _context?.User.Identity?.IsAuthenticated ?? false
                ? _context.User.Identities.First(x => x.IsAuthenticated).Claims
                    .Single(x => x.Type == ClaimTypes.NameIdentifier).Value
                : null;
        else
            rawUserId = _context?.User.FindFirst("sub")?.Value;

        return Guid.TryParse(rawUserId, out var userId) && !Guid.Empty.Equals(userId) ? userId : CommonConstants.SystemUserId;
    }
}