using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WinAuth.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class User : Controller
    {
        // проверяем, заполнился ли контект информацией об авторизованном пользователе
        [HttpGet]
        public IActionResult WhoAmI()
        {
            return Ok(HttpContext.User.Identity?.IsAuthenticated);
        }

        [HttpGet("info")]
        public IActionResult GetUserClaims()
        {
            // для демонстрации создаем копию полученных клеймов пользователя, просто затираем 
            // клеймы нашего домена, чтобы не утекла никакая информация :)
            var identity = new ClaimsIdentity(
                ((ClaimsIdentity) HttpContext.User.Identity).Claims.Select(x => new Claim(x.Type, 
                    x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"? "user@domain.ru" : "CN=Group-Test,OU=Role Based Access Permissions,OU=_Groups,DC=domain,DC=ru", x.ValueType, x.Issuer)));
            return Ok(identity.Claims.Select(x=>new {x.Issuer, x.OriginalIssuer, x.Properties, x.Type, x.Value, x.ValueType}));
        }
    }
}