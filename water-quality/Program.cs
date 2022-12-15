// See https://aka.ms/new-console-template for more information
// Console.WriteLine("Hello, World!");

using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Data.Sqlite;

namespace water_quality
{
  class Program
  {
    static string connectionString = @"Data Source=water-Quality.db";
    static void Main(string[] args)
    {
      using (var connection = new SqliteConnection(connectionString))
      {
    
        connection.Open();
        var tableCmd = connection.CreateCommand();

        tableCmd.CommandText =
          @"CREATE TABLE IF NOT EXISTS inspections (
        inspection_id INTEGER PRIMARY KEY AUTOINCREMENT,
        facility_id INTEGER NOT NULL,
        date TEXT NOT NULL,
        lead_level REAL NOT NULL,
        FOREIGN KEY (facility_id)
          REFERENCES facilities (facility_id)
        );
        
        CREATE TABLE IF NOT EXISTS facilities (
        facility_id INTEGER PRIMARY KEY AUTOINCREMENT,
        facility_name TEXT NOT NULL,
        facility_type TEXT NOT NULL,
        city TEXT NOT NULL,
        state TEXT NOT NULL,
        year_built INTEGER NOT NULL
        )
        ";

        tableCmd.ExecuteNonQuery();

        connection.Close();
      }
      GetUserInput();
    }

    static void GetUserInput()
      {
        Console.Clear();
        bool closeApp = false;
        while (closeApp == false)
        {
          Console.WriteLine("\n\nWATER QUALITY DATA");
          Console.WriteLine("\nMAIN MENU");
          Console.WriteLine("\nPlease select an option.");
          Console.WriteLine("\nType 0 to Close Application.");
          Console.WriteLine("Type 1 to View All Records.");
          Console.WriteLine("Type 2 to Insert Record.");
          Console.WriteLine("Type 3 to Delete Record.");
          Console.WriteLine("Type 4 to Update Record.");
          Console.WriteLine("------------------------------------------\n");

          string command = Console.ReadLine();

          switch (command)
          {
            case "0":
              Console.WriteLine("\nGoodbye!\n");
              closeApp = true;
              Environment.Exit(0);
              break;
            case "1":
              GetAllRecords();
              break;
            case "2":
              Insert();
              break;
            case "3":
              Delete();
              break;
            case "4":
              Update();
              break;
            default:
              Console.WriteLine("\nInvalid Command. Please type a number from 0 to 4.\n");
              break;
          }
        }
      }

    private static void GetAllRecords()
    {
      Console.Clear();
      using (var connection = new SqliteConnection(connectionString))
      {
        connection.Open();
        var tableCmd = connection.CreateCommand();
        tableCmd.CommandText =
          $"SELECT * FROM inspections";

        List<WaterQuality> tableData = new();

        SqliteDataReader reader = tableCmd.ExecuteReader();

        if (reader.HasRows)
        {
          while (reader.Read())
          {
            tableData.Add(
              new WaterQuality
              {
                Id = reader.GetInt32(0),
                Location = reader.GetString(1),
                Date = DateTime.ParseExact(reader.GetString(2), "dd-MM-yy", new CultureInfo("en-US")),
                Quantity = reader.GetFloat(3)
              }); ;
          }
        } else
        {
          Console.WriteLine("No rows found");
        }

        connection.Close();

        Console.WriteLine("------------------------------------------\n");
        foreach (var dw in tableData)
        {
          Console.WriteLine($"{dw.Id} - Location: {dw.Location} | Date: {dw.Date.ToString("dd-MMM-yyyy")} | Quantity: {dw.Quantity}");
        }
        Console.WriteLine("------------------------------------------\n");
      }
    }
    private static void Insert()
    {
      string location = GetLocationInput();
      string date = GetDateInput();

      // TODO: add 'F' to quantity to pass floating point number to SQL command?
      // TODO: change all code from int to float?
      float quantity = GetNumberInput("\n\nPlease enter quantity of lead detected in parts per billion (ppb)\n\n");
      
      using (var connection = new SqliteConnection(connectionString))
      {
        connection.Open();
        var tableCmd = connection.CreateCommand();
        tableCmd.CommandText =
          $"INSERT INTO inspections(location, date, quantity) VALUES('{location}', '{date}', {quantity})";

        tableCmd.ExecuteNonQuery();

        connection.Close();
      }
    }

    private static void Delete()
    {
      Console.Clear();
      GetAllRecords();

      var recordId = GetNumberInput("\n\nPlease type the ID of the record you want to delete or type 0 to return to main menu.\n\n");

      using (var connection = new SqliteConnection(connectionString))
      {
        connection.Open();
        var tableCmd = connection.CreateCommand();

        tableCmd.CommandText = $"DELETE from inspections WHERE Id = '{recordId}'";

        int rowCount = tableCmd.ExecuteNonQuery();

        if (rowCount == 0)
        {
          Console.WriteLine($"\n\nRecord with ID {recordId} doesn't exist.\n\n");
          Delete();
        }
      }

      Console.WriteLine($"\n\nRecord with ID {recordId} was deleted.\n\n");

      GetUserInput();
    }

    internal static void Update()
    {
      GetAllRecords();

      var recordId = GetNumberInput("\n\nPlease type Id of the record you would like to update. Type 0 to return to main menu.\n\n");

      using (var connection = new SqliteConnection(connectionString))
      {
        connection.Open();

        var checkCmd = connection.CreateCommand();
        checkCmd.CommandText = $"SELECT EXISTS(SELECT 1 FROM inspections WHERE Id = {recordId})";
        int checkQuery = Convert.ToInt32(checkCmd.ExecuteScalar());

        if (checkQuery == 0)
        {
          Console.WriteLine($"\n\nRecord with Id {recordId} doesn't exist.\n\n");
          connection.Close();
          Update();
        }

        string location = GetLocationInput();
        string date = GetDateInput();

        float quantity = GetNumberInput("\n\nPlease enter quantity of lead detected in parts per billion (ppb)\n\n");

        var tableCmd = connection.CreateCommand();
        tableCmd.CommandText = $"UPDATE inspections SET location = '{location}', date = '{date}', quantity = {quantity} WHERE Id = {recordId}";

        tableCmd.ExecuteNonQuery();

        connection.Close();
      }
    }

    internal static string GetLocationInput()
    {
      Console.WriteLine("\n\nPlease enter location. Type 0 to return to main menu.\n\n");

      string locationInput = Console.ReadLine();

      if (locationInput == "0") GetUserInput();

      return locationInput;
    }
    internal static string GetDateInput()
    {
      Console.WriteLine("\n\nPlease insert the date: (Format: dd-mm-yy). Type 0 to return to main menu.\n\n");

      string dateInput = Console.ReadLine();

      if (dateInput == "0") GetUserInput();

      while (!DateTime.TryParseExact(dateInput, "dd-MM-yy", new CultureInfo("en-US"), DateTimeStyles.None, out _))
      {
        Console.WriteLine("\n\nInvalid date. (Format: dd-mm-yy). Type 0 to return to main menu or try again.\n\n");
        dateInput = Console.ReadLine();
      }

      return dateInput;
    }

    internal static float GetNumberInput(string message)
    {
      Console.WriteLine(message);

      string numberInput = Console.ReadLine();

      if (numberInput == "0") GetUserInput();

      // while (!Int32.TryParse(numberInput, out _) || Convert.ToInt32(numberInput) < 0)
      // {
      //   Console.WriteLine("\n\nInvalid number. Try again.\n\n");
      //   numberInput = Console.ReadLine();
      // }

      float finalInput = float.Parse(numberInput);

      return finalInput;
    }
  }
  public class WaterQuality
  {
    public int Id { get; set; }
    public string Location { get; set; }
    public DateTime Date { get; set; }
    public float Quantity { get; set; }
  }
}