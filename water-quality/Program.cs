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
          Console.WriteLine("\nType 1 to View All Inspection Records.");
          Console.WriteLine("Type 2 to Insert Inspection Record.");
          Console.WriteLine("Type 3 to Delete Inspection Record.");
          Console.WriteLine("Type 4 to Update Inspection Record.");
          Console.WriteLine("\nType 5 to View All Facilities.");
          Console.WriteLine("Type 6 to Add New Facility.");
          Console.WriteLine("Type 7 to Delete Facility Information.");
          Console.WriteLine("Type 8 to Update Facility Information.");
          Console.WriteLine("Type 9 to View Inspections by Facility.");
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
              GetAllRecords("inspections");
              break;
            case "2":
              Insert("inspections");
              break;
            case "3":
              Delete("inspections");
              break;
            // case "4":
            //   Update("4");
            //   break;
            case "5":
              GetAllRecords("facilities");
              break;
            case "6":
              Insert("facilities");
              break;
            case "7":
              Delete("facilities");
              break;
            // case "8":
            //   Update("8");
            //   break;
            case "9":
              GetRecordsByFacility();
              break;
            // default:
            //   Console.WriteLine("\nInvalid Command. Please type a number from 0 to 4.\n");
            //   break;
          }
        }
      }

    private static void GetAllRecords(string option)
    {
      // Get all inspections
      if (option == "inspections")
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

          Console.WriteLine("INSPECTIONS");
          Console.WriteLine("------------------------------------------\n");
          foreach (var dw in tableData)
          {
            Console.WriteLine($"{dw.inspection_id} - Facility: {dw.facility_id} | Date: {dw.date.ToString("dd-MMM-yyyy")} | Lead Level: {dw.lead_level}");
          }
          Console.WriteLine("------------------------------------------\n");
        }
      }

      // Get all facilities
      if (option == "facilities")
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
  
          Console.WriteLine("FACILITIES");
          Console.WriteLine("------------------------------------------\n");
          foreach (var dw in tableData)
          {
            Console.WriteLine($"{dw.facility_id} - Name: {dw.facility_name} | Type: {dw.facility_type} | City: {dw.city} | State: {dw.state} | Year Built: {dw.year_built}");
          }
          Console.WriteLine("------------------------------------------\n");
        }
      }
    }
    private static void Insert(string option)
    {
      // Insert inspection
      if (option == "inspections")
      {
        GetAllRecords("facilities");
        int facilityId = GetIntInput("\n\nPlease enter facility ID. Type 'main menu' to return to main menu.\n\n");
        string date = GetDateInput();
  
        float leadLevel = GetFloatInput("\n\nPlease enter quantity of lead detected in parts per billion (ppb). Type 'main menu' to return to main menu.\n\n");
        
        using (var connection = new SqliteConnection(connectionString))
        {
          connection.Open();
          var tableCmd = connection.CreateCommand();
          tableCmd.CommandText =
            $"INSERT INTO inspections(facility_id, date, lead_level) VALUES('{facilityId}', '{date}', {leadLevel})";
  
          tableCmd.ExecuteNonQuery();
  
          connection.Close();
        }
      }

      // Insert facility
      if (option == "facilities")
      {
        string facilityName = GetStringInput("\n\nPlease enter name of facility. Type 0 to return to main menu.\n\n");
        string facilityType = GetStringInput("\n\nPlease enter type of facility. Type 0 to return to main menu.\n\n");
        string city = GetStringInput("\n\nPlease enter city where facility is located. Type 0 to return to main menu.\n\n");
        string state = GetStringInput("\n\nPlease enter state where facility is located. Type 0 to return to main menu.\n\n");

        int yearBuilt = GetIntInput("\n\nPlease enter year facility was built. Type 0 to return to main menu.\n\n");

        using (var connection = new SqliteConnection(connectionString))
        {
          connection.Open();
          var tableCmd = connection.CreateCommand();
          tableCmd.CommandText =
            $"INSERT INTO facilities(facility_name, facility_type, city, state, year_built) VALUES('{facilityName}', '{facilityType}', '{city}', '{state}', {yearBuilt})";
  
          tableCmd.ExecuteNonQuery();
  
          connection.Close();
        }
      }
    }

    private static void Delete(string option)
    {
      if (option == "inspections")
      {
        GetAllRecords("inspections");
  
        var recordId = GetIdInput();

        using (var connection = new SqliteConnection(connectionString))
        {
          connection.Open();
          var tableCmd = connection.CreateCommand();
  
          tableCmd.CommandText = $"DELETE from inspections WHERE inspection_id = '{recordId}'";
  
          int rowCount = tableCmd.ExecuteNonQuery();
  
          if (rowCount == 0)
          {
            Console.WriteLine($"\n\nInspection record with ID {recordId} doesn't exist.");
            Delete("inspections");
          }
        }
        Console.WriteLine($"\n\nInspection record with ID {recordId} was deleted.");
        GetUserInput();
      }

      if (option == "facilities")
      {
        GetAllRecords("facilities");
  
        var recordId = GetIdInput();
  
        using (var connection = new SqliteConnection(connectionString))
        {
          connection.Open();
          var tableCmdDeleteChildren = connection.CreateCommand();
          var tableCmdDeleteParent = connection.CreateCommand();
  
          tableCmdDeleteChildren.CommandText =
          $"DELETE from inspections WHERE facility_id = '{recordId}'";

          tableCmdDeleteParent.CommandText = $"DELETE from facilities WHERE facility_id = '{recordId}'";

          int rowCountChildren = tableCmdDeleteChildren.ExecuteNonQuery();
          int rowCountParent = tableCmdDeleteParent.ExecuteNonQuery();
  
          if (rowCountParent == 0)
          {
            Console.WriteLine($"\n\nInspection record with ID {recordId} doesn't exist.");
            Delete("inspections");
          }
        }

        Console.WriteLine($"\n\nInspection record with ID {recordId} was deleted.");
        GetUserInput();
      }
    }

    // internal static void Update()
    // {
    //   GetAllRecords("1");

    //   var recordId = GetFloatInput("\n\nPlease type Id of the record you would like to update. Type 0 to return to main menu.\n\n");

    //   using (var connection = new SqliteConnection(connectionString))
    //   {
    //     connection.Open();

    //     var checkCmd = connection.CreateCommand();
    //     checkCmd.CommandText = $"SELECT EXISTS(SELECT 1 FROM inspections WHERE Id = {recordId})";
    //     int checkQuery = Convert.ToInt32(checkCmd.ExecuteScalar());

    //     if (checkQuery == 0)
    //     {
    //       Console.WriteLine($"\n\nRecord with Id {recordId} doesn't exist.\n\n");
    //       connection.Close();
    //       Update();
    //     }

    //     string location = GetStringInput();
    //     string date = GetDateInput();

    //     float quantity = GetFloatInput("\n\nPlease enter quantity of lead detected in parts per billion (ppb)\n\n");

    //     var tableCmd = connection.CreateCommand();
    //     tableCmd.CommandText = $"UPDATE inspections SET location = '{location}', date = '{date}', quantity = {quantity} WHERE Id = {recordId}";

    //     tableCmd.ExecuteNonQuery();

    //     connection.Close();
    //   }
    // }

    private static void GetRecordsByFacility()
    {
      GetAllRecords("5");

      var facilityIdInput = GetIntInput("\n\nPlease type the ID of the facility.\n\n");

      int facilityId = Convert.ToInt32(facilityIdInput);

      using (var connection = new SqliteConnection(connectionString))
      {
        connection.Open();
        var tableCmd = connection.CreateCommand();
        tableCmd.CommandText =
          $"SELECT * FROM inspections WHERE facility_id={facilityId} ";

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

        Console.WriteLine("ALL INSPECTIONS FOR FACILITY " + facilityId);
        Console.WriteLine("------------------------------------------\n");
        foreach (var dw in tableData)
        {
          Console.WriteLine($"{dw.inspection_id} - Facility: {dw.facility_id} | Date: {dw.date.ToString("dd-MMM-yyyy")} | Lead Level: {dw.lead_level}");
        }
        Console.WriteLine("------------------------------------------\n");
      }
    }

    internal static string GetStringInput(string message)
    {      
      Console.WriteLine(message);

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

    internal static float GetFloatInput(string message)
    {
      Console.WriteLine(message);

      string numberInput = Console.ReadLine();

      if (numberInput == "main menu") GetUserInput();

      while (numberInput == null)
      {
        Console.WriteLine("\n\nInvalid number. Try again.\n\n");
        numberInput = Console.ReadLine();
      }

      float finalInput = float.Parse(numberInput);

      return finalInput;
    }

    internal static int GetIntInput(string message)
    {
      Console.WriteLine(message);

      string numberInput = Console.ReadLine();

      if (numberInput == "main menu") GetUserInput();

       while (!Int32.TryParse(numberInput, out _) || Convert.ToInt32(numberInput) < 0 || numberInput == null)
      {
        Console.WriteLine("\n\nInvalid number. Try again.\n\n");
        numberInput = Console.ReadLine();
      }

      int finalInput = Convert.ToInt32(numberInput);

      return finalInput;
    }

    internal static int GetIdInput()
    {
      Console.WriteLine("\n\nPlease type the ID of the inspection record you want to delete or type 0 to return to main menu.\n\n");

      string idInput = Console.ReadLine();

      if (idInput == "0") GetUserInput();

      while (!Int32.TryParse(idInput, out _) || Convert.ToInt32(idInput) <= 0 || idInput == null)
      {
        Console.WriteLine("\n\nInvalid ID. Try again.\n\n");
        idInput = Console.ReadLine();
      }

      int finalInput = Convert.ToInt32(idInput);

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