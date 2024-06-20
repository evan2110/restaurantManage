using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace RestaurantManage.Middlewares;

public class AuthenticationMiddleware
{
    private readonly RequestDelegate _next;

    public AuthenticationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        string userName = context.Session.GetString("UserName");
        string role = context.Session.GetString("Role");
        if (userName == null)
        {
            context.Response.Redirect("/Login");
            return;
        }
        
        await _next(context);
    }

}