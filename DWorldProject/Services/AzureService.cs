using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using DWorldProject.Data.Entities;
using DWorldProject.Services.Abstact;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.Model;
using DWorldProject.Models.Request;

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

        public async Task<Amazon.DynamoDBv2.Model.PutItemResponse> AddAzureData(string tableName, BlogPostRequestModel model, int id)
        {
            var request = new PutItemRequest
            {
                TableName = tableName,
                Item = new Dictionary<string, AttributeValue>()
                {
                    { "Id", new AttributeValue { N = id.ToString() }},
                    { "HeaderImageURL", new AttributeValue { S = model.HeaderImageUrl }},
                    { "TopicId", new AttributeValue { S = model.TopicId.ToString() }},
                    { "YouTubeVideoURL", new AttributeValue { S = model.YouTubeVideoURL }},
                    {
                        "ImageGallery",
                        new AttributeValue
                            { SS = new List<string>(model.ImageGallery)  }
                    }
                }
            };
            var images = Task.Run(async () => await _amazonDynamoDb.PutItemAsync(request));
            return await images;
        }



    }
}
