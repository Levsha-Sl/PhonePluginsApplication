using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PhoneApp.Domain.Attributes;
using PhoneApp.Domain.DTO;
using PhoneApp.Domain.Interfaces;

namespace EmployessUploaderPlugin
{
  [Author(Name = "Shirnin Vyacheslav")]
  public class Plugin : IPluggable
  {
    private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

    public IEnumerable<DataTransferObject> Run(IEnumerable<DataTransferObject> args)
    {
      logger.Info("Uploading employees");

      var employeesList = args.Cast<EmployeesDTO>().ToList();

      try
      {
        var employeesJson = JsonConvert.SerializeObject(employeesList, Formatting.Indented);
        File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), "Employees.json"), employeesJson);
        logger.Info($"List employees saved");
      }
      catch (Exception ex)
      {
        logger.Error($"List employees not saved: {ex.Message}");
      }

      return employeesList.Cast<DataTransferObject>();
    }
  }
}
