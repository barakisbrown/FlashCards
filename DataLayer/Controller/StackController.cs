using Dapper;
using DataLayer.Models;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using Spectre.Console;

namespace DataLayer.Controller
{    
    /// <summary>
    /// MATT
    /// FIRST CREATE A PARAM OBJECT
    /// Use MakConnection and then Execute except QUERYS
    /// </summary>
    public class StackController
    {
        private static readonly DbUser dataSource = Configuration.GetUserSecretsConnStrings();
        private readonly string selectSql = "SELECT * FROM dbo.Stack";
        private readonly string insertSql = "INSERT INTO dbo.Stack(Name) VALUE (@Name)";
        private readonly string editSQL = "UPDATE dbo.Stack WHERE dbo.Stack.Name = @Name";
        private readonly string deleteSQL = "DELETE dbo.Stack WHERE dbo.Stack.Name = @Name";
        private readonly string viewSql = "SELECT * FROM dbo.CardsPerStack";
        private List<Stack> stacks = [];

        public StackController()
        {
            stacks = GetAllStacks();
        }

        public bool AddStack(string? Name)
        {
            if (Name.IsNullOrEmpty() || VeriyName(Name)) return false;
            object[] param = { new { Name } };
            bool success = MakeConnection.Execute(insertSql, param) == 1;
            if (success)
            {
                stacks = GetAllStacks();
                return true;
            }

            return false;
        }

        public bool EditStack(string? NameEdit)
        {
            if (NameEdit.IsNullOrEmpty()) return false;

            if (VeriyName(NameEdit)) return false;

            if (MakeConnection.Execute(editSQL, new { Name = NameEdit }) == 1)
            {
                stacks = GetAllStacks();
                return true;
            }

            return false;
        }
        
        public bool DeleteStack(string? deleteMe)
        {
            if (deleteMe.IsNullOrEmpty() || deleteMe == "DEFAULT") return false;
            return MakeConnection.Execute(deleteSQL, new { Name = deleteMe }) == 1;
        }


        public List<Stack> GetAllStacks()
        {
            return MakeConnection.Query<Stack>(selectSql).ToList();

        }

        /// <summary>
        /// Searches the internal list for a specified name
        /// </summary>
        /// <param name="Name">Name to be found</param>
        /// <returns>true if found false if not found</returns>
        public bool VeriyName(string ?Name)
        {
            var exist = stacks.FirstOrDefault(x => x.Name == Name);
            return (exist == null);

        }

        public int COUNT => stacks.Count;
        private SqlConnection MakeConnection
        {
            get
            {
                var conn = new SqlConnection(dataSource.Main);
                if (conn.State != System.Data.ConnectionState.Open)
                    try
                    {
                        conn.Open();
                    }
                    catch (Exception e)
                    {
                        AnsiConsole.WriteLine("Error problem opening connection to the database engine. CHeck to see if it running.");
                        throw;
                    }
                return conn;
            }
        }

        public List<CardsPerStackDTO> StackTotalCardView()
        {
            return MakeConnection.Query<CardsPerStackDTO>(viewSql).ToList();
            
        }
    }
}
