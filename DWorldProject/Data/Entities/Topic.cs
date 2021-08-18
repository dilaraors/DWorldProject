using System.ComponentModel.DataAnnotations;

namespace DWorldProject.Data.Entities
{
    public class Topic : BaseEntity
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public int SectionId { get; set; }
        public Section Section { get; set; }

    }
}
