using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using DWorldProject.Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DWorldProject.Repositories
{
    public class DynamoDbRepository// : IDynamoDbRepository
    {
        private readonly DynamoDBContext _context;

        public DynamoDbRepository(IAmazonDynamoDB dynamoDbClient)
        {
            _context = new DynamoDBContext(dynamoDbClient);
        }

        public async Task AddImage(Image image)
        {
            await _context.SaveAsync(image);
        }

        public async Task DeleteImage(Image image)
        {
            await _context.DeleteAsync(image);
        }

        public async Task<IEnumerable<Image>> GetAllItems()
        {
            return await _context.ScanAsync<Image>(
               new List<ScanCondition>()).GetRemainingAsync();
        }

        public async Task<IEnumerable<Image>> GetUsersItemsByName(int id, string name)
        {
            var config = new DynamoDBOperationConfig
            {
                QueryFilter = new List<ScanCondition>
        {
            new ScanCondition("Name", ScanOperator.BeginsWith, name)
        }
            };

            return await _context.QueryAsync<Image>(id, config).GetRemainingAsync();
        }
    }
}
