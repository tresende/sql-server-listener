
using Newtonsoft.Json;
using System;
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
            var cs = "Data Source=.\\SQLEXPRESS;Initial Catalog=Ports;Integrated Security=True";
            using (SqlConnection connection = new SqlConnection(cs))
            {
                connection.Open();
                var query = "select id, name from item2 order by id desc";
                SqlCommand command = new SqlCommand(query, connection);
                SqlDependency dependency = new SqlDependency(command);
                dependency.OnChange += OnChange;
                SqlDependency.Start(cs);
                var item = GetItem(command);
                command.Dispose();
                Print(item);
            }
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
