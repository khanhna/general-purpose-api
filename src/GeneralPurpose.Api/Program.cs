using System.Text.Json.Serialization;
using GeneralPurpose.Api.Converters;
using GeneralPurpose.Api.Extensions;
using GeneralPurpose.Api.Filters;
using GeneralPurpose.Application.Commands.FunStudio;
using GeneralPurpose.Infrastructure.Utilities;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Exceptions;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;

const string corsPolicyName = "simplePolicy";

var configurationRoot = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
    .AddJsonFile("appsettings.Production.json", optional: true, reloadOnChange: true)
    .Build();

var isDevEnv = ApplicationUtils.IsDevelopmentEnvironment(configurationRoot["Environment"]);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configurationRoot)
    .Enrich.WithExceptionDetails()
    .WriteToSeq(configurationRoot.GetSection("AppLogging"))
#if DEBUG
    .WriteTo.Console()
#endif
    .CreateBootstrapLogger();

Log.Information("--> Starting up application!");

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseDefaultServiceProvider(opts => opts.ValidateOnBuild = true).UseSerilog((ctx, lc) =>
{
    lc.Enrich.FromLogContext()
        .Enrich.WithExceptionDetails();
    
    if(isDevEnv)
        lc.WriteTo.Console();
    
    lc.ReadFrom.Configuration(ctx.Configuration);
});

// Add services to the container.
builder.Services.AddOptions()
    .AddHttpContextAccessor()
    //.AddValidatorsFromAssembly(typeof(SkinRetouchRequestValidation).Assembly)
    .AddFluentValidationAutoValidation()
    .AddApplicationDbContext(configurationRoot.GetConnectionString("ImageProcessDb")!, isDevEnv)
    .AddRepositories()
    .AddApplicationServices()
    .AddMediator(x =>
    {
        x.Assemblies = [typeof(SyncTransactionsCommand)];
        x.ServiceLifetime = ServiceLifetime.Transient;
    })
    .AddApplicationCors(corsPolicyName);

builder.Services.AddControllers(o => o.Filters.Add<ExceptionsFilter>())
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.Converters.Add(new DateTimeConverter());
        o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
if (isDevEnv)
{
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "GeneralPurpose.Api", Version = "v1" });
        c.DescribeAllParametersInCamelCase();
    });
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (isDevEnv)
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "GeneralPurpose.Api v1"));
}

app.MapControllers();

try
{
    Log.Information("Starting host");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}