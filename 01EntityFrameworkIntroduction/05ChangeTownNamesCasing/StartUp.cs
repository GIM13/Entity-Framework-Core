using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

namespace _05ChangeTownNamesCasing
{
    class StartUp
    {
        static void Main()
        {
            string countryName = Console.ReadLine();

            using var connection = new SqlConnection(@"Server=.\SQLEXPRESS;
                                                       Database=MinionsDB;
                                                       Integrated Security=true;");

            connection.Open();

            var command = new SqlCommand("UPDATE Towns " +
                                         "   SET Name = UPPER(Name) " +
                                         " WHERE CountryCode = (SELECT c.Id" +
                                                                " FROM Countries AS c " +
                                                                "WHERE c.Name = @countryName)", connection);
            command.Parameters.AddWithValue("@countryName", countryName);

            command.ExecuteNonQuery();

            command = new SqlCommand("SELECT t.Name  " +
                                     "  FROM Towns as t " +
                                     "  JOIN Countries AS c ON c.Id = t.CountryCode " +
                                     " WHERE c.Name = @countryName", connection);
            command.Parameters.AddWithValue("@countryName", countryName);

            using var reader = command.ExecuteReader();

            var towns = new List<string>();

            while (reader.Read())
            {
                towns.Add(reader["Name"].ToString());
            }

            if (towns.Count == 0)
            {
                Console.WriteLine("No town names were affected.");
            }
            else
            {
                Console.WriteLine($"{towns.Count} town names were affected.");

                Console.WriteLine("[" + string.Join(", ", towns) + "]");
            }
        }
    }
}
