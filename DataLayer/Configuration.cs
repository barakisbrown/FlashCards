using Microsoft.Extensions.Configuration;
using DataLayer.Models;

namespace DataLayer;


/// <summary>
/// Class that loads the AppSettings into either of the following:
/// Strongly Types Class => DataLayer.Models.DbSetup
/// Returns the Database Configuration String
/// </summary>
public static class Configuration
{
    private const string ConfigStringName = "MyDBSqlite";

    public static DbConfig LoadSettings()
    {
        var path = AppDomain.CurrentDomain.BaseDirectory + "\\Properties";
        
        var builder = new ConfigurationBuilder()
            .SetBasePath(path)
            .AddJsonFile("appsettings.json", false);

        IConfiguration configuration = builder.Build();

        return configuration.GetSection("AppKey").Get<DbConfig>();
    }

    public static string GetConnectionStrings()
    {
        var path = AppDomain.CurrentDomain.BaseDirectory + "\\Properties";

        var builder = new ConfigurationBuilder()
            .SetBasePath(path)
            .AddJsonFile("appsettings.json", false);

        IConfiguration configuration = builder.Build();

        return configuration.GetConnectionString(ConfigStringName);
    }
}