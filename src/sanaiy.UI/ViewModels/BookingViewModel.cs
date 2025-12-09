namespace sanaiy.UI.ViewModels
{
    public class BookingViewModel
    {
        public Guid Id { get; set; }
        public DateTime? Date { get; set; }                  // موعد الحجز
        public string? PreferredTime { get; set; }          // الوقت المفضل
        public bool ShowPaymentButton { get; set; }

        // ======== العميل ========
        public string? ClientName { get; set; }
        public string? ClientImageUrl { get; set; }

        // ======== الحرفي ========
        public string? CraftsmanName { get; set; }
        public string? CraftsmanImageUrl { get; set; }
        public string? CraftsmanProfileImage { get; set; }

        // ======== الخدمة ========
        public string? ServiceName { get; set; }
        public string? ShortDescription { get; set; }
        public string? Category { get; set; }

        // ======== الحالة ========
        public string? StatusText { get; set; }             // نص الحالة للعرض
        public string? StatusBadgeCss { get; set; }         // CSS مناسب للحالة

        // ======== ملاحظات ========
        public string? Notes { get; set; }
        public string? AdditionalNote { get; set; }

        // ======== الموقع ========
        public string? Location { get; set; }

        // ======== السعر والدفع (اختياري) ========
        public decimal? PriceFinal { get; set; }
        public string? PaymentMethod { get; set; }
    }
}
