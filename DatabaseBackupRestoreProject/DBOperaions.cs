using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace DatabaseBackupRestoreProject
{
    class DBOperaions
    {
        public SqlConnection conn = null;
        string connectionString = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
        public void ExecuteSQLStmt(string sql,string actionName,string databaseName)
        {
            if (conn.State == ConnectionState.Open)
                conn.Close();
            conn.ConnectionString = connectionString;
            conn.Open();
            SqlCommand cmd = new SqlCommand(sql, conn);
            try
            {
                int result = cmd.ExecuteNonQuery();
                Console.WriteLine(cmd.ExecuteNonQuery() == -1 ? actionName+" " + databaseName + " Successfully" : actionName + " " + databaseName +" Failed"+ result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(actionName + " " + databaseName +" ("+ ex.Message.ToString()+")");
            }
        }


        
        public void createNewDataBase(string databaseName)
        {
            string storeNewDatabasesPath = ConfigurationManager.AppSettings["storeNewDatabasesPath"];
           
            // Create a connection  
            conn = new SqlConnection(connectionString);
            // Open the connection  
            if (conn.State != ConnectionState.Open)
                conn.Open();
                string sql = "CREATE DATABASE "+ databaseName + " ON PRIMARY"
                + "(Name="+ databaseName + ", filename = '"+ storeNewDatabasesPath + databaseName + "_data.mdf', size=3,"
                + "maxsize=5, filegrowth=10%)log on"
                + "(name="+ databaseName + "_log, filename='"+storeNewDatabasesPath + databaseName + "_log.ldf',size=3,"
                + "maxsize=20,filegrowth=1)";
                ExecuteSQLStmt(sql, "Create Database ", databaseName);
        }

        public void restoreDatabase(string databaseName,string filename)
        {
            string filePath = ConfigurationManager.AppSettings["backupFilsPath"]+ filename;
           
            // Create a connection  
            conn = new SqlConnection(connectionString);
            // Open the connection  
            if (conn.State != ConnectionState.Open)
                conn.Open();

            string sqlStmt2 = string.Format("ALTER DATABASE [" + databaseName + "] SET SINGLE_USER WITH ROLLBACK IMMEDIATE");
            SqlCommand bu2 = new SqlCommand(sqlStmt2, conn);
            bu2.ExecuteNonQuery();

            string sqlStmt3 = "USE MASTER RESTORE DATABASE [" + databaseName + "] FROM DISK='" + filePath + "'WITH REPLACE;";
            SqlCommand bu3 = new SqlCommand(sqlStmt3, conn);
            bu3.ExecuteNonQuery();

            string sqlStmt4 = string.Format("ALTER DATABASE [" + databaseName + "] SET MULTI_USER");
            SqlCommand bu4 = new SqlCommand(sqlStmt4, conn);
            bu4.ExecuteNonQuery();

            Console.WriteLine(bu4.ExecuteNonQuery() >0 ? "Restore Database " + databaseName + " Successfully" : "Restore Database " +databaseName+ " Failed");

            conn.Close();

        }
    }
    }
