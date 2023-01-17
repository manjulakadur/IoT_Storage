using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace StorageWebApi.Services
{
    public interface IFileShareStorage
    {
        Task<string> FileShareAsync(string directoryName, string filename, string shareName, Stream fileContent);
        //Task CreateShareAsync(string shareNamepath);
        Task<List<string>> GetAllFileShares();
        Task DeleteSnapshotAsync(string share);
    }
}
