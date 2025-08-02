using Microsoft.EntityFrameworkCore; // ✅ PHẢI CÓ DÒNG NÀY
using Prometheus;

namespace cicd
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.WebHost.UseUrls("http://*:80");

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
            app.UseHttpMetrics();

            app.UseMetricServer();
            // app.MapMetrics();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
    
    // Gom cả MapMetrics vào đây để đảm bảo nó cũng là một endpoint
    endpoints.MapMetrics(); 
});
            app.Run();
        }
    }
}
