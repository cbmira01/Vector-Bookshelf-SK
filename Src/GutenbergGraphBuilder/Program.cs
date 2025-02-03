using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using SharpCompress.Archives.Zip;
using SharpCompress.Readers.Tar;

namespace GutenbergGraphBuilder
{
    class Program
    {
        private static string _fusekiDataUrl = null!;
        private static string _fusekiUsername = null!;
        private static string _fusekiPassword = null!;

        static void Main(string[] args)
        {

            Console.WriteLine("Waiting for debugger to attach...");
            while (!Debugger.IsAttached)
            {
                Thread.Sleep(100);
            }
            Console.WriteLine("Debugger is attached!");

            // Get the Graph Store Protocol endpoint URL for loading data
            _fusekiDataUrl = Environment.GetEnvironmentVariable("FUSEKI_DATA_URL")
                ?? "http://fuseki:3030/dataset/data?graph=http://projectgutenberg.org/graph/ebooks";

            _fusekiUsername = Environment.GetEnvironmentVariable("FUSEKI_USERNAME") ?? "yourUsername";
            _fusekiPassword = Environment.GetEnvironmentVariable("FUSEKI_PASSWORD") ?? "yourPassword";

            string tarZipPath = Path.Combine("/app/Resources", "rdf-files.tar.zip");
            ProcessRdfFiles(tarZipPath);
        }

        public static void ProcessRdfFiles(string tarZipPath)
        {
            try
            {
                using var zipStream = File.OpenRead(tarZipPath);
                using var zipArchive = ZipArchive.Open(zipStream);

                var tarEntry = zipArchive.Entries
                    .Where(e => !e.IsDirectory && e.Key is not null)
                    .FirstOrDefault(e => e.Key!.EndsWith(".tar", StringComparison.OrdinalIgnoreCase));

                if (tarEntry == null)
                {
                    Console.WriteLine("No .tar file found in the ZIP archive.");
                    return;
                }

                using var tarStream = tarEntry.OpenEntryStream();
                using var tarReader = TarReader.Open(tarStream);

                int numFiles = 0;
                long sumSize = 0;

                while (tarReader.MoveToNextEntry())
                {
                    if (tarReader.Entry != null && !tarReader.Entry.IsDirectory)
                    {
                        Console.WriteLine($"Key: {tarReader.Entry.Key}, Size: {tarReader.Entry.Size}");
                        numFiles++;

                        using var entryStream = tarReader.OpenEntryStream();
                        using var reader = new StreamReader(entryStream, Encoding.UTF8);

                        StringBuilder rdfContent = new StringBuilder();
                        char[] buffer = new char[8192];
                        int bytesRead;
                        long totalBytesRead = 0;

                        while ((bytesRead = reader.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            rdfContent.Append(buffer, 0, bytesRead);
                            totalBytesRead += bytesRead;
                        }

                        //Console.WriteLine($"First 500 chars:\n{rdfContent.ToString()[..Math.Min(500, rdfContent.Length)]}");
                        //Console.WriteLine($"Fully read {totalBytesRead} bytes from {tarReader.Entry.Key}");
                        //Console.WriteLine("Press Enter to continue...");
                        //Console.ReadLine();

                        sumSize += tarReader.Entry.Size;

                        bool isSuccess = SendRdfToGraphStoreAsync(rdfContent.ToString())
                            .GetAwaiter().GetResult();

                        if (isSuccess)
                            Console.WriteLine("RDF data successfully sent...");
                        else
                            Console.WriteLine("Failed to send RDF data.");

                        if (numFiles >= 10) { return; } // limit for testing
                    }
                }

                Console.WriteLine($"All RDF files processed, {numFiles} files, {sumSize} bytes.");
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetRdfContent: {ex.Message}");
                return;
            }
        }

        static async Task<bool> SendRdfToGraphStoreAsync(string rdfXmlContent)
        {
            try
            {
                using HttpClient client = new HttpClient();
                var byteArray = Encoding.ASCII.GetBytes($"{_fusekiUsername}:{_fusekiPassword}");
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                // Create content with RDF/XML MIME type
                var content = new StringContent(rdfXmlContent, Encoding.UTF8, "application/rdf+xml");

                // Use PUT for bulk upload
                HttpResponseMessage response = await client.PutAsync(_fusekiDataUrl, content);

                // Log response details
                Console.WriteLine($"Response: {response.StatusCode}");
                Console.WriteLine(await response.Content.ReadAsStringAsync());

                // Consider 2xx status codes as success
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SendRdfToGraphStoreAsync: {ex.Message}");
                return false;
            }
        }
    }
}
