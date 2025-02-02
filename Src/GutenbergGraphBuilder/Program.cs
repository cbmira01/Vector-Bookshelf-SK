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
            string fusekiUpdateUrl = Environment.GetEnvironmentVariable("FUSEKI_UPDATE_URL") ?? "http://localhost:3030/dataset/update";
            string fusekiQueryUrl = Environment.GetEnvironmentVariable("FUSEKI_QUERY_URL") ?? "http://localhost:3030/dataset/query";
            string fusekiUsername = Environment.GetEnvironmentVariable("FUSEKI_USERNAME") ?? "yourUsername";
            string fusekiPassword = Environment.GetEnvironmentVariable("FUSEKI_PASSWORD") ?? "yourPassword";

            // Insert data
            string rdfData = @"
                PREFIX rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#>
                PREFIX dc: <http://purl.org/dc/elements/1.1/>
                PREFIX book: <http://example.org/book/>

                INSERT DATA { 
                    book:book1 rdf:type book:Book ;
                        dc:title ""The Great Gatsby"" ;
                        dc:creator ""F. Scott Fitzgerald"" ;
                        dc:date ""1925"" ;
                        book:genre ""Fiction"" .
                }";

            using HttpClient client = new HttpClient();
            var byteArray = Encoding.ASCII.GetBytes($"{fusekiUsername}:{fusekiPassword}");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            // Perform update
            var content = new StringContent(rdfData, Encoding.UTF8, "application/sparql-update");
            HttpResponseMessage response = await client.PostAsync(fusekiUpdateUrl, content);
            Console.WriteLine($"Update Response: {response.StatusCode}");

            // Query to verify data
            string queryString = @"
                PREFIX rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#>
                PREFIX dc: <http://purl.org/dc/elements/1.1/>
                PREFIX book: <http://example.org/book/>

                SELECT ?title ?author ?date ?genre
                WHERE {
                    book:book1 dc:title ?title ;
                              dc:creator ?author ;
                              dc:date ?date ;
                              book:genre ?genre .
                }";

            var queryContent = new StringContent(queryString, Encoding.UTF8, "application/sparql-query");
            response = await client.PostAsync(fusekiQueryUrl, queryContent);
            Console.WriteLine($"Query Response: {response.StatusCode}");
            Console.WriteLine(await response.Content.ReadAsStringAsync());
        }
    }
}
