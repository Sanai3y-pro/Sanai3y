using System;
using System.Collections.Generic;
using sanaiy.BLL.Enums;

namespace sanaiy.BLL.Entities
{
    /// <summary>
    /// الحجز (الطلب من العميل)
    /// </summary>
    public class Booking : BaseEntity
    {
        // Booking Details
        public string? ServicesName { get; set; } // للتوافق مع النظام القديم
        public string? AdditionalNote { get; set; }
        public bool IsActive { get; set; } = true;

        // Scheduling
        public DateTime? PreferredDate { get; set; }
        public string? PreferredTime { get; set; }

        // Status & Payment
        public BookingStatus Status { get; set; } = BookingStatus.Pending;
        public PaymentMethod? PaymentMethod { get; set; }
        public decimal? PriceFinal { get; set; }

        // Foreign Keys
        public Guid ClientId { get; set; }
        public Guid? CraftsmanId { get; set; } // nullable (في البداية)
        public Guid ServiceId { get; set; }
        public Guid? LocationId { get; set; }

        // Navigation Properties
        public virtual Client Client { get; set; } = null!;
        public virtual Craftsman? Craftsman { get; set; }
        public virtual Service? Service { get; set; } = null!;
        public virtual Address? Location { get; set; }

        public virtual ICollection<Quote> Quotes { get; set; } = new HashSet<Quote>();
        public virtual Review? Review { get; set; }
        public virtual PaymentTransaction? PaymentTransaction { get; set; }
        public virtual ICollection<Violation> Violations { get; set; } = new HashSet<Violation>();
        public virtual Commission? Commission { get; set; }
        public virtual ICollection<WalletTransaction> WalletTransactions { get; set; } = new HashSet<WalletTransaction>();
    }
}