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
    public async Task<IActionResult> Login(string username, string password)
    {
        // Replace this with your actual database check logic
        if (username == "admin" && password == "Admin@123")
        {
            // Create user identity details
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, username) };
            var identity = new ClaimsIdentity(claims, "MyCookieAuth");
            var principal = new ClaimsPrincipal(identity);

            // Write the encrypted cookie out to the browser session
            await HttpContext.SignInAsync("MyCookieAuth", principal);

            return RedirectToAction("StudentRegistration", "Home");
        }

        ViewBag.Error = "Invalid login credentials.";
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync("MyCookieAuth");
        return RedirectToAction("Login");
    }
}
