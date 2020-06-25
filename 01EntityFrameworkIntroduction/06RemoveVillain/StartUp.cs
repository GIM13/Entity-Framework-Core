using Microsoft.Data.SqlClient;
using System;

namespace _06RemoveVillain
{
    class StartUp
    {
        static void Main()
        {
            string idVillain = Console.ReadLine();



            using var connection = new SqlConnection(@"Server=.\SQLEXPRESS;
                                                       Database=MinionsDB;
                                                       Integrated Security=true;");

            connection.Open();

            var transaction = connection.BeginTransaction();

            var command = new SqlCommand("SELECT Name " +
                                         "  FROM Villains " +
                                         " WHERE Id = @villainId", connection);
            command.Parameters.AddWithValue("@villainId", idVillain);
            command.Transaction = transaction;

            string nameVillain = command.ExecuteScalar()?.ToString();

            string result = string.Empty;

            if (nameVillain == null)
            {
                result = "No such villain was found.";
            }
            else
            {
                try
                {
                    command = new SqlCommand("DELETE FROM MinionsVillains " +
                                              "WHERE VillainId = @villainId", connection);
                    command.Parameters.AddWithValue("@villainId", idVillain);
                    command.Transaction = transaction;

                    int num = command.ExecuteNonQuery();

                    command = new SqlCommand("DELETE FROM Villains " +
                                             " WHERE Id = @villainId", connection);
                    command.Parameters.AddWithValue("@villainId", idVillain);
                    command.Transaction = transaction;

                    command.ExecuteNonQuery();

                    transaction.Commit();

                    result = $"{nameVillain} was deleted." + Environment.NewLine +
                             $"{num} minions were released.";
                }
                catch (Exception ex)
                {
                    try
                    {
                        result = ex.Message;

                        transaction.Rollback();
                    }
                    catch (Exception e)
                    {
                        result = e.Message;
                    }
                }
            }
            Console.WriteLine(result);
        }
    }
}
