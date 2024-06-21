namespace RestaurantManage.Middlewares;

public class AuthorizeMiddleware
{
    private readonly RequestDelegate _next;

    public AuthorizeMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        string userName = context.Session.GetString("UserName");
        string role = context.Session.GetString("Role");
        if (role != "Admin")
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync("Forbidden: You do not have access to this resource.");
            return;
        }
        
        await _next(context);
    }
}