using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommandLine;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Auth;
using Microsoft.Azure.Storage.Blob;

namespace AzureBlobMimeTypeReset
{
    class Options
    {
        [Option('n', Required = true, HelpText = "Storage Account Name")]
        public string AccountName { get; set; }

        [Option('k', Required = true, HelpText = "Storage Account Key")]
        public string AccountKey { get; set; }

        [Option('c', Required = true, HelpText = "Blob Container Name")]
        public string ContainerName { get; set; }
    }

    class Program
    {
        public static Options Options { get; set; }

        public static CloudBlobContainer BlobContainer { get; set; }

        static async Task Main(string[] args)
        {
            var parserResult = Parser.Default.ParseArguments<Options>(args);
            if (parserResult.Tag == ParserResultType.Parsed)
            {
                Options = ((Parsed<Options>)parserResult).Value;

                // 1. Get Azure Blob Files
                WriteMessage($"[{DateTime.Now}] Finding Files on Azure Blob Storage...");
                BlobContainer = GetBlobContainer();
                if (null == BlobContainer)
                {
                    WriteMessage("ERROR: Can not get BlobContainer.", ConsoleColor.Red);
                    Console.ReadKey();
                    return;
                }

                // 2. Update Mime Type
                var pvd = new FileExtensionContentTypeProvider();
                WriteMessage($"[{DateTime.Now}] Updating Mime Type...");
                int affectedFilesCount = 0;

                foreach (var blob in BlobContainer.ListBlobs().OfType<CloudBlockBlob>())
                {
                    string extension = Path.GetExtension(blob.Uri.AbsoluteUri).ToLower();
                    bool isKnownType = pvd.TryGetContentType(extension, out string mimeType);
                    if (isKnownType)
                    {
                        if (TrySetContentType(blob, mimeType) != null)
                        {
                            WriteMessage($"[{DateTime.Now}] Updating {blob.Uri.AbsoluteUri} => {mimeType}");
                            await blob.SetPropertiesAsync();
                            affectedFilesCount++;
                        }
                    }
                }

                WriteMessage($"[{DateTime.Now}] Update completed, {affectedFilesCount} file(s) updated.");
            }

            Console.ReadKey();
        }

        private static CloudBlockBlob TrySetContentType(CloudBlockBlob blob, string contentType)
        {
            if (blob.Properties.ContentType.ToLower() != contentType)
            {
                blob.Properties.ContentType = contentType;
                return blob;
            }
            return null;
        }

        private static void WriteMessage(string message, ConsoleColor color = ConsoleColor.White, bool resetColor = true)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            if (resetColor)
            {
                Console.ResetColor();
            }
        }

        private static CloudBlobContainer GetBlobContainer()
        {
            CloudStorageAccount storageAccount = new CloudStorageAccount(new StorageCredentials(Options.AccountName, Options.AccountKey), true);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(Options.ContainerName);
            return container;
        }
    }
}
