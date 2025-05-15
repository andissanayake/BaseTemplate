using BaseTemplate.Application.Common.Interfaces;
using BaseTemplate.Infrastructure.Data;
using Serilog;
using Serilog.Events;

namespace BaseTemplate.API;
public class Program
{
    private const string CLIENT_POLYCY_KEY = "CLIENT_POLYCY_KEY";
    public static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}"
            )
            .WriteTo.File("Logs/log-.txt",
                rollingInterval: RollingInterval.Day,
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}",
                retainedFileCountLimit: 7)
            .CreateLogger();

        var builder = WebApplication.CreateBuilder(args);
        builder.Host.UseSerilog();

        builder.Services.AddApplication();
        builder.Services.AddInfrastructure(builder.Configuration);
        builder.Services.AddAPI(builder.Configuration);

        builder.Services.AddCors(options =>
        {
            options.AddPolicy(name: CLIENT_POLYCY_KEY, builder =>
            {
                builder.WithOrigins("http://localhost:5000")
                   .AllowAnyMethod()
                   .AllowAnyHeader();
            });
        });

        var app = builder.Build();
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            using var scope = app.Services.CreateScope();
            var databaseInitializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
            var dbFactory = scope.ServiceProvider.GetRequiredService<IDbConnectionFactory>();
            databaseInitializer.Migrate(dbFactory.CreateConnection(), @"C:\Users\Acer\source\test\BaseTemplate\src\Infrastructure\Data\Scripts");
        }

        app.UseHealthChecks("/health");
        //app.UseHttpsRedirection();
        app.UseCors(CLIENT_POLYCY_KEY);
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseDefaultFiles();
        app.UseStaticFiles();
        app.MapControllers();
        app.MapFallbackToFile("index.html");
        app.Run();
    }
}

