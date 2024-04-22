using System.ComponentModel.DataAnnotations;

namespace UWEServer.Entities
{
    public class Category
    {
        #region Property
        [Key]
        public virtual int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public virtual string? Name { get; set; }
        public virtual bool DelFlag { get; set; }

        public virtual DateTime? CreatedTime { get; set; }

        public virtual DateTime? UpdatedTime { get; set; }
        #endregion

    }
}
