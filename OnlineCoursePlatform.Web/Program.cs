using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using OnlineCoursePlatform.Core.Convertors;
using OnlineCoursePlatform.Core.Services;
using OnlineCoursePlatform.Core.Services.Interfaces;
using OnlineCoursePlatform.DataLayer.Context;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 52428800;
});

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

#region Authentication

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
}).AddCookie(options =>
{
    options.LoginPath = "/Login";
    options.LogoutPath = "/Logout";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(43200);
});

#endregion

#region DataBase Context

builder.Services.AddDbContext<OCPContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("OCPConnection"));
});

#endregion

#region IoC

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IViewRenderService, RenderViewToString>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IForumService, ForumService>();

#endregion

var app = builder.Build();

app.Use(async (context, next) =>
{
    await next();
    if (context.Response.StatusCode == 404) context.Response.Redirect("/Home/Error404");
});

app.Use(async (context, next) =>
{
    if(context.Request.Path.Value.ToLower().StartsWith("/courseonline"))
    {
        var callingUrl = context.Request.Headers["Referer"].ToString().ToLower();
        if (callingUrl != null && callingUrl.StartsWith("https://localhost:7008") ||
        callingUrl.StartsWith("http://localhost:5195")) await next.Invoke();

        else context.Response.Redirect("/Login");
    }
    else await next.Invoke();
});

app.UseStaticFiles();
app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();

app.MapRazorPages();

app.MapControllerRoute(
  name: "areas",
  pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
  name: "default",
  pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();