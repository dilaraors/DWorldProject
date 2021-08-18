using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DWorldProject.Data.Entities
{
    public class BaseEntity
    {
        protected BaseEntity()
        {
            CreatedDate = DateTime.Now;
            IsActive = true;
            IsDeleted = false;
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public virtual int Id { get; set; }

        [Column] public virtual bool IsDeleted { get; set; } = false;
        [Column] public virtual bool IsActive { get; set; } = true;
        [Column]
        public virtual DateTime CreatedDate { get; set; } = DateTime.Now;
        [Column]
        public virtual DateTime? UpdatedDate { get; set; }
        [Column]
        public virtual DateTime? DeletedDate { get; set; }
    }
}
