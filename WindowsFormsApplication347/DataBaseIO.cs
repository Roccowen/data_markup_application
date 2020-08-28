using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Data.SqlClient;
using System.IO;

namespace markup_data_app
{
    class DataBaseIO
    {
        SqlConnection dbConnection;
        string Path { get; }

        public DataBaseIO(string path)
        {
            Path = path;
            if (!File.Exists(Path))
                SQLiteConnection.CreateFile(Path);
            dbConnection = new SqlConnection(@"DataSource=" + Path + "; Version=3;");
            using (dbConnection)
            {
                string commandText = "CREATE TABLE IF NOT EXISTS [markup_data] ( " +
                    "[id] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, " +
                    "[path] NVARCHAR(256), " +
                    "[marks] VARCHAR(256),), ";
            }
        }




    }
}
