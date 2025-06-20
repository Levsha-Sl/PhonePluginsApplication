using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EmployeesLoaderPlugin.Services;
using Newtonsoft.Json;
using PhoneApp.Domain;
using PhoneApp.Domain.Attributes;
using PhoneApp.Domain.DTO;
using PhoneApp.Domain.Interfaces;

namespace EmployeesLoaderPlugin
{

  [Author(Name = "Ivan Petrov")]
  public class Plugin : IPluggable
  {
    private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

    public IEnumerable<DataTransferObject> Run(IEnumerable<DataTransferObject> args)
    {
      logger.Info("Loading employees");

      var employeesList = new List<EmployeesDTO>();
      var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Employees.json");

      string command = "";

      Console.WriteLine("Download list from Employees.json file or API? file/api");
      while (employeesList.Count == 0)
      {
        Console.Write("> ");
        command = Console.ReadLine();

        switch (command)
        {
          case "file":
            if (File.Exists(filePath))
            {
              try
              {
                employeesList = JsonConvert.DeserializeObject<List<EmployeesDTO>>(filePath) ?? employeesList;
                if (employeesList.Count == 0)
                {
                  Console.WriteLine($"" +
                    $"Employees.json is empty or the data is incorrect." +
                    $"\nFill in the file or check the input for correctness and retry the request." +
                    $"\nOR Download list from API? file/api");
                }
              }
              catch (JsonReaderException)
              {
                Console.WriteLine($"" +
                  $"Employees.json is empty or the data is incorrect." +
                  $"\nFill in the file or check the input for correctness and retry the request." +
                  $"\nOR Download list from API? file/api");
              }
            }
            else
            {
              Console.WriteLine($"" +
                $"Employees.json is not in the application directory." +
                $"\nPlace the file in the directory and retry the request." +
                $"\nOR Download list from API? file/api");
            }
            break;
          case "api":
            var apiClient = new ApiDummyJson();
            employeesList = apiClient.GetAllEmployeesAsync(employeesList).GetAwaiter().GetResult() ?? employeesList;
            if (employeesList.Count == 0)
            {
              Console.WriteLine("API not available, using default employee list or file. st/file");
            }
            break;
          case "st":
            employeesList = JsonConvert.DeserializeObject<List<EmployeesDTO>>(EmployeesLoaderPlugin.Properties.Resources.EmployeesJson);
            break;
        }
      }

      logger.Info($"Loaded {employeesList.Count()} employees");
      return employeesList.Cast<DataTransferObject>();
    }
  }
}
