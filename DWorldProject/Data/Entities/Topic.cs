using DWorldProject.Enums;

namespace DWorldProject.Data.Entities
{
    public class Topic : BaseEntity
    {
        public string Name { get; set; }
        public Section Section { get; set; }

    }
}
