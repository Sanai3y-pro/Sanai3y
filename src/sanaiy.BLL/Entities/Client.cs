using System;
using System.Collections.Generic;
using sanaiy.BLL.Enums;

namespace sanaiy.BLL.Entities
{
    /// <summary>
    /// العميل (المستخدم الذي يطلب الخدمات)
    /// </summary>
    public class Client : BaseEntity
    {
        // Personal Information
        public string Fname { get; set; } = string.Empty;
        public string Lname { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty; // Hashed
        public string? Phone { get; set; }

        // Account Settings
        public UserStatus Status { get; set; } = UserStatus.PendingEmailConfirmation;
        public bool IsActive { get; set; } = true;
        public string? ProfileImageUrl { get; set; }

        // Navigation Properties
        public virtual ICollection<Booking> Bookings { get; set; } = new HashSet<Booking>();
        public virtual ICollection<Address> Addresses { get; set; } = new HashSet<Address>();
        public virtual ICollection<Review> Reviews { get; set; } = new HashSet<Review>();
        public virtual ICollection<Violation> ViolationsReported { get; set; } = new HashSet<Violation>();
        public virtual ICollection<PaymentTransaction> PaymentTransactions { get; set; } = new HashSet<PaymentTransaction>();

        // Computed Property
        public string FullName => $"{Fname} {Lname}";
    }
}