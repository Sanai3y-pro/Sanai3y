using System;
using System.Collections.Generic;
using sanaiy.BLL.Enums;

namespace sanaiy.BLL.Entities
{
    
    public class Address : BaseEntity
    {
       
        public string FullAddress { get; set; } = string.Empty;
        public AddressType? AddressType { get; set; }
        public string? City { get; set; }
        public bool IsDefault { get; set; } = false;

      
        public Guid? ClientId { get; set; }
        public Guid? CraftsmanId { get; set; }

        // Navigation Properties
        public virtual Client? Client { get; set; }
        public virtual Craftsman? Craftsman { get; set; }
        public virtual ICollection<Booking> Bookings { get; set; } = new HashSet<Booking>();
    }
}