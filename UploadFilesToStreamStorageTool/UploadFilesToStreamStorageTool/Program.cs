using StreamStorage;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;

namespace UploadFilesToStreamStorageTool
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Uploading...");
            var storageProvider = StreamStorageServiceFactory.Create().Provider;
            for (var i = 1; i < 10; i++)
            {
                var transfer = new FileTransfer(ConfigurationManager.AppSettings["folder" + i.ToString()], storageProvider);
                transfer.Transfer().Wait();
            }
            Console.WriteLine("Upload files to stream storage complete! Press any key to exit.");
            Console.ReadKey();
        }
    }
}
