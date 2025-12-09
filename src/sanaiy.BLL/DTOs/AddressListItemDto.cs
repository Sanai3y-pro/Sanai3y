using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sanaiy.BLL.DTOs.Address
{
    public class AddressListItemDto
    {
        public Guid Id { get; set; }
        public string FullAddress { get; set; } = string.Empty;
        public string? AddressType { get; set; }
        public string? City { get; set; }
        public bool IsDefault { get; set; }

        // For Map Display
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
}