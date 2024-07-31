using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Protocols;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Configuration;
using Microsoft.Data;

class Program
{

    static string GetConnectionString(string[] args)
    {
        string connectionString = ConfigurationManager.ConnectionStrings["MyDbConnection"].ConnectionString;
        Console.WriteLine(connectionString);
        return connectionString!;
    }

    static void InsertData(SqlConnection connection)
    {
        Console.WriteLine("" +
            "Read the data from the JSON files and write to server." +
            "Shows the datatype TDS = 244 supported on client side for " +
            "JSON parameters. ");

        string jsonData = "[\r\n    {\r\n        \"name\": \"Dave\",\r\n        \"skills\": [ \"Python\" ]\r\n    },\r\n    {\r\n        \"name\": \"Ron\",\r\n        \"surname\": \"Tianchen\"\r\n    }\r\n]";

        using (SqlCommand command = connection.CreateCommand())
        {
            string query = "Insert into jsonTab values (@jsonData)";
            command.CommandText = query;
            // Add the parameter and set its value
            var parameter = new SqlParameter("@jsonData", jsonData);
            parameter.SqlDbType = SqlDbTypeExtensions.Json;
            command.Parameters.Add(parameter);
            int rowsAffected = command.ExecuteNonQuery();
            Console.WriteLine($"Rows affected: {rowsAffected}");
        }
    }

    static void InsertDataUsingStoredProc(SqlConnection connection)
    {
        Console.WriteLine("" +
            "Read the data from the JSON files and write to server." +
            "Shows the datatype TDS = 244 supported on client side for " +
            "JSON parameters. ");

        string jsonData = "[\r\n    {\r\n        \"name\": \"Dave\",\r\n        \"skills\": [ \"Python\" ]\r\n    },\r\n    {\r\n        \"name\": \"Ron\",\r\n        \"surname\": \"Tianchen\"\r\n    }\r\n]";

        using (SqlCommand command = connection.CreateCommand())
        {
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "jsonsp";
            // Add the JSON data parameter
            command.Parameters.Add(new SqlParameter("@jsonData", SqlDbTypeExtensions.Json)
            {
                Value = jsonData
            });

            // Open the connection and execute the command
            command.ExecuteNonQuery();
        }
    }

    static void ReadData(SqlConnection connection)
    {
        Console.WriteLine("" +
            "Query a table from the server with JSON data." +
            "Shows the datatype TDS = 244 supported on client side for " +
            "JSON columns. ");
        using (SqlCommand command = connection.CreateCommand())
        {
            command.CommandText = "SELECT * FROM jsonTab";
            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                string json = reader.GetString(0);
                Console.WriteLine(json);
            }
            ReadOnlyCollection<DbColumn> schema = reader.GetColumnSchema();

            Console.WriteLine("" +
            "Demonstrates how the client supports JSON column and surfaces the information in the schema.. ");
            foreach (DbColumn column in schema)
            {
                Console.WriteLine(column.ColumnName);
                Console.WriteLine(column?.DataType?.ToString());
                Console.WriteLine(column?.DataTypeName);
            }
        }
    }

    static void Main(string[] args)
    {
        string connectionString = GetConnectionString(args);
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            InsertData(connection);
            ReadData(connection);
        }   
    }
}