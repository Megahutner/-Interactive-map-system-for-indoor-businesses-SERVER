using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace UWEServer.Entities
{
    public class Block
    {
            #region Property
            [Key]
            public virtual int Id { get; set; }
            public virtual string? BlockId { get; set; }
            [Required]
            [MaxLength(100)]
            public virtual string? Name { get; set; }
            public virtual bool DelFlag { get; set; }

            public virtual DateTime? CreatedTime { get; set; }

            public virtual DateTime? UpdatedTime { get; set; }
            public virtual double Latitude { get; set; }
            public virtual double Longtitude { get; set; }
            public virtual double Width { get; set; }
            public virtual double Height { get; set; }
            public virtual double Front { get; set; }
            public virtual string? Color { get; set; }

            #endregion

            #region Relationship
            public virtual Zone? Zone { get; set; }
            public virtual int ZoneId { get; set; }
            public virtual Category? Category { get; set;}
            public virtual int? CategoryId { get; set; }
            public virtual int? ZoneLinkId { get; set; }
        #endregion
    }    
}
