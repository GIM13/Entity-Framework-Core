using Microsoft.Data.SqlClient;
using System;
using System.Data;

namespace _09IncreaseAgeStoredProcedure
{
    class StartUp
    {
        static void Main()
        {
            var minionsId = Console.ReadLine();

            using var connection = new SqlConnection(@"Server=.\SQLEXPRESS;
                                                       Database=MinionsDB;
                                                       Integrated Security=true;");
            connection.Open();

            var command = new SqlCommand("usp_GetOlder", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@Id", minionsId);

            command.ExecuteNonQuery();

            command = new SqlCommand("SELECT Name, Age " +
                                     "  FROM Minions " +
                                     " WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", minionsId);

            using var reader = command.ExecuteReader();

            reader.Read();

            Console.WriteLine($"{reader["Name"]} - {reader["Age"]} years old");
        }
    }
}
