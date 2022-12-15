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
          Console.WriteLine("Type 1 to View All Inspection Records.");
          Console.WriteLine("Type 2 to Insert Record.");
          Console.WriteLine("Type 3 to Delete Record.");
          Console.WriteLine("Type 4 to Update Record.");
          Console.WriteLine("Type 5 to View All Facilities.");
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
              GetAllRecords("1");
              break;
            // case "2":
            //   Insert();
            //   break;
            // case "3":
            //   Delete();
            //   break;
            // case "4":
            //   Update();
            //   break;
            case "5":
              GetAllRecords("5");
              break;
            // default:
            //   Console.WriteLine("\nInvalid Command. Please type a number from 0 to 4.\n");
            //   break;
          }
        }
      }

    private static void GetAllRecords(string input)
    {
      Console.Clear();

      // Get all inspections
      if (input == "1")
      {
        using (var connection = new SqliteConnection(connectionString))
        {
          connection.Open();
          var tableCmd = connection.CreateCommand();
          tableCmd.CommandText =
            $"SELECT * FROM inspections";
  
          List<Inspections> tableData = new();
  
          SqliteDataReader reader = tableCmd.ExecuteReader();
  
          if (reader.HasRows)
          {
            while (reader.Read())
            {
              tableData.Add(
                new Inspections
                {
                  inspection_id = reader.GetInt32(0),
                  facility_id = reader.GetString(1),
                  date = DateTime.ParseExact(reader.GetString(2), "dd-MM-yy", new CultureInfo("en-US")),
                  lead_level = reader.GetFloat(3)
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
            Console.WriteLine($"{dw.inspection_id} - Facility: {dw.facility_id} | Date: {dw.date.ToString("dd-MMM-yyyy")} | Lead Level: {dw.lead_level}");
          }
          Console.WriteLine("------------------------------------------\n");
        }
      }

      // Get all facilities
      if (input == "5")
      {
        using (var connection = new SqliteConnection(connectionString))
        {
          connection.Open();
          var tableCmd = connection.CreateCommand();
          tableCmd.CommandText =
            $"SELECT * FROM facilities";
  
          List<Facilities> tableData = new();
  
          SqliteDataReader reader = tableCmd.ExecuteReader();
  
          if (reader.HasRows)
          {
            while (reader.Read())
            {
              tableData.Add(
                new Facilities
                {
                  facility_id = reader.GetInt32(0),
                  facility_name = reader.GetString(1),
                  facility_type = reader.GetString(2),
                  city = reader.GetString(3),
                  state = reader.GetString(4),
                  year_built = reader.GetInt32(5)
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
            Console.WriteLine($"{dw.facility_id} - Name: {dw.facility_name} | Type: {dw.facility_type} | City: {dw.city} | State: {dw.state} | Year Built: {dw.year_built}");
          }
          Console.WriteLine("------------------------------------------\n");
        }
      }
    }
    private static void Insert()
    {
      string location = GetStringInput();
      string date = GetDateInput();

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
      GetAllRecords("1");

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
      GetAllRecords("1");

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

        string location = GetStringInput();
        string date = GetDateInput();

        float quantity = GetNumberInput("\n\nPlease enter quantity of lead detected in parts per billion (ppb)\n\n");

        var tableCmd = connection.CreateCommand();
        tableCmd.CommandText = $"UPDATE inspections SET location = '{location}', date = '{date}', quantity = {quantity} WHERE Id = {recordId}";

        tableCmd.ExecuteNonQuery();

        connection.Close();
      }
    }

    internal static string GetStringInput()
    {
      Console.WriteLine("\n\nPlease enter location. Type 0 to return to main menu.\n\n");

      string stringInput = Console.ReadLine();

      if (stringInput == "0") GetUserInput();

      return stringInput;
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
  public class Inspections
  {
    public int inspection_id { get; set; }
    public string facility_id { get; set; }
    public DateTime date { get; set; }
    public float lead_level { get; set; }
  }

  public class Facilities
  {
    public int facility_id { get; set; }
    public string facility_name { get; set; }
    public string facility_type { get; set; }
    public string city { get; set; }
    public string state { get; set; }
    public int year_built { get; set; }
  }
}