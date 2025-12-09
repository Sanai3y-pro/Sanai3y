using System;

namespace sanaiy.UI.ViewModels
{
    public class AddCategoryRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Icon { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
