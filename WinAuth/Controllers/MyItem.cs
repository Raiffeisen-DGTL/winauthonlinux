using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using WinAuth.DataBase;

namespace WinAuth.Controllers
{
    [Route("api/[controller]")]
    public class MyItem : Controller
    {
        private readonly MyContext _context;

        public MyItem(MyContext context)
        {
            _context = context;
        }
        
        // простой запрос сущностей из таблицы
        [HttpGet]
        public IActionResult Index()
        {
            try
            {
                return Ok(_context.CatalogItems.ToList());
            }
            catch (Exception ex)
            {
                return base.StatusCode(500, ex);
            }
        }
    }
}