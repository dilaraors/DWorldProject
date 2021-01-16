using Amazon.DynamoDBv2.DataModel;

namespace DWorldProject.Data.Entities
{
    [DynamoDBTable("Image")]
    public class Image
    {
        [DynamoDBProperty("Id")]
        [DynamoDBHashKey]
        public int Id { get; set; }
        [DynamoDBProperty("HeaderImageURL")]
        public string HeaderImageURL { get; set; }
        [DynamoDBProperty("TopicId")]
        public int TopicId { get; set; }
        [DynamoDBProperty("ImageGallery")]
        public string[] ImageGallery { get; set; }
        [DynamoDBProperty("YouTubeVideoURL")]
        public string YouTubeVideoURL { get; set; }
        [DynamoDBProperty("HomePageImages")]
        public string[] HomePageImages { get; set; }
    }
}
