using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

namespace _07PrintAllMinionNames
{
    class StartUp
    {
        static void Main()
        {
            using var connection = new SqlConnection(@"Server=.\SQLEXPRESS;
                                                       Database=MinionsDB;
                                                       Integrated Security=true;");
            connection.Open();

            var command = new SqlCommand("SELECT Name FROM Minions", connection);

            using var reader = command.ExecuteReader();

            var minionNames = new List<string>();

            while (reader.Read())
            {
                minionNames.Add(reader["Name"].ToString());
            }

            var result = new List<string>();

            while (minionNames.Count > 1)
            {
                result.Add(minionNames[0]);
                result.Add(minionNames[^1]);

                minionNames.RemoveAt(0);
                minionNames.RemoveAt(minionNames.Count - 1);
            }

            if (minionNames.Count == 1)
            {
                result.Add(minionNames[0]);
            }

            Console.WriteLine(string.Join(Environment.NewLine, result));
        }
    }
}
