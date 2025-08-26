using GeneralPurpose.Infrastructure.Data;
using GeneralPurpose.Infrastructure.Persistence;
using GeneralPurpose.Infrastructure.Persistence.KeyTypedRepositories;
using GeneralPurpose.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace GeneralPurpose.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationDbContext(this IServiceCollection services, string connectionString,
        bool isDevEnv = false)
    {
        services.AddDbContext<ApplicationDbContext>(opts =>
        {
            opts.UseNpgsql(connectionString,
                sqlOpts => sqlOpts.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.GetName().Name));
            
            if(isDevEnv) opts.EnableSensitiveDataLogging();
                    
            opts.LogTo(Console.WriteLine);
        });

        return services;
    }
    
    public static IServiceCollection AddApplicationCors(this IServiceCollection services, string corsPolicyName)
    {
        services.AddCors(opt => opt.AddPolicy(corsPolicyName,
            corsPolicyBuilder => corsPolicyBuilder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));
        return services;
    }
    
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped(typeof(IRepository<,>), typeof(BaseRepository<,>));

        return services;
    }
    
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IIdentityService, IdentityService>();

        return services;
    }
}