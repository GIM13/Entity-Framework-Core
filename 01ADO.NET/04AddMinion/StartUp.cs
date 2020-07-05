using Microsoft.Data.SqlClient;
using System;
using System.Linq;

namespace _04AddMinion
{
    class StartUp
    {
        static void Main()
        {
            string[] inputMinion = Console.ReadLine().Split().ToArray();
            string villainName = Console.ReadLine().Split().ToArray()[1];

            using var connection = new SqlConnection(@"Server=.\SQLEXPRESS;
                                                       Database=MinionsDB;
                                                       Integrated Security=true;
                                                       MultipleActiveResultSets=True;");
            connection.Open();

            var command = new SqlCommand("SELECT Id " +
                                           "FROM Towns " +
                                          "WHERE Name = @townName; ", connection);

            string minionTown = inputMinion[3];

            command.Parameters.AddWithValue("@townName", minionTown);

            var townId = command.ExecuteScalar()?.ToString();

            if (townId == null)
            {
                command = new SqlCommand("INSERT INTO Towns (Name) " +
                                         "VALUES (@townName); ", connection);
                command.Parameters.AddWithValue("@townName", minionTown);

                command.ExecuteNonQuery();

                Console.WriteLine($"Town {minionTown} was added to the database.");
            }

            IdVillain(villainName, connection, out command, out string villainId);

            if (villainId == null)
            {
                command = new SqlCommand("INSERT INTO Villains(Name, EvilnessFactorId) " +
                                         "VALUES (@villainName, 4)", connection);
                command.Parameters.AddWithValue("@villainName", villainName);

                command.ExecuteNonQuery();

                Console.WriteLine($"Villain {villainName} was added to the database.");
            }

            string minionName = inputMinion[1];

            IdMinion(connection, out command, minionName, out string minionId);

            if (minionId == null)
            {
                string minionAge = inputMinion[2];

                command = new SqlCommand("INSERT INTO Minions (Name, Age, TownId) " +
                                         "VALUES (@minionName, @age, @townId); ", connection);
                command.Parameters.AddWithValue("@minionName", minionName);
                command.Parameters.AddWithValue("@age", minionAge);
                command.Parameters.AddWithValue("@townId", townId);

                command.ExecuteNonQuery();
            }

            IdMinion(connection, out command, minionName, out minionId);
            IdVillain(villainName, connection, out command, out villainId);

            command = new SqlCommand($"INSERT INTO MinionsVillains (MinionId, VillainId) " +
                                     $"VALUES ({minionId}, {villainId})", connection);

            command.ExecuteNonQuery();

            Console.WriteLine($"Successfully added {minionName} to be minion of {villainName}.");
        }

        private static void IdVillain(string villainName, SqlConnection connection, out SqlCommand command, out string villainId)
        {
            command = new SqlCommand("SELECT Id " +
                                       "FROM Villains " +
                                      "WHERE Name = @villainName;", connection);
            command.Parameters.AddWithValue("@villainName", villainName);

            villainId = command.ExecuteScalar()?.ToString();
        }

        private static void IdMinion(SqlConnection connection, out SqlCommand command, string minionName, out string minionId)
        {
            command = new SqlCommand("SELECT Id " +
                                       "FROM Minions " +
                                      "WHERE Name = @minionName; ", connection);
            command.Parameters.AddWithValue("@minionName", minionName);

            minionId = command.ExecuteScalar()?.ToString();
        }
    }
}
