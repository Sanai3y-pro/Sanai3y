using System;
using System.Collections.Generic;

namespace sanaiy.BLL.Entities
{
    public class Category : BaseEntity
    {
      
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Image { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation Properties
        public virtual ICollection<Service> Services { get; set; } = new HashSet<Service>();
    }
}