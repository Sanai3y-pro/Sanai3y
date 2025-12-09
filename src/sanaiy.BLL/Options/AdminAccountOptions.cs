using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sanaiy.BLL.Options
{
    public class AdminAccountOptions
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty; // للتست
        public string Role { get; set; } = "Admin";
    }
}
