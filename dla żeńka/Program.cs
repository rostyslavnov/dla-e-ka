using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;

namespace project
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string connectionString = "Data Source=LAPTOP-D44GI3J9;Initial Catalog=project;Integrated Security=True";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string lastFact = GetLastFactFromFile(); // Получаем последний факт из файла
                if (!string.IsNullOrEmpty(lastFact))
                {
                    Console.WriteLine("Ostatni zapisany fakt:");
                    Console.WriteLine(lastFact);
                    Console.WriteLine();
                }

                while (true)
                {
                    Console.WriteLine("Wybierz działanie:");
                    Console.WriteLine("1. Uzyskaj losowy fakt");
                    Console.WriteLine("0. Wyjście");

                    string input = Console.ReadLine();

                    switch (input)
                    {
                        case "1":
                            FactGenerator factGenerator = new FactGenerator();
                            factGenerator.Connection = connection;
                            string randomFact = factGenerator.DisplayRandomFact();
                            string filePath = @"D:\C#\sem 2\project\facts.txt";
                            SaveFactToFile(randomFact, filePath);
                            break;
                        case "0":
                            return;
                        default:
                            Console.WriteLine("\nNieprawidłowy wpis. Spróbuj ponownie.\n");
                            break;
                    }
                }
            }
        }

        static string GetLastFactFromFile()
        {
            string filePath = @"D:\C#\sem 2\project\facts.txt";
            if (File.Exists(filePath))
            {
                string[] lines = File.ReadAllLines(filePath);
                if (lines.Length > 0)
                {
                    return lines[lines.Length - 1];
                }
            }
            return null;
        }

        static void SaveFactToFile(string fact, string filePath)
        {
            using (StreamWriter writer = File.AppendText(filePath))
            {
                writer.WriteLine(fact);
            }
        }
    }

    public class FactGenerator
    {
        private SqlConnection connection;

        public SqlConnection Connection
        {
            get { return connection; }
            set { connection = value; }
        }

        public string DisplayRandomFact()
        {
            string query = "SELECT * FROM facts";
            using (SqlCommand command = new SqlCommand(query, Connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    var facts = new List<string>();

                    while (reader.Read())
                    {
                        facts.Add(reader["factText"].ToString());
                    }

                    Random rand = new Random();
                    int index = rand.Next(facts.Count);
                    Console.WriteLine("\n+----------------------------------+");
                    Console.WriteLine("|            Losowy fakt           |");
                    Console.WriteLine("+----------------------------------+");

                    string randomFact = facts[index];
                    Console.WriteLine(randomFact);
                    Console.WriteLine("+----------------------------------+\n");

                    return randomFact;
                }
            }
        }
    }
}
