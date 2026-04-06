using System.Data;
using Microsoft.Data.SqlClient;
using LDSTest.src.utils;
using TestContext = NUnit.Framework.TestContext;

public static class Database
{
    public static void Restore(DBServer dbServer, string guid, string backupFileName = "default.bak")
    {
        SqlConnection myConn;
        var connectionString = $"Server={dbServer.serverName};User Id={dbServer.userName};Password={dbServer.password};database=master";
        myConn = new SqlConnection(connectionString);
        myConn.Open();

        using (var command = new SqlCommand("TrxAutomationSetup", myConn))
        {
            command.CommandType = CommandType.StoredProcedure;

            
            string localBackupPath = "C:\\testautomation\\backup";
            string localDataPath = "C:\\Program Files\\Microsoft SQL Server\\MSSQL15.MSSQLSERVER\\MSSQL\\DATA";
            string localLogPath = localDataPath;

            string remoteBackupPath = "E:\\MSSQL\\Backup\\TestAutomation";
            string remoteDataPath = "E:\\MSSQL\\Data";
            string remoteLogPath = "D:\\MSSQL\\Log";

            string backupPath = TestContext.Parameters["environment"] == "local" ? localBackupPath : remoteBackupPath;
            string dataPath = TestContext.Parameters["environment"] == "local" ? localDataPath : remoteDataPath;
            string logPath = TestContext.Parameters["environment"] == "local" ? localLogPath : remoteLogPath;

            string parameterName;

            parameterName = "@BACKUPFILE";
            command.Parameters.Add(parameterName, SqlDbType.NVarChar, 30);
            command.Parameters[parameterName].Value = backupFileName;
            command.Parameters[parameterName].Direction = ParameterDirection.Input;

            parameterName = "@BACKUPPATH";
            command.Parameters.Add(parameterName, SqlDbType.NVarChar, 50);
            command.Parameters[parameterName].Value = backupPath;
            command.Parameters[parameterName].Direction = ParameterDirection.Input;

            parameterName = "@SERVERDATAPATH";
            command.Parameters.Add(parameterName, SqlDbType.NVarChar, 80);
            command.Parameters[parameterName].Value = dataPath;
            command.Parameters[parameterName].Direction = ParameterDirection.Input;

            parameterName = "@SERVERLOGPATH";
            command.Parameters.Add(parameterName, SqlDbType.NVarChar, 80);
            command.Parameters[parameterName].Value = logPath;
            command.Parameters[parameterName].Direction = ParameterDirection.Input;

            parameterName = "@GUID";
            command.Parameters.Add(parameterName, SqlDbType.NVarChar, 50);
            command.Parameters[parameterName].Value = guid;
            command.Parameters[parameterName].Direction = ParameterDirection.Input;

            parameterName = "@SERVERNAME";
            command.Parameters.Add(parameterName, SqlDbType.NVarChar, 50);
            command.Parameters[parameterName].Value = dbServer.serverName;
            command.Parameters[parameterName].Direction = ParameterDirection.Input;

            parameterName = "@DBNAME";
            command.Parameters.Add(parameterName, SqlDbType.NVarChar, 100);
            command.Parameters[parameterName].Value = "";
            command.Parameters[parameterName].Direction = ParameterDirection.Output;

            int result = command.ExecuteNonQuery();
        }

        myConn.Close();
    }

    public static void WaitForJobs(DBServer dbServer)
    {
        Int32 jobCount = 1;
        int loopCount = 0;
        int loopSleepSeconds = 10;
        int maxWaitSeconds = 60 * 30; //30 minutes
        int maxLoops = maxWaitSeconds / loopSleepSeconds;

        while (jobCount > 0 && loopCount < maxLoops)
        {
            TestContext.Progress.WriteLine("Database.WaitForJobs begin");
            //SELECT Id FROM [TRXConfig].[dbo].[Users] 
            //join[TRXConfig].[dbo].[Jobs] on useruid = id where Login = 'T9cnl8LP8g'
            string queryString = $"select COUNT(*) FROM [TRXConfig].[dbo].[Users] join[TRXConfig].[dbo].[Jobs] on useruid = id where Login = '{Test.trxUserName}'";
            string connectionString = $"Server={dbServer.serverName};User Id={dbServer.userName};Password={dbServer.password};database=TRXConfig";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();
                jobCount = Convert.ToInt32(command.ExecuteScalar());
                TestContext.Progress.WriteLine($"Jobs count = {jobCount}");
            }

            Thread.Sleep(10000);
            loopCount++;
        }
    }

    public static void RunQuery(DBServer dbServer, string query)
    {
        string dbName = TestContext.Parameters["databaseRootName"] + "_" + Test.guid;
        string connectionString = $"Server={dbServer.serverName};User Id={dbServer.userName};Password={dbServer.password};Database={dbName}";
        using SqlConnection connection = new SqlConnection(connectionString);
        SqlCommand command = new SqlCommand(query, connection);
        connection.Open();
        command.ExecuteScalar();
    }

    public static void Cleanup(DBServer dbServer, string guid)
    {
        SqlConnection myConn;

        string connectionString = $"Server={dbServer.serverName};User Id={dbServer.userName};Password={dbServer.password};database=master";
        myConn = new SqlConnection(connectionString);
        myConn.Open();

        using (var command = new SqlCommand("TrxAutomationCleanup", myConn))
        {
            command.CommandType = CommandType.StoredProcedure;

            string parameterName;

            parameterName = "@GUID";
            command.Parameters.Add(parameterName, SqlDbType.NVarChar, 50);
            command.Parameters[parameterName].Value = guid;
            command.Parameters[parameterName].Direction = ParameterDirection.Input;

            parameterName = "@DBNAME";
            command.Parameters.Add(parameterName, SqlDbType.NVarChar, 100);
            command.Parameters[parameterName].Value = "";
            command.Parameters[parameterName].Direction = ParameterDirection.Output;

            int result = command.ExecuteNonQuery();
        }

        myConn.Close();
    }
}