using Dapper;
using DataLayer.Models;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using Spectre.Console;
using System.Data;

namespace DataLayer.Controller
{    
    /// <summary>
    /// MATT
    /// FIRST CREATE A PARAM OBJECT
    /// Use MakConnection and then Execute except QUERYS
    /// </summary>
    public class StackController
    {
        private static readonly DbUser _dataSource = Configuration.GetUserSecretsConnStrings();
        private const string _selectSql = "SELECT * FROM dbo.Stack";
        private const string _insertSql = "INSERT INTO dbo.Stack (Name) VALUES (@Name)";
        private const string _editSQL = "UPDATE dbo.Stack SET Name = @NewName WHERE Name = @OldName";
        private const string _deleteSQL = "DELETE FROM dbo.Stack WHERE Name = @Name";
        private const string _viewSql = "SELECT * FROM dbo.CardsPerStack";
        private List<Stack> _stacks = new List<Stack>();

        public StackController()
        {
            _stacks = GetAllStacks();
        }

        public bool AddStack(string? Name)
        {
            if (string.IsNullOrWhiteSpace(Name) || VerifyName(Name))
                return false;

            var rows = MakeConnection.Execute(_insertSql, new { Name });
            if (rows == 1)
            {
                _stacks = GetAllStacks();
                return true;
            }

            return false;
        }

        public bool EditStack(string? NameEdit)
        {
            if (NameEdit.IsNullOrEmpty()) return false;

            if (VerifyName(NameEdit)) return false;

            if (MakeConnection.Execute(_editSQL, new { Name = NameEdit }) == 1)
            {
                _stacks = GetAllStacks();
                return true;
            }

            return false;
        }
        
        public bool DeleteStack(string? deleteMe)
        {
            if (deleteMe.IsNullOrEmpty() || deleteMe == "DEFAULT") return false;
            var deleted = MakeConnection.Execute(_deleteSQL, new { Name = deleteMe }) == 1;
            if (deleted)
            {
                _stacks = GetAllStacks();
                return true;
            }
            return false;
        }


        public List<Stack> GetAllStacks()
        {
            return MakeConnection.Query<Stack>(_selectSql).ToList();

        }

        /// <summary>
        /// Searches the internal list for a specified name
        /// </summary>
        /// <param name="Name">Name to be found</param>
        /// <returns>true if found false if not found</returns>
        public bool VerifyName(string? Name) => _stacks.Any(x => x.Name == Name);

        public int COUNT => _stacks.Count;
        private SqlConnection MakeConnection
        {
            get
            {
                var conn = new SqlConnection(_dataSource.Main);
                if (conn.State != System.Data.ConnectionState.Open)
                    try
                    {
                        conn.Open();
                    }
                    catch (SqlException)
                    {
                        throw;
                    }
                return conn;
            }
        }

        public List<CardsPerStackDTO> StackTotalCardView()
        {
            return MakeConnection.Query<CardsPerStackDTO>(_viewSql).ToList();
            
        }
    }
}
