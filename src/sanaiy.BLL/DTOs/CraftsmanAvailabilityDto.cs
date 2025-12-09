using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sanaiy.BLL.DTOs.Craftsman;

namespace sanaiy.BLL.DTOs
{
    public class CraftsmanAvailabilityDto
    {
        public Guid Id { get; set; }
        public string Day { get; set; } = "";
        public string StartTime { get; set; } = "";
        public string EndTime { get; set; } = "";
    }
}
