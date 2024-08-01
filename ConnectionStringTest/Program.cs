using System;
using System.Data;
using Microsoft.Data.SqlClient;
using ConsoleTableExt;
using Figgle;

namespace DatabaseConnectionTester
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.Clear();
                DisplayTitle();

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Digite a sua connection string para o SQL Server:");
                Console.ResetColor();
                string connectionString = Console.ReadLine();

                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Conexão estabelecida com sucesso!");
                        Console.ResetColor();

                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine("Listando bancos de dados disponíveis...");
                        Console.ResetColor();
                        ListDatabases(connection);

                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("Digite o nome do banco de dados que deseja usar:");
                        Console.ResetColor();
                        string databaseName = Console.ReadLine();

                        connection.ChangeDatabase(databaseName);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Conectado ao banco de dados {databaseName}");
                        Console.ResetColor();

                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine("Listando tabelas disponíveis...");
                        Console.ResetColor();
                        ListTables(connection);

                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("Digite o nome completo da tabela para executar um SELECT:");
                        Console.ResetColor();
                        string tableName = Console.ReadLine();

                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine($"Executando SELECT TOP 10 * FROM {tableName}...");
                        Console.ResetColor();
                        ExecuteSelect(connection, tableName);
                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Erro ao conectar ao banco de dados: {ex.Message}");
                    Console.ResetColor();
                }

                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("Digite '0' para sair ou '1' para inserir uma nova connection string.");
                Console.ResetColor();
                string choice = Console.ReadLine();
                if (choice == "0")
                {
                    break;
                }
            }
        }

        static void DisplayTitle()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(FiggleFonts.Standard.Render("DotNet Core"));
            Console.WriteLine(FiggleFonts.Standard.Render("Connection String Test"));
            Console.ResetColor();
        }

        static void ListDatabases(SqlConnection connection)
        {
            string query = "SELECT name FROM sys.databases";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Console.WriteLine(reader.GetString(0));
                    }
                }
            }
        }

        static void ListTables(SqlConnection connection)
        {
            string query = @"
                SELECT TABLE_SCHEMA + '.' + TABLE_NAME AS FullTableName
                FROM INFORMATION_SCHEMA.TABLES 
                WHERE TABLE_TYPE = 'BASE TABLE'";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Console.WriteLine(reader.GetString(0));
                    }
                }
            }
        }

        static void ExecuteSelect(SqlConnection connection, string tableName)
        {
            string query = $"SELECT TOP 10 * FROM {tableName}";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    var table = new DataTable();
                    table.Load(reader);

                    Console.WriteLine();
                    ConsoleTableBuilder
                        .From(table)
                        .WithFormat(ConsoleTableBuilderFormat.Alternative)
                        .ExportAndWriteLine();
                    Console.WriteLine();
                }
            }
        }
    }
}
