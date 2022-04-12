using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SLE_System.Models;
using kiosko.Helpers;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;

namespace kiosko.Controllers
{
    public class AccountController : Controller
    {

        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        MailService _mailService;
        ErrorService _errorService;

        public AccountController(UserManager<IdentityUser> userManager,
                              SignInManager<IdentityUser> signInManager,
                               MailService mailService, ErrorService errorService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mailService = mailService;
            _errorService = errorService;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser
                {
                    UserName = model.Usuario,
                    Email = model.Usuario,
                };

                var result = await _userManager.CreateAsync(user, model.Clave);
                await _userManager.AddToRoleAsync(user, model.Role);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    return RedirectToAction("index", "Modulos");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                ModelState.AddModelError(string.Empty, "Error: Usuario y/o contraseña incorrectos.");

            }
            return View(model);
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            if(User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Modulos");

            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel user)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(user.Usuario, user.Clave, user.RememberMe, false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Modulos");
                }

                ModelState.AddModelError(string.Empty, "Error: Usuario y/o contraseña incorrectos.");

            }
            return View(user);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Login");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        public IActionResult ResetPassword(string id, string token, string password)
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendResetPwdLink(string email)
        {
            var message = new { status = "", message = "" };
            IActionResult ret = null;
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                byte[] tokenGeneratedBytes = Encoding.UTF8.GetBytes(token);
                var tokenEncoded = WebEncoders.Base64UrlEncode(tokenGeneratedBytes);
                var link = "https://localhost:7045/Account/ResetPassword?";
                var buillink = link + "&Id=" + user.Id + "&token=" + tokenEncoded;
                var cuerpoMensaje = "<h3>Recuperación de contraseña</h1>";
                cuerpoMensaje += "<p>Da click en el siguiente enlace para cambiar tu contraseña</p>";
                cuerpoMensaje += "<a href=";
                cuerpoMensaje += buillink;
                cuerpoMensaje += ">Da click aquí</a>";
                try
                {
                    //_mailService.SendEmailGmail(email, "Recuperación de contraseña", cuerpoMensaje);
                    message = new { status = "ok", message = "Email enviado" };
                    ret = StatusCode(StatusCodes.Status200OK, message);
                }
                catch (Exception ex)
                {
                    _errorService.SaveErrorMessage("_mailService.SendEmailGmail", "AccountController", "SendResetPwdwLink", ex.Message);
                    message = new { status = "error", message = ex.Message };
                    ret = StatusCode(StatusCodes.Status500InternalServerError, message);
                }
            }
            else
            {
                _errorService.SaveErrorMessage("_userManager.FindByEmailAsync", "AccountController", "SendResetPwdwLink", "Usuario no existe");
                message = new { status = "error", message = "Internal server error" };
                ret = StatusCode(StatusCodes.Status500InternalServerError, message);
            }
            return ret;
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePassword(string id, string token, string password)
        {
            IActionResult ret = null;
            var user = await _userManager.FindByIdAsync(id);
            var message = new { status = "", message = "" };
            if (user == null)
            {
                _errorService.SaveErrorMessage("_userManager.FindByIdAsync", "AccountController", "UpdatePassword", "No existe el usuario con el Id");
                message = new { status = "error", message = "Internal server error" };
                ret = StatusCode(StatusCodes.Status500InternalServerError, message);
            }
            var codeDecodedBytes = WebEncoders.Base64UrlDecode(token);
            var tokenDecoded = Encoding.UTF8.GetString(codeDecodedBytes);
            var result = await _userManager.ResetPasswordAsync(user, tokenDecoded, password);
            if (!result.Succeeded)
            {
                _errorService.SaveErrorMessage("_userManager.ResetPasswordAsync", "AccountController", "UpdatePassword", "Contraseña no Actualizada");
                message = new { status = "error", message = "Internal server error" };
                ret = StatusCode(StatusCodes.Status500InternalServerError, message);
            }
            else
            {
                message = new { status = "ok", message = "Contraseña actualizada" };
                ret = StatusCode(StatusCodes.Status201Created, message);
            }
            return ret;
        }

    }

    internal class Constants
    {
        public const string AdministratorRole = "Admin";
    }

    public class EmailTemplate
    {
        public string Link { get; set; }
        public string UserId { get; set; }
        public string EmailType { get; set; }
    }
}
