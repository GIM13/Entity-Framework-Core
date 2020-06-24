using Microsoft.Data.SqlClient;
using System;

namespace _02DatabaseFirst
{
    class StartUp
    {
        static void Main()
        {
            using var connection = new SqlConnection(@"Server=.\SQLEXPRESS;
                                                       Database=MinionsDB;
                                                       Integrated Security=true");

            var command = new SqlCommand("  SELECT v.Name, COUNT(mv.VillainId) AS MinionsCount " +
                                         "    FROM Villains AS v " +
                                         "    JOIN MinionsVillains AS mv ON v.Id = mv.VillainId " +
                                         "GROUP BY v.Id, v.Name " +
                                         "  HAVING COUNT(mv.VillainId) > 3 " +
                                         "ORDER BY COUNT(mv.VillainId)", connection);

            connection.Open();

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                Console.WriteLine($"{reader["Name"]} - {reader["MinionsCount"]}");
            }
        }
    }
}
