using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace GutenbergGraphBuilder
{
    class Program
    {
        static async Task Main()
        {
            string fusekiUpdateUrl = 
                Environment.GetEnvironmentVariable("FUSEKI_UPDATE_URL")
                ?? "http://localhost:3030/dataset/update";

            // Retrieve credentials from environment variables (or replace with your actual credentials)
            string fusekiUsername = Environment.GetEnvironmentVariable("FUSEKI_USERNAME") ?? "yourUsername";
            string fusekiPassword = Environment.GetEnvironmentVariable("FUSEKI_PASSWORD") ?? "yourPassword";

            string rdfData = @"
                PREFIX ex: <http://example.org/>
                INSERT DATA { ex:subject ex:predicate ex:object . }
            ";

            using HttpClient client = new HttpClient();
            // Setup Basic Authentication header
            var byteArray = Encoding.ASCII.GetBytes($"{fusekiUsername}:{fusekiPassword}");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            var content = new StringContent(rdfData, Encoding.UTF8, "application/sparql-update");

            HttpResponseMessage response = await client.PostAsync(fusekiUpdateUrl, content);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Fuseki credentials accepted.");
            }
            else
            {
                Console.WriteLine("Fuseki credentials rejected.");
            }

            Console.WriteLine($"Response: {response.StatusCode}");
            Console.WriteLine(await response.Content.ReadAsStringAsync());
        }
    }
}
