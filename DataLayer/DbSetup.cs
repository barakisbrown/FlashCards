namespace DataLayer;

using DataLayer.Models;
using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using Spectre.Console;

/*
    * DataSource => SQLITE | SQLSERVER {EITHER CASE IT IS A FILE}
    * DbName => FlashCards
    * 3 Tables {Card / Stck / Study} -- Many Cards to 1 Stack Relation 
    * 1 VIEW which is virtual table of Stack and Card Data mostly for viewing
    * Scripts for the following:
    * Creation of all tables. All will be blank except Stack which record 1 is DEFAULT
    * Reading the data / Update Data / Deleting Data              
 */
public class DbSetup
{
    private readonly DbConfig appSettings = Configuration.LoadSettings();
    private readonly string connectionString = Configuration.GetConnectionStrings();
    private static bool IsSetup = false;

    public DbSetup()
    {
        if (!IsSetup)
            InitSetup();
    }

    private void InitSetup()
    {
        AnsiConsole.WriteLine("FlashCard App Database Setup");
        if (DbExist())
        {
            IsSetup = true;
        }
        else
        {
            AnsiConsole.WriteLine();
            AnsiConsole.WriteLine("Database [FlashCards] has not yet been created");
            // STEPS BELOW TO CREATE DB
            
        }
    }

    private bool DbExist()
    {
        using var conn = new SqlConnection(connectionString);
        if (conn.State != System.Data.ConnectionState.Open)
            conn.Open();

        // Connection is OPEN
        using var cmd = new SqlCommand();
        cmd.CommandText = "SELECT DB_ID(@DbName)";

        SqlParameter dbName = new("@DbName", System.Data.SqlDbType.NChar, appSettings.DbName.Length) { Value = appSettings.DbName };
        cmd.Parameters.Add(dbName);
        cmd.Connection = conn;
        cmd.Prepare();

        var Exist = cmd.ExecuteScalar();
        return DBNull.Value.Equals(Exist);
    }

    private bool TableExist(string? tableName)
    {
        using var conn = new SqlConnection(connectionString);
        if (conn.State != System.Data.ConnectionState.Open)
            conn.Open();

        using var cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "If Object_ID(@table,'U') IS NOT NULL SELECT 1 ELSE SELECT 0";

        SqlParameter parm = new("@table", System.Data.SqlDbType.NChar, tableName.Length) { Value = tableName };
        cmd.Parameters.Add(parm);
        cmd.Prepare();

        int? result = cmd.ExecuteScalar() as int?;
        return result == 1;
    }

    private void ExectureScript(string fileName)
    {
        Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
        string root = Directory.GetCurrentDirectory();
        string script = File.ReadAllText(root + "\\Scripts\\" + fileName);

        using var conn = new SqlConnection(connectionString);
        Server server = new(new ServerConnection(conn));

        try
        {
            server.ConnectionContext.ExecuteNonQuery(script);

        }catch(Exception e)
        {
            AnsiConsole.WriteLine($"Error Processing Sql Script.  Script being used is {fileName}");
            AnsiConsole.WriteLine($"Excpetion Message that was caught is : \n{e.Message}");
            throw;
        }

        AnsiConsole.WriteLine($"{fileName} script was successfully executed.");
    }
}
