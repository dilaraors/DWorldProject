using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using DWorldProject.Data.Entities;
using DWorldProject.Services.Abstact;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DWorldProject.Services
{
    public class AzureService : IAzureService
    {

        private readonly IAmazonDynamoDB _amazonDynamoDb;
        private readonly DynamoDBContext _context;

        public AzureService(IAmazonDynamoDB amazonDynamoDb)
        {
            _amazonDynamoDb = amazonDynamoDb;
            _context = new DynamoDBContext(amazonDynamoDb);
        }

        public async Task<Amazon.DynamoDBv2.Model.ScanResponse> GetAzureData(string tableName)
        {
            List<string> attributesToGet = new List<string>();
            var images = Task.Run(async () => await _amazonDynamoDb.ScanAsync(tableName, attributesToGet));
            return await images;
        }

        public async Task<List<Image>> GetAzureDataById(int id)
        {
            var scanConditions = new List<ScanCondition>();
            scanConditions.Add(new ScanCondition("Id", ScanOperator.Equal, id));

            var response = Task.Run(async () => await _context.ScanAsync<Image>(scanConditions, null).GetRemainingAsync());
            return await response;
        }

    }
}
