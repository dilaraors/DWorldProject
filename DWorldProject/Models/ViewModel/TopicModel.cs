using System;
using System.ComponentModel.DataAnnotations;

namespace DWorldProject.Models.ViewModel
{
    public class TopicModel
    {
        public int? Id { get; set; }
        [Required]
        public int SectionId { get; set; }
        [Required]
        public string Name { get; set; }
        public virtual DateTime? CreatedDate { get; set; }
        public virtual DateTime? UpdatedDate { get; set; }
        public virtual DateTime? DeletedDate { get; set; }
    }
}
