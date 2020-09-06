namespace Database
{
    using System;

    using System.Data.SQLite;
    using Interfaces;

    public class DatabaseHandler : IDatabaseHandler
    {
        public DatabaseHandler()
        {
            SQLiteConnection sqlite_conn;
            sqlite_conn = CreateConnection();
        }

        private SQLiteConnection CreateConnection()
        {

            SQLiteConnection sqlite_conn;
            // Create a new database connection:
            sqlite_conn = new SQLiteConnection("Data Source=database.db; Version = 3; New = True; Compress = True; ");
            // Open the connection:
            try
            {
                sqlite_conn.Open();
            }
            catch (Exception ex)
            {

            }
            return sqlite_conn;
        }
    }
}
