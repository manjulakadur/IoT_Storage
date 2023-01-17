using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StorageWebApi.BL;
using StorageWebApi.Services;

namespace StorageWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileShareController : ControllerBase
    {
        private readonly IFileShareStorage _fileshareService;
        public FileShareController(IFileShareStorage fileshareService)
        {
            _fileshareService = fileshareService ?? throw new ArgumentNullException(nameof(fileshareService));
        }

        [HttpGet("GetAllFiles")]
        public async Task<List<string>> GetAllFiles()
        {
            return await _fileshareService.GetAllFileShares();
        }

        [HttpPost("Create")]
        public async Task<bool> Create(string fileSharePath)
        {
             await _fileshareService.CreateShareAsync(fileSharePath);
            return true;
        }

        [HttpDelete("Delete")]
        public async Task<bool> Delete(string shareName)
        {
            await _fileshareService.DeleteSnapshotAsync(shareName);
            return true;
        }

    }
}