using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DWorldProject.Data.Entities
{
    public class Section : BaseEntity
    {
        [Required]
        public string Name { get; set; }
        public ICollection<Topic> Topics { get; set; }

    }
}
