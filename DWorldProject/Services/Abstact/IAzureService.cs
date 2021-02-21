using DWorldProject.Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DWorldProject.Services.Abstact
{
    public interface IAzureService
    {
        Task<Amazon.DynamoDBv2.Model.ScanResponse> GetAzureData(string tableName);
        Task<List<Image>> GetAzureDataById(int id);
    }
}
