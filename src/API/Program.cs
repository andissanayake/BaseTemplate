using BaseTemplate.API.Infrastructure;
using BaseTemplate.API.Services;
using BaseTemplate.Application.Common.Interfaces;
using BaseTemplate.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;

namespace BaseTemplate.API;
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddApplication();
        builder.Services.AddInfrastructure(builder.Configuration);
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();
        builder.Services.AddSingleton<IUser, CurrentUserService>();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddExceptionHandler<CustomExceptionHandler>();
        builder.Services.AddHealthChecks()
            .AddDbContextCheck<ApplicationDbContext>();
        builder.Services.AddControllers();
        builder.Services.Configure<ApiBehaviorOptions>(options =>
            options.SuppressModelStateInvalidFilter = true);

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        app.UseHealthChecks("/health");
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.UseExceptionHandler(options => { });
        app.Run();
    }
}

