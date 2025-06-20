using Newtonsoft.Json;
using System.Collections.Generic;

namespace EmployeesLoaderPlugin.Services
{
  public class ApiResponse
  {
    [JsonProperty("users")]
    public List<ApiUser> Users { get; set; }
  }

  public class ApiUser
  {
    [JsonProperty("firstName")]
    public string FirstName { get; set; }

    [JsonProperty("lastName")]
    public string LastName { get; set; }

    [JsonProperty("phone")]
    public string Phone { get; set; }
  }
}