using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace DatabaseBackupRestoreProject
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                DBOperaions dbOperations = new DBOperaions();

                //get files from the database backup path
                string[] backupFilesPath = Directory.GetFiles(ConfigurationManager.AppSettings["backupFilsPath"], "*.bak")
                                         .Select(Path.GetFileName)
                                         .ToArray();

                string[] databases = new string[backupFilesPath.Length];

                //get databases name
                for (int i = 0; i < backupFilesPath.Length; i++)
                {

                    string[] databaseNameArr = backupFilesPath[i].Split('_');
                    databases[i] = databaseNameArr[0];
                }

                //loop create and restore databases
                for (int i = 0; i < databases.Length; i++)
                {

                    Console.WriteLine("Database Name:" + databases[i] + " Database File:" + backupFilesPath[i]);
                   
                    //create new database
                    dbOperations.createNewDataBase(databases[i]);

                    //restore database
                    dbOperations.restoreDatabase(databases[i], backupFilesPath[i]);
                    Console.WriteLine("-----------------------------------------------------------------------");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error in Execution (" + ex.Message + ")");
            }

        }
    }
}
