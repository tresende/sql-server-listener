
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Data.SqlClient;

namespace SQLListener
{
    public class Program
    {
        static void Main(string[] args)
        {
            Listen();
            Console.ReadKey();
        }

        private static void Listen()
        {
            string connectionString = GetConnectionString();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = "select id, name from dbo.item2 order by id desc";
                SqlCommand command = new SqlCommand(query, connection);
                SqlDependency dependency = new SqlDependency(command);
                dependency.OnChange += OnChange;
                SqlDependency.Start(connectionString);
                var item = GetItem(command);
                command.Dispose();
                Print(item);
            }
        }

        private static string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["trigger-database"].ConnectionString;
        }

        private static void Print(Item item)
        {
            var text = JsonConvert.SerializeObject(item);
            Console.WriteLine(text);
        }

        private static Item GetItem(SqlCommand command)
        {
            var item = new Item();
            var reader = command.ExecuteReader();
            if (reader.Read())
            {
                item.Id = reader.GetInt32(reader.GetOrdinal("id"));
                item.Name = reader.GetString(reader.GetOrdinal("name"));
            }
            return item;
        }

        private static void OnChange(object sender, SqlNotificationEventArgs e)
        {
            Listen();
        }
    }
}
