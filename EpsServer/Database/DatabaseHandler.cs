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

        public bool CheckProduct(string[] products)
        {
            bool result = false;

            foreach (string product in products)
            {
                List<ProductModel> items = this.GetProduct(product);

                if (items.Any() && items.Count == 1)
                {
                    result = true;
                }
                else
                {
                    result = false;
                    break;
                }
            }

            return result;
        }

        private List<ProductModel> GetProduct(string product)
        {
            SQLiteDataReader reader;
            SQLiteCommand sqlite_cmd = this.Db.CreateCommand();
            sqlite_cmd.CommandText = $"SELECT * FROM Product WHERE code = '{product}'";

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

            return list;
        }

        public int UseCode(string discountCode)
        {
            int result = 0;

            DiscountModel discount = CheckDiscountCode(discountCode);

            if (discount != null)
            {
                using (SQLiteCommand cmd = new SQLiteCommand(this.Db))
                {
                    using (var transaction = this.Db.BeginTransaction())
                    {
                        cmd.CommandText = $"UPDATE Discount SET IsUsed = 1 WHERE Id = {discount.Id}";
                        result = cmd.ExecuteNonQuery();

                        transaction.Commit();
                    }
                }
            }

            return result;
        }

        public DiscountModel CheckDiscountCode(string discountCode)
        {
            DiscountModel result = default;

            SQLiteDataReader reader;
            SQLiteCommand sqlite_cmd = this.Db.CreateCommand();
            sqlite_cmd.CommandText = $"SELECT * FROM Discount WHERE Code = '{discountCode}'";

            reader = sqlite_cmd.ExecuteReader();

            List<DiscountModel> list = new List<DiscountModel>();

            while (reader.Read())
            {
                list.Add(new DiscountModel()
                {
                    Id = (int)(long)reader["id"],
                    Code = reader["Code"].ToString(),
                    IsUsed = (int)(long)reader["IsUsed"],
                    ProductCodes = reader["ProductCodes"].ToString().Split(';')
                });
            }

            if (list.Any())
            {
                result = list.First();
            }

            return result;
        }

        public void GenerateCodes(HashSet<string> codes, string[] products)
        {
            string productsString = string.Join(";", products);

            using (SQLiteCommand cmd = new SQLiteCommand(this.Db))
            {
                using (var transaction = this.Db.BeginTransaction())
                {
                    foreach (string code in codes)
                    {
                        cmd.CommandText = $"insert into Discount(Code, ProductCodes, IsUsed) values ('{code}', '{productsString}',0)";
                        cmd.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
            }
        }
    }
}
