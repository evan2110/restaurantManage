using RestaurantManage.Middlewares;

namespace RestaurantManage
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            builder.Services.AddHttpContextAccessor();

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

            app.UseAuthorization();
            app.UseSession();

            app.MapWhen(context =>
            {
                return (context.Request.Path.StartsWithSegments("/Cart") && context.Request.Method == "GET")
                        || (context.Request.Path.StartsWithSegments("/Account") && context.Request.Method == "GET")
                        || (context.Request.Path.StartsWithSegments("/History") && context.Request.Method == "GET")
                        /*|| (context.Request.Path.StartsWithSegments("/") && context.Request.Method == "GET")*/
                        || (context.Request.Path.StartsWithSegments("/Detail") && context.Request.Method == "GET");
            }, appBuilder =>
            {
                appBuilder.UseMiddleware<AuthenticationMiddleware>();
                appBuilder.UseRouting();
                appBuilder.UseAuthorization();
                appBuilder.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllerRoute(
                        name: "default",
                        pattern: "{controller=Home}/{action=Index}/{id?}");
                });
            });
            
            // Đăng ký AuthorizeMiddleware cho URL /Dashboard
            app.Map("/Dashboard", appBuilder =>
            {
                appBuilder.UseMiddleware<AuthorizeMiddleware>();
                appBuilder.UseRouting();
                appBuilder.UseAuthorization();
                appBuilder.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllerRoute(
                        name: "dashboard",
                        pattern: "{controller=Dashboard}/{action=Index}/{id?}");
                });
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            app.Run();
        }
    }
}
