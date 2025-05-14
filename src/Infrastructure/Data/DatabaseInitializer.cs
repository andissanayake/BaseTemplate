using System.Data;
using Dapper;
using Microsoft.Extensions.Logging;
namespace BaseTemplate.Infrastructure.Data;
public class DatabaseInitializer
{
    private readonly ILogger<DatabaseInitializer> _logger;
    public DatabaseInitializer(ILogger<DatabaseInitializer> logger)
    {
        _logger = logger;
    }

    public void Migrate(IDbConnection connection, string scriptsFolder)
    {
        connection.Open();

        _logger.LogInformation("Starting database migrations...");

        // Ensure the migration history table exists
        connection.Execute(@"
                CREATE TABLE IF NOT EXISTS migration_history (
                    id SERIAL PRIMARY KEY,
                    script_name VARCHAR(255) NOT NULL,
                    executed_at TIMESTAMPTZ DEFAULT NOW()
                );
            ");

        if (!Directory.Exists(scriptsFolder))
        {
            _logger.LogWarning("No migration scripts found in the Scripts folder.");
            return;
        }

        // 🔄 Fetch all executed scripts ONCE (no repetitive querying)
        var executedScripts = connection.Query<string>(
            "SELECT script_name FROM migration_history"
        ).ToHashSet();

        var sqlFiles = Directory.GetFiles(scriptsFolder, "*.psql").OrderBy(f => f).ToList();
        _logger.LogInformation($"Found {sqlFiles.Count} SQL scripts to execute...");

        foreach (var file in sqlFiles)
        {
            var scriptName = Path.GetFileName(file);

            // ✅ In-Memory check — no database call needed
            if (executedScripts.Contains(scriptName))
            {
                _logger.LogInformation($"⚠️ Skipping {scriptName} - already executed.");
                continue;
            }

            try
            {
                _logger.LogInformation($"🚀 Executing script: {scriptName}");
                var scriptContent = File.ReadAllText(file);
                connection.Execute(scriptContent);

                // Log it as executed
                connection.Execute(
                    "INSERT INTO migration_history (script_name) VALUES (@scriptName)",
                    new { scriptName }
                );

                _logger.LogInformation($"✅ Successfully executed: {scriptName}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Error executing {scriptName}");
                break;
            }
        }

        connection.Close();
        _logger.LogInformation("Database migration completed successfully.");
    }
}
