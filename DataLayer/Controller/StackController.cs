using Dapper;
using DataLayer.Models;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using Spectre.Console;
using System.Data;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;

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
        private const string _selectSql = "SELECT * FROM dbo.Stack WHERE Stack.Name != 'DEFAULT'";
        private const string _insertSql = "INSERT INTO dbo.Stack (Name) VALUES (@Name)";
        private const string _editSQL = "UPDATE dbo.Stack SET Name = @NewName WHERE Name = @OldName";
        private const string _deleteSQL = "DELETE FROM dbo.Stack WHERE Name = @Name";
        private const string _viewSql = "SELECT * FROM dbo.CardsPerStack";
        private List<Stack> _stacks = new List<Stack>();

        public StackController()
        {
            _stacks = GetAllStacks();
        }

        public (bool,bool) AddStack(string? Name)
        {
            bool added = false;
            bool unique = false;
            if (string.IsNullOrEmpty(Name)) return (added,unique);
            else
            {
                try
                {
                    var rows = MakeConnection.Execute(_insertSql, new { Name });
                }
                catch (SqlException cmd) when (cmd.Number == 2627)
                {
                   
                    added = false;
                    unique = true;
                }
                finally
                {
                    added = true;
                }
            }

            return (added,unique);
        }

        public bool EditStack(string? OrigName, string NewEdit)
        {
            if (string.IsNullOrEmpty(OrigName) || string.IsNullOrEmpty(NewEdit)) return false;

            Object[] parm = { new { NewName = NewEdit, OldName = OrigName } };
            if (MakeConnection.Execute(_editSQL, parm) == 1)
            {
                _stacks = GetAllStacks();
                return true;
            }

            return false;
        }
        
        public bool DeleteStack(string? deleteMe)
        {
            if (deleteMe.IsNullOrEmpty() || (deleteMe.ToLower().Contains("default"))) return false;
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
