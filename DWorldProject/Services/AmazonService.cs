using System;
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
    public class AmazonService : IAmazonService
    {

        private readonly IAmazonDynamoDB _amazonDynamoDb;
        private readonly DynamoDBContext _context;

        public AmazonService(IAmazonDynamoDB amazonDynamoDb)
        {
            _amazonDynamoDb = amazonDynamoDb;
            _context = new DynamoDBContext(amazonDynamoDb);
        }

        public async Task<Amazon.DynamoDBv2.Model.ScanResponse> GetAmazonData(string tableName)
        {
            List<string> attributesToGet = new List<string>();
            var images = Task.Run(async () => await _amazonDynamoDb.ScanAsync(tableName, attributesToGet));
            return await images;
        }

        public async Task<List<Image>> GetAmazonDataById(int id)
        {
            var scanConditions = new List<ScanCondition>();
            scanConditions.Add(new ScanCondition("Id", ScanOperator.Equal, id));

            var response = await _context.ScanAsync<Image>(scanConditions, null).GetRemainingAsync();
            return response;
        }

        public async Task<Amazon.DynamoDBv2.Model.PutItemResponse> AddAmazonData(string tableName, BlogPostRequestModel model, int id)
        {
            var request = new PutItemRequest
            {
                TableName = tableName,
                Item = new Dictionary<string, AttributeValue>()
                {
                    { "Id", new AttributeValue { N = id.ToString() }},
                }
            };
            if (model.HeaderImageURL != null)
            {
                request.Item.Add(nameof(model.HeaderImageURL), new AttributeValue { S = model.HeaderImageURL });
            }
            if (model.TopicId != null)
            {
                request.Item.Add(nameof(model.TopicId), new AttributeValue { S = model.TopicId.ToString()});
            }
            if (model.YouTubeVideoURL != null)
            {
                request.Item.Add(nameof(model.YouTubeVideoURL), new AttributeValue { S = model.YouTubeVideoURL});
            }
            if (model.ImageGallery.Count > 0)
            {
                request.Item.Add(nameof(model.ImageGallery), new AttributeValue
                    { SS = new List<string>(model.ImageGallery) });
            }
            try
            {
                var images = await _amazonDynamoDb.PutItemAsync(request);
                return images;
            }
            catch (Exception e)
            {
                Console.Write(e);
                return null;
            }
            
        }

        public async Task<Amazon.DynamoDBv2.Model.UpdateItemResponse> UpdateAmazonData(string tableName, BlogPostRequestModel model, int id)
        {
            var request = new UpdateItemRequest()
            {
                Key = new Dictionary<string, AttributeValue>() { { "Id", new AttributeValue { N = id.ToString() } } },
                //ExpressionAttributeNames = new Dictionary<string, string>()
                //{
                //    {"#HI", nameof(model.HeaderImageURL)},
                //    {"#IG", nameof(model.ImageGallery)},
                //    {"#YV", nameof(model.YouTubeVideoURL)}
                //},
                //ExpressionAttributeValues = new Dictionary<string, AttributeValue>()
                //{
                //    {":header",new AttributeValue {
                //        S = model.HeaderImageURL
                //    }},
                //    {":gallery",new AttributeValue {
                //        SS = new List<string>(model.ImageGallery)
                //    }},
                //    {":youtube",new AttributeValue {
                //            S = model.YouTubeVideoURL
                //        }
                //    }
                //},
                //UpdateExpression = "SET #HI = :header, #IG = :gallery, #YV = :youtube ",

                TableName = tableName,
                ReturnValues = "ALL_NEW" // Give me all attributes of the updated item.
            };

            if (model.HeaderImageURL != null)
            {
                request.ExpressionAttributeNames.Add("#HI", nameof(model.HeaderImageURL));
                request.ExpressionAttributeValues.Add(":header", new AttributeValue
                {
                    S = model.HeaderImageURL
                });
                request.UpdateExpression = "SET #HI = :header";
            }
            if (model.ImageGallery != null)
            {
                request.ExpressionAttributeNames.Add("#IG", nameof(model.ImageGallery));
                request.ExpressionAttributeValues.Add(":gallery", new AttributeValue
                {
                    SS = new List<string>(model.ImageGallery)
                });
                request.UpdateExpression = request.UpdateExpression + ", #IG = :gallery ";
            }
            if (model.YouTubeVideoURL != null)
            {
                request.ExpressionAttributeNames.Add("#YV", nameof(model.YouTubeVideoURL));
                request.ExpressionAttributeValues.Add(":youtube", new AttributeValue
                {
                    S = model.YouTubeVideoURL
                });
                request.UpdateExpression = request.UpdateExpression + ", #YV = :youtube ";
            }
            try
            {
                var images = await _amazonDynamoDb.UpdateItemAsync(request);
                return images;
            }
            catch (Exception e)
            {
                Console.Write(e);
                return null;
            }

        }



    }
}
