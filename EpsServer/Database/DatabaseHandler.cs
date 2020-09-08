namespace Database
{
    using System;
    using System.Collections.Generic;
    using System.Data.SQLite;
    using System.Linq;
    using Interfaces;
    using Models;

    public class DatabaseHandler : IDatabaseHandler
    {
        private SQLiteConnection Db { get; }

        public DatabaseHandler()
        {
            this.Db = CreateConnection();
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

        public void ReadData()
        {
            SQLiteDataReader reader;
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = this.Db.CreateCommand();
            sqlite_cmd.CommandText = "SELECT * FROM Product";

            reader = sqlite_cmd.ExecuteReader();

            List<ProductModel> list = new List<ProductModel>();

            while (reader.Read())
            {
                list.Add(new ProductModel()
                {
                    Id = (int)(long)reader["id"],
                    Code = reader["Code"].ToString()
                });
            }

            foreach (var item in list)
            {
                Console.WriteLine($"{item.Code}");
            }
        }
    }
}
