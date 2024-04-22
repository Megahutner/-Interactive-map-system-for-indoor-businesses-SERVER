using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Text;

namespace UWEServer.Entities
{
    public class Zone
    {
        #region Property
        [Key]
        public virtual int Id { get; set; }

        public virtual string? ZoneId { get; set; }

        public virtual string? Name { get; set; }
        public virtual bool DelFlag { get; set; }
        public virtual DateTime? CreatedTime { get; set; }

        public virtual DateTime? UpdatedTime { get; set; }
        public virtual string? ImageUrl { get; set; }
        public virtual double Width { get; set; }
        public virtual double Height { get; set; }
        #endregion

        #region Relationship
        //public virtual ICollection<OccupiedBlock>? OccupiedBlocks { get; set; }
        public virtual ICollection<Block>? Blocks { get; set; }
        public virtual ICollection<Terminal>? Terminals { get; set; }
        #endregion
    }

    //public class OccupiedBlock
    //{
    //    public virtual double Latitude { get; set; }
    //    public virtual double Longtitude { get; set; }
    //}
}
