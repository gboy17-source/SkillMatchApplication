using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System;

namespace SkillMatchApplication.Services
{
    internal class ApiClient
    {
        private readonly HttpClient client;

        public ApiClient()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri(Environment.GetEnvironmentVariable("API_URL"));
        }

        public async Task<string> PostJson(string endpoint, string json)
        {
            // Prepare the JSON content
            var content = new StringContent(json ?? string.Empty, Encoding.UTF8, "application/json");

            // Send the POST request
            var response = await client.PostAsync(endpoint, content);
            var res = await response.Content.ReadAsStringAsync();

            // Check for success
            if (!response.IsSuccessStatusCode) throw new Exception(res);

            return res;
        }

        public async Task<string> GetJson(string endpoint)
        {
            // Send the GET request
            var response = await client.GetAsync(endpoint);
            var res = await response.Content.ReadAsStringAsync();

            // Check for success
            if (!response.IsSuccessStatusCode) throw new Exception(res);
            return res;
        }

        public async Task<string> PutJson(string endpoint, string json)
        {
            HttpContent content = null;
            if (json != null)
                content = new StringContent(json, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Put, endpoint)
            {
                Content = content
            };

            var response = await client.SendAsync(request);
            var res = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode) throw new Exception(res);
            return res;
        }

        public void SetJwt(string token)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        public void ClearJwt()
        {
            client.DefaultRequestHeaders.Authorization = null;
        }
    }
}