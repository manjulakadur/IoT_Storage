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
    public class TableStorageController : ControllerBase
    {
        private readonly ITableStorage _storageService;
        public TableStorageController(ITableStorage storageService)
        {
            _storageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
        }
        [HttpGet]
        [Route("GetItems")]
        public async Task<IActionResult> GetItems(string name, string id)
        {
            return Ok(await _storageService.GetEntityAsync(name, id));
        }

        [HttpPost]
        [Route("InsertItems")]
        public async Task<IActionResult> InsertItems([FromBody] GroceryItemEntity entity)
        {
            entity.PartitionKey = entity.Name.ToString();

            string Id = Guid.NewGuid().ToString();
            entity.Id = Id;
            entity.RowKey = Id;

            var createdEntity = await _storageService.UpsertEntityAsync(entity);
            return CreatedAtAction(nameof(GetItems), createdEntity);
        }

        [HttpPut]
        [Route("UpdateItem")]
        public async Task<IActionResult> UpdateItem([FromBody]GroceryItemEntity entity)
        {
            entity.PartitionKey = entity.Name.ToString();
            entity.RowKey = entity.Id;

            await _storageService.UpsertEntityAsync(entity);
            return NoContent();
        }

        [HttpDelete]
        [Route("DeleteItem")]
        public async Task<IActionResult> DeleteItem(string name, string id)
        {
            await _storageService.DeleteEntityAsync(name, id);
            return NoContent();
        }

    }
}