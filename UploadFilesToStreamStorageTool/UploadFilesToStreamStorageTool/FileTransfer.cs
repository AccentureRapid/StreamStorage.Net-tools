using StreamStorage;
using System;
using System.IO;
using System.Threading.Tasks;

namespace UploadFilesToStreamStorageTool
{
    /// <summary>
    /// 文件传输
    /// </summary>
    public class FileTransfer
    {
        private string _folderPath = "";
        private IStreamStorageProvider _storageProvider = null;

        /// <summary>
        /// 文件传输
        /// </summary>
        /// <param name="folderPath"></param>
        /// <param name="storageProvider"></param>
        public FileTransfer(string folderPath, IStreamStorageProvider storageProvider)
        {
            _folderPath = folderPath;
            _storageProvider = storageProvider;
        }

        /// <summary>
        /// 传输
        /// </summary>
        public async Task Transfer()
        {
            if (_storageProvider == null || String.IsNullOrEmpty(_folderPath) || !Directory.Exists(_folderPath)) return;

            DirectoryInfo folder = new DirectoryInfo(_folderPath);
            await TransferAsync("", folder);
        }

        private async Task TransferAsync(string parentObjectPath, DirectoryInfo folder)
        {
            string objectPath = parentObjectPath.TrimEnd('/') + "/" + folder.Name;
            foreach (var subDirectory in folder.EnumerateDirectories())
            {
                await TransferAsync(objectPath, subDirectory);
            }

            new Task(() =>
           {
               foreach (var file in folder.EnumerateFiles())
               {
                   string objectName = objectPath + "/" + file.Name;
                   try
                   {
                       using (var fs = file.OpenRead())
                       {
                           _storageProvider.PutObject(objectName, fs, true);
                           fs.Flush();
                           fs.Close();
                       }
                   }
                   catch (Exception ex)
                   {
                       Console.WriteLine(String.Format("Error:File \"{0}\" upload to oss error!\r\n{1}\r\n{2}\r\n", file.FullName, ex.Message + ex.InnerException != null ? ex.InnerException.Message : "", ex.StackTrace));
                   }

               }
           }).RunSynchronously();
        }
    }
}
