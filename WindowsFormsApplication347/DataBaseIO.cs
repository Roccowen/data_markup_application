using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;

namespace markup_data_app
{
    class DataBaseIO
    {
        SQLiteConnection dbConnection { get; }
        string Path { get; }

        public DataBaseIO(string path)
        {
            Path = path;
            if (!File.Exists(Path))
                SQLiteConnection.CreateFile(Path);
            dbConnection = new SQLiteConnection(@"DataSource=" + Path + "; Version=3;");
            
            string commandText = "CREATE TABLE IF NOT EXISTS [markup_data] ( " +               
                "[path] TEXT PRIMARY KEY NOT NULL, " +
                "[marks] TEXT)";
            ExecuteCommand(commandText);
        }
        private void ExecuteCommand(string commandText = null, SQLiteCommand sQLiteCommand = null)
        {
            if (sQLiteCommand == null)
                sQLiteCommand = new SQLiteCommand(commandText, dbConnection);
            dbConnection.Open();
            sQLiteCommand.ExecuteNonQuery();
            dbConnection.Close();
        }
        public void WriteMarks(string path, string marks)
        {
            try
            {
                string commandText = "INSERT INTO [markup_data] ([path], [marks]) VALUES (@path, @marks)";
                var command = new SQLiteCommand(commandText, dbConnection);
                command.Parameters.AddWithValue("@path", path);
                command.Parameters.AddWithValue("@marks", marks);
                ExecuteCommand(commandText, command);
            }
            catch
            {
                string commandText = "UPDATE [markup_data] SET [marks] = @marks WHERE [path] = @path";
                var command = new SQLiteCommand(commandText, dbConnection);
                command.Parameters.AddWithValue("@path", path);
                command.Parameters.AddWithValue("@marks", marks);
                ExecuteCommand(commandText, command);
            }
        }





    }
}
