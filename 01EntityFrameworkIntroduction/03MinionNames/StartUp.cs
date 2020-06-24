using Microsoft.Data.SqlClient;
using System;

namespace _03MinionNames
{
    class StartUp
    {
        static void Main()
        {
            int input = int.Parse(Console.ReadLine());

            using var connection = new SqlConnection(@"Server=.\SQLEXPRESS;
                                                       Database=MinionsDB;
                                                       Integrated Security=true;");

            connection.Open();

             var commandVillain = new SqlCommand($"SELECT Name as VillainName FROM Villains WHERE Id = @Id;", connection);

            commandVillain.Parameters.AddWithValue("@Id", input);

            var readerVillain = commandVillain.ExecuteScalar();

            if (readerVillain == null)
            {
                Console.WriteLine($"No villain with ID {input} exists in the database.");
                return;
            }

            Console.WriteLine($"Villain: {readerVillain}");

            var commandMinion = new SqlCommand($"  SELECT ROW_NUMBER() OVER (ORDER BY m.Name) as RowNum, m.Name as MinionName, m.Age " +
                                                "    FROM MinionsVillains AS mv " +
                                                "    JOIN Minions as m ON mv.MinionId = m.Id " +
                                                "   WHERE mv.VillainId = @Id " +
                                                "ORDER BY MinionName;", connection);

            commandMinion.Parameters.AddWithValue("@Id", input);

            using var readerMinion = commandMinion.ExecuteReader();

            if (!readerMinion.HasRows)
            {
                Console.WriteLine($"(no minions)");
                return;
            }

            int count = 1;

            while (readerMinion.Read())
            {
                Console.WriteLine($"{count++}. {readerMinion["MinionName"]} {readerMinion["Age"]}");
            }
        }
    }
}
