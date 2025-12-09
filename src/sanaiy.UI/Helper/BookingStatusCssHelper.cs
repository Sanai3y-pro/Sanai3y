namespace sanaiy.UI.Helpers
{
    public static class BookingStatusCssHelper
    {
        public static string GetStatusCss(string status)
        {
            return status switch
            {
                "Pending" => "text-yellow-600 bg-yellow-100",
                "Confirmed" => "text-green-600 bg-green-100",
                "Rejected" => "text-red-600 bg-red-100",
                "Completed" => "text-blue-600 bg-blue-100",
                _ => "text-gray-600 bg-gray-100"
            };
        }
    }
}
