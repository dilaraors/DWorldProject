using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DWorldProject.Models.ViewModel
{
    public class SectionModel
    {
        public int? Id { get; set; }
        [Required]
        public string Name { get; set; }
        public List<TopicModel> Topics { get; set; }
        public virtual DateTime? CreatedDate { get; set; }
        public virtual DateTime? UpdatedDate { get; set; }
        public virtual DateTime? DeletedDate { get; set; }
    }
}
