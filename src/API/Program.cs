namespace BaseTemplate.API;
public class Program
{
    private const string CLIENT_POLYCY_KEY = "CLIENT_POLYCY_KEY";
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

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
            app.UseCors(CLIENT_POLYCY_KEY);
        }

        app.UseHealthChecks("/health");
        //app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();

        // Serve static files from wwwroot
        app.UseDefaultFiles();
        app.UseStaticFiles();

        app.MapControllers();

        // Handle SPA routing - serve index.html for all unmatched routes
        app.MapFallbackToFile("index.html");

        //app.UseExceptionHandler(options => { });
        app.Run();
    }
}

