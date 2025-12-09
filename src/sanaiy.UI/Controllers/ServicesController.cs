using Microsoft.AspNetCore.Mvc;
using sanaiy.BLL.DTOs.Service;
using sanaiy.BLL.Entities;

namespace sanaiy.UI.Controllers
{
    public class ServicesController : Controller
    {
        public IActionResult Services()
        {
            return View();
        }


 
    }
}
