using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
      logger.Info("Starting Viewer");
      logger.Info("Type q or quit to exit");
      logger.Info("Available commands: list, add, del, update");

      var employeesList = args.Cast<EmployeesDTO>().ToList();

      string command = "";

      while (!command.ToLower().Contains("quit") && !command.ToLower().Trim().Equals("q"))
      {
        Console.Write("> ");
        command = Console.ReadLine();

        switch (command)
        {
          case "list":
            int index = 0;
            foreach (var employee in employeesList)
            {
              Console.WriteLine($"{index + 1,-3} Name: {employee.Name,-20} | Phone: {employee.Phone}");
              ++index;
            }
            break;
          case "add":
            Console.Write("Name: ");
            var newEmployee = new EmployeesDTO() { Name = Console.ReadLine() };
            Console.Write("Phone: ");
            try
            {
              newEmployee.Phone = Console.ReadLine();
            }
            catch (ArgumentNullException ex)
            {
              Console.WriteLine(ex.Message);
              Console.WriteLine($"Update {newEmployee.Name}");
            }
            employeesList.Add(newEmployee);
            Console.WriteLine($"{newEmployee.Name} added to employees");
            break;
          case "del":
            Console.Write("Index of employee to delete: ");
            int indexToDelete;
            if (!Int32.TryParse(Console.ReadLine(), out indexToDelete))
            {
              logger.Error("Not an index or not an int value!");
            }
            else
            {
              --indexToDelete;
              if (indexToDelete > 0 && indexToDelete < employeesList.Count())
              {
                employeesList.RemoveAt(indexToDelete);
              }
            }
            break;
          case "update":
            Console.Write("Index of employee to update: ");
            int indexToUpdate;
            if (!Int32.TryParse(Console.ReadLine(), out indexToUpdate))
            {
              logger.Error("Not an index or not an int value!");
            }
            else
            {
              --indexToUpdate;
              if (indexToUpdate > 0 && indexToUpdate < employeesList.Count())
              {
                Console.Write("Name: ");
                employeesList[indexToUpdate].Name = Console.ReadLine();
                Console.Write("Phone: ");
                try
                {
                  employeesList[indexToUpdate].Phone = Console.ReadLine();
                }
                catch (ArgumentNullException ex)
                {
                  Console.WriteLine(ex.Message);
                  Console.WriteLine($"Update {employeesList[indexToUpdate].Name} again");
                }
                Console.WriteLine($"{employeesList[indexToUpdate].Name} update");
              }
            }
            break;
        }

        Console.WriteLine("");
      }

      return employeesList.Cast<DataTransferObject>();
    }
  }
}
