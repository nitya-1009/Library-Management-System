using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

public class AccountController : Controller
{
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    // 🎯 SECURITY BYPASS TAG FOR FORGOT PASSWORD PAGE
    [HttpGet]
    [AllowAnonymous]
    public IActionResult ForgotPassword()
    {
        return View();
    }

    // 🎯 SECURITY BYPASS TAG FOR SUPPORT HELP DESK PAGE
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Contact()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string username, string password, string ReturnUrl = null)
    {
        if (username == "admin" && password == "Admin@123")
        {
            // 1. यूजर की पहचान बनाना
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, username) };

            // यहाँ हमने "MyCookieAuth" लिख दिया है
            var claimsIdentity = new ClaimsIdentity(claims, "MyCookieAuth");

            // 2. ब्राउज़र में साइन-इन की कुकी सेव करना (यहाँ भी "MyCookieAuth" आएगा)
            await HttpContext.SignInAsync("MyCookieAuth", new ClaimsPrincipal(claimsIdentity));

            if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
            {
                return Redirect(ReturnUrl);
            }

            return RedirectToAction("StudentRegistration", "Home");
        }

        ViewBag.Error = "Invalid Username or Password";
        return View();
    }


    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync("MyCookieAuth");
        return RedirectToAction("Login");
    }
}
