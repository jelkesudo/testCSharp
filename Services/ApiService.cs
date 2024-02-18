using Newtonsoft.Json;
using testCSharp.Extenstions;
using testCSharp.Models;

namespace testCSharp.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl;

        public ApiService(HttpClient httpClient, string apiUrl)
        {
            _httpClient = httpClient;
            _apiUrl = apiUrl;
        }

        public async Task<List<Employee>> GetEmployees(string endpoint, string key)
        {
            using (var httpClient = new HttpClient())
            {
                var json = await _httpClient.GetStringAsync($"{_apiUrl}{endpoint}{key}");
                return JsonConvert.DeserializeObject<List<Employee>>(json);
            }
        }

        public Dictionary<string, long> SortWorkingHoursByEmployee(List<Employee> employeesList)
        {
            Dictionary<string, long> employeesWithWorkHours = new Dictionary<string, long>();

            foreach (Employee e in employeesList)
            {
                if (e.DeletedOn != null) continue;

                if (String.IsNullOrEmpty(e.EmployeeName)) continue;

                long workTimeInMiliseconds = DateTimeExtensions.CreateTimeStampFromDateTime(e.EndTimeUtc) - DateTimeExtensions.CreateTimeStampFromDateTime(e.StarTimeUtc);

                long workTimeInHours = workTimeInMiliseconds / 3600000;

                if (workTimeInHours < 0) continue;

                if (!employeesWithWorkHours.ContainsKey(e.EmployeeName))
                {
                    employeesWithWorkHours.Add(e.EmployeeName, 0);
                }

                employeesWithWorkHours[e.EmployeeName] += workTimeInHours;
            }

            return employeesWithWorkHours;
        }
    }
}
