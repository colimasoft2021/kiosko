using kiosko.Data;
using kiosko.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace kiosko.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiLoginController : ControllerBase
    {
        private readonly LoginContext _login;

        public ApiLoginController(LoginContext login)
        {
            _login = login;
        }

        [HttpPost]
        public IActionResult Login(LoginModel item)
        {
            SqlParameter[] param = new SqlParameter[]
            {
                new SqlParameter("@vchEmail",item.vchEmail),
                new SqlParameter("@vchPass",item.vchPass)
            };
            try
            {
                var result = _login.LoginItems.FromSqlRaw<LoginModel>("exec Loggin @vchEmail, @vchPass",param).ToList();
                if (result.Count == 1)
                {
                    //res = isLoginUser(item, 1);
                    result[0].vchPass = "1";
                    return Ok(result);
                }
                else
                {
                    result[0].vchPass = "0";
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
