using FirstMVCWebApp.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConn"),
        sqlServerOptions => sqlServerOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null
        )
    ));


// Add services to the container.
builder.Services.AddControllersWithViews();


// 1. Add Cookie Authentication Services
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "MyCookieAuth";
    options.DefaultSignInScheme = "MyCookieAuth";
    options.DefaultChallengeScheme = "MyCookieAuth";
})
.AddCookie("MyCookieAuth", options =>
{
    options.Cookie.Name = "UserLoginCookie";
    options.LoginPath = "/Account/Login"; // Redirect path if unauthorized
    options.ExpireTimeSpan = TimeSpan.FromMinutes(20); // Cookie timeout
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// पुराना कोड बदलकर डिफ़ॉल्ट रास्ता Account/Login कर दें
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");



app.Run();
