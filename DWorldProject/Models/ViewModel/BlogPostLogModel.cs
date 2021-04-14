namespace DWorldProject.Models.ViewModel
{
    public class BlogPostLogModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int? SectionId { get; set; }
        public int? TopicId { get; set; }
        public int? UserId { get; set; }
        public int OperationType { get; set; }
    }
}
