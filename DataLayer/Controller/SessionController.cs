namespace DataLayer.Controller;

using DataLayer.Models;
using Dapper;
using Microsoft.Data.SqlClient;


public class SessionController
{
    private readonly string _tableName = "dbo.SESSIONS";
    private readonly static DbUser _dataSource = Configuration.GetUserSecretsConnStrings();
    private readonly string _selectSQL = "SELECT * FROM ";
    private readonly string _insertSQL;
    private List<Session> _sessions = new();

    public SessionController()
    {
        _selectSQL = "" + _tableName;
        _insertSQL = "INSERT INTO " + _tableName + "(StackName,StudyDate,NumQuestions,StudyScore,StackID)";
        _insertSQL += "values (@StackName,@StudyDate,@NumQuestions,@StudyScore,@StackID)";
        _sessions = GetAllSessions();
    }

    /// <summary>
    /// Selects all Session data and returns it
    /// </summary>
    /// <returns>List of Session Data</returns>
    public List<Session> GetAllSessions()
    {
        using var conn = MakeConnection;
        return conn.Query<Session>(_selectSQL).ToList();
    }

    /// <summary>
    /// Propery that returns a SqlConnection
    /// </summary>
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
    /// <summary>
    /// Add(Insert) a new Session Data[datetime and score]
    /// </summary>
    /// <param name="data">Datetime and score of a paticular session</param>
    /// <returns>added,message => added is true then message is blank else message will be the exception message</returns>
    public (bool,string) AddSession(Session data)
    {
        string message = string.Empty;
        bool added = false;
        if (string.IsNullOrEmpty(data.StackName)) added = false;
        else
        {
            try
            {                
                object[] param = { new { data.StackName,data.StudyDate,data.NumQuestions,data.StudyScore,data.StackID } };
                using var conn = MakeConnection;
                var rows = conn.Execute(_insertSQL, new { param });
                if (rows == 1)
                {
                    added = true;
                    _sessions = GetAllSessions();
                }
            }
            catch (SqlException cmd)
            {
                added = false;
                message = cmd.Message;
            }
        }
        return (added,message);
    }

    /// <summary>
    /// Returns how many session have been logged in the system
    /// </summary>
    public int COUNT => _sessions.Count;

}
