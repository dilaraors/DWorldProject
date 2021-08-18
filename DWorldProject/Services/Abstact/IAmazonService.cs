using DWorldProject.Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using DWorldProject.Models.Request;

namespace DWorldProject.Services.Abstact
{
    public interface IAmazonService
    {
        Task<Amazon.DynamoDBv2.Model.ScanResponse> GetAmazonData(string tableName);
        Task<List<Image>> GetAmazonDataById(int id);
        Task<Amazon.DynamoDBv2.Model.PutItemResponse> AddAmazonData(string tableName, BlogPostRequestModel model, int id);
        Task<Amazon.DynamoDBv2.Model. UpdateItemResponse> UpdateAmazonData(string tableName, BlogPostRequestModel model, int id);
    }
}
