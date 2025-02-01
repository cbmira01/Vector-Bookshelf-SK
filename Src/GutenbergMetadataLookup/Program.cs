using System.Text;
using SharpCompress.Archives.Zip;
using SharpCompress.Readers.Tar;

/*
 * 
 * This program is example code for extracting Gutenberg Project RDF metadata.
 * 
 * The latest Gutenberg Project metadata can be found at https://www.gutenberg.org/cache/epub/feeds/
 * 
 */

public static class Program
{
    public static void Main()
    {
        string tarZipPath = "Resources\\rdf-files.tar.zip";
        int itemId = 1; // Example item ID

        string? rdfContent = GetRdfContent(tarZipPath, itemId);

        if (rdfContent != null)
        {

            Console.WriteLine($"RDF Content for Item {itemId}:\n" + rdfContent);
        }
        else
        {
            Console.WriteLine("Item not found.");
        }
    }

    public static string? GetRdfContent(string tarZipPath, int itemId)
    {
        try
        {
            // 1) Open the .zip stream
            using var zipStream = File.OpenRead(tarZipPath);
            using var zipArchive = ZipArchive.Open(zipStream);

            // 2) Find the .tar entry inside the zip
            var tarEntry = zipArchive
                .Entries
                .FirstOrDefault(e => e.Key.EndsWith(".tar", StringComparison.OrdinalIgnoreCase) && !e.IsDirectory);

            if (tarEntry == null)
            {
                Console.WriteLine("No .tar file found in the ZIP archive.");
                return null;
            }

            // 3) Open a stream for that tar entry
            using var tarStream = tarEntry.OpenEntryStream();

            // 4) Use TarReader (streaming) to find the RDF entry
            using var tarReader = TarReader.Open(tarStream); // ReaderOptions optional

            string rdfPath = $"cache/epub/{itemId}/pg{itemId}.rdf";

            // 5) Enumerate tar entries until we find pg{itemId}.rdf
            while (tarReader.MoveToNextEntry())
            {

                //Console.WriteLine($"Key: {tarReader.Entry.Key}, " +
                //  $"Size: {tarReader.Entry.Size}, " +
                //  $"IsDirectory: {tarReader.Entry.IsDirectory}");

                // Compare full path inside the tar
                if (!tarReader.Entry.IsDirectory && tarReader.Entry.Key == rdfPath)
                {
                    using var reader = new StreamReader(tarReader.OpenEntryStream(), Encoding.UTF8);
                    return reader.ReadToEnd();
                }
            }

            // If we got here, that RDF wasn't found
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetRdfContent: {ex.Message}");
            return null;
        }
    }
}
