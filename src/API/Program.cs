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
                builder.WithOrigins("http://localhost:6001")
                   .AllowAnyMethod()
                   .AllowAnyHeader();
            });
        });

        var app = builder.Build();
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHealthChecks("/health");
        //app.UseHttpsRedirection();
        app.UseCors(CLIENT_POLYCY_KEY);
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.UseExceptionHandler(options => { });
        app.Run();
    }
}

