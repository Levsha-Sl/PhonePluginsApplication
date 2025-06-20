using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PhoneApp.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace EmployeesLoaderPlugin.Services
{
  public class ApiDummyJson
  {
    static private HttpClient _client;

    private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

    static ApiDummyJson()
    {
      ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;

      _client = new HttpClient
      {
        BaseAddress = new Uri("https://dummyjson.com"),
        Timeout = TimeSpan.FromSeconds(30)
      };
      _client.DefaultRequestHeaders.Accept.Clear();
      _client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<List<EmployeesDTO>> GetAllEmployeesAsync(List<EmployeesDTO> allEmployees)
    {
      try
      {
        Type type = typeof(ApiUser);
        FieldInfo[] fields = type.GetFields(BindingFlags.Public);
        var attributesName = new List<string>();

        foreach (FieldInfo field in fields)
        {
          var attributes = field.GetCustomAttributes(typeof(JsonProperty), false);
          attributesName.Add(((JsonProperty)attributes[0]).PropertyName);
        }

        var requestUri = $"users?&select={string.Join(",", attributesName)}";

        Console.WriteLine($"We make a request to the API");
        var response = await GetWithRetryAsync(_client, requestUri);
        var jsonResponse = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(jsonResponse);

        if (apiResponse?.Users != null && apiResponse.Users.Count > 0)
        {
          foreach (var user in apiResponse.Users)
          {
            allEmployees.Add(new EmployeesDTO
            {
              Name = $"{user.LastName} {user.FirstName}",
              Phone = user.Phone
            });
          }
        }

        return allEmployees;
      }
      catch (Exception ex)
      {
        logger.Error($"Failed to make request to API: {ex.Message}");
        return null;
      }
    }

    public static async Task<HttpResponseMessage> GetWithRetryAsync(HttpClient client, string url, int retryCount = 3, int delayMilliseconds = 1000)
    {
      for (int i = 0; i < retryCount; i++)
      {
        try
        {
          HttpResponseMessage response = await client.GetAsync(url);
          response.EnsureSuccessStatusCode();
          return response;
        }
        catch (Exception ex)
        {
          Console.WriteLine($"Attempt {i + 1} failed: {ex.Message}");
          if (i == retryCount - 1)
          {
            throw;
          }
          await Task.Delay(delayMilliseconds);
        }
      }
      return null;
    }
  }
}