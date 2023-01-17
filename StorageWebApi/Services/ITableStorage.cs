using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StorageWebApi.BL;

namespace StorageWebApi.Services
{
    public interface ITableStorage
    {
        Task<GroceryItemEntity> GetEntityAsync(string name,string id);
        Task<GroceryItemEntity> UpsertEntityAsync(GroceryItemEntity entity);
        Task DeleteEntityAsync(string name,string id);
    }
}
