using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

namespace _08IncreaseMinionAge
{
    class StartUp
    {
        static void Main()
        {
            var minionsId = Console.ReadLine().Split();

            using var connection = new SqlConnection(@"Server=.\SQLEXPRESS;
                                                       Database=MinionsDB;
                                                       Integrated Security=true;");
            connection.Open();

            var command = new SqlCommand("", connection);

            foreach (var id in minionsId)
            {
                command = new SqlCommand(" UPDATE Minions " +
                                         "    SET Name = UPPER(LEFT(Name, 1)) + SUBSTRING(Name, 2, LEN(Name)), Age += 1 " +
                                         "  WHERE Id = @Id", connection);
                command.Parameters.AddWithValue("@Id", id);

                command.ExecuteNonQuery();
            }
            
            command = new SqlCommand("SELECT Name, Age " +
                                     "  FROM Minions", connection);

            using var reader = command.ExecuteReader();

            var towns = new List<string>();

            while (reader.Read())
            {
                Console.WriteLine($"{reader["Name"]} {reader["Age"]}");
            }
        }
    }
}
