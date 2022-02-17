using kiosko.Data;
using kiosko.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace kiosko.Controllers
{
    public class LoginController : Controller
    {
        private readonly LoginContext _login;
        private readonly KColSoftContext _KColSoft;

        public LoginController(LoginContext login, KColSoftContext kColSoft)
        {
            _login = login;
            _KColSoft = kColSoft;
        }
        public IActionResult Index()
        {
            return View("Login");
        }

        [HttpPost]
        public IActionResult Login(LoginModel item)
        {
            SqlParameter[] param = new SqlParameter[]
            {
                new SqlParameter("@vchEmail",item.vchEmail),
                new SqlParameter("@vchPass",item.vchPass),
                new SqlParameter("@vchKUsuario", item.vchEmail),
            };

            try
            {
                var result = _login.LoginItems.FromSqlRaw<LoginModel>("exec Loggin @vchEmail, @vchPass", param).ToList();

                if (result.Count == 0)
                {
                    TempData["msg"] = "Usuario o Contrasena incorrectos, intenete otra vez";
                }
                else
                {
                    //xd
                    var res = _KColSoft.KColSoftsItem.FromSqlRaw<KColSoftModel>("exec dbo.RegistroDB @vchKUsuario", param).ToList();
                    return View("~/Views/Home/Index.cshtml");
                    TempData["msg"] = "Bienvenido";
                }

                return View("Login");
            }

            catch (Exception ex)
            {
                TempData["msg"] = "No encuentro la base de datos";
                throw;
            }

        }
    }
}
