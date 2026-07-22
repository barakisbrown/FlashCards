namespace DataLayer.Controller;

using DataLayer.Models;
using Dapper;
using Microsoft.Data.SqlClient;


public class SessionController
{
    private readonly string _tableName = "dbo.SESSIONS";
    private readonly static DbUser _dataSource = Configuration.GetUserSecretsConnStrings();
    private readonly string _selectSQL = "SELECT * FROM ";
    private readonly List<Session> _sessions = new();

    public SessionController()
    {
        _selectSQL = "" + _tableName;
        _sessions = GetAllSessions();
    }

    public List<Session> GetAllSessions()
    {
        using var conn = MakeConnection;
        return conn.Query<Session>(_selectSQL).ToList();
    }

    private SqlConnection MakeConnection
    {
        get
        {
            var conn = new SqlConnection(_dataSource.Main);
            if (conn.State != System.Data.ConnectionState.Open)
            {
                try { conn.Open(); } catch (SqlException) { throw; }                   
            }
            return conn;
        }
    }

    public bool AddSession(Session data)
    {
        throw new NotImplementedException();        
    }

    public int COUNT => _sessions.Count;

}
