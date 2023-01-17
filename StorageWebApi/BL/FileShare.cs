using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;
using Microsoft.Extensions.Configuration;
using StorageWebApi.Services;

namespace StorageWebApi.BL
{
    public class FileShare : IFileShareStorage
    {
        private readonly IConfiguration _configuration;
        public FileShare(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<string> FileShareAsync(string directoryName, string filename, string shareName, Stream fileContent)
        {
            // Get the connection string from app settings
            string connectionString = _configuration.GetValue<string>("MyConfig:StorageConnection");
            // Get a reference to a share and then create it
            ShareClient share = new ShareClient(connectionString, shareName);
            await share.CreateIfNotExistsAsync();

            if (await share.ExistsAsync())
            {
                // Get a reference to a directory and create it
                ShareDirectoryClient directory = share.GetDirectoryClient(directoryName);
                await directory.CreateIfNotExistsAsync();

                if (await directory.ExistsAsync())
                {
                    // Get a reference to a file and upload it
                    ShareFileClient file = directory.GetFileClient(filename);
                    if (await file.ExistsAsync())
                    {
                        file.Create(fileContent.Length);
                        await file.UploadRangeAsync(
                            new HttpRange(0, fileContent.Length),
                            fileContent);
                    }
                }
                else
                {
                    throw new InvalidOperationException("No directory Found");
                }
            }
            else
            {
                throw new InvalidOperationException("No share Client Found");
            }
            return "File Shared SuccessFully";
        }
       /* public async Task CreateShareAsync(string shareNamePath)
        {
            // Get the connection string from app settings
            string connectionString = _configuration.GetValue<string>("MyConfig:StorageConnection");

            // Instantiate a ShareClient which will be used to create and manipulate the file share
            string dirName = "ManjulaFileStorage";
            string shareName = Path.GetFileName(shareNamePath);
            string currentDirectory = Path.GetDirectoryName(shareNamePath);
            string sPath = Path.GetFullPath(currentDirectory);
            
            ShareClient share = new ShareClient(connectionString, shareName);
            ShareDirectoryClient directory = share.GetDirectoryClient(dirName);
            // Get a reference to our file and upload it to azure
            ShareFileClient file = directory.GetFileClient(shareName);
            using (FileStream stream = File.OpenRead(shareNamePath))
            {
                file.Create(stream.Length);
               // await share.CreateIfNotExistsAsync();
                file.UploadRange(
                    new HttpRange(0, stream.Length),
                    stream);
            }
            
            // Create the share if it doesn't already exist
           // await share.CreateIfNotExistsAsync();
            
            // Ensure that the share exists
           /* if (await share.ExistsAsync())
            {
               
                // Get a reference to the sample directory
                ShareDirectoryClient directory = share.GetDirectoryClient("CustomLogs");

                // Create the directory if it doesn't already exist
                await directory.CreateIfNotExistsAsync();

                // Ensure that the directory exists
                if (await directory.ExistsAsync())
                {
                    // Get a reference to a file object
                    ShareFileClient file = directory.GetFileClient("Log1.txt");

                    // Ensure that the file exists
                    if (await file.ExistsAsync())
                    {
                        // Download the file
                        ShareFileDownloadInfo download = await file.DownloadAsync();

                        // Save the data to a local file, overwrite if the file already exists
                        using (FileStream stream = File.OpenWrite(@"downloadedLog1.txt"))
                        {
                            await download.Content.CopyToAsync(stream);
                            await stream.FlushAsync();
                            stream.Close();
                        }
                    }
                }*/
           // }           
            
        public async Task<List<string>> GetAllFileShares()
        {
            string connectionString = _configuration.GetValue<string>("MyConfig:StorageConnection");
            
            ShareServiceClient shareServiceClient = new ShareServiceClient(connectionString);
            
            List<string> files = new List<string>();
            
            // Display each share and the snapshots on each share
            foreach (ShareItem item in shareServiceClient.GetShares(ShareTraits.All, ShareStates.Snapshots))
            {
                if (null != item.Name) 
                {
                    files.Add(item.Name);
                }
            }
            return files;
        }

        public async Task DeleteSnapshotAsync(string shareName)
        {
            // Get the connection string from app settings
            string connectionString = _configuration.GetValue<string>("MyConfig:StorageConnection");

            // Instatiate a ShareServiceClient
            ShareServiceClient shareService = new ShareServiceClient(connectionString);

            // Get a ShareClient
            ShareClient share = shareService.GetShareClient(shareName);            
            await share.DeleteAsync();

            // Get a ShareClient that points to a snapshot
            //ShareClient snapshotShare = share.WithSnapshot(snapshotTime);
            // Delete the snapshot
            //await snapshotShare.DeleteIfExistsAsync();
            
        }
    }
}
