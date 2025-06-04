using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using qyn_mp3.Models;
using qyn_mp3.Repositories;
using qyn_mp3.Services;

namespace qyn_mp3
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Đăng ký OtpService
            builder.Services.AddSingleton<IOtpService, OtpService>();

            builder.Services.AddMemoryCache();
            builder.Services.AddSingleton<ICacheService, MemoryCacheService>();

            builder.Services.AddSingleton<IOtpGenerator, SecureOtpGenerator>();


            // Add services to the container.
            builder.Services.AddControllersWithViews();

            //Add email sender
            builder.Services.AddTransient<IEmailSender, EmailSender>();
            builder.Services.AddScoped<DbContext>();

            builder.Services.AddDistributedMemoryCache();

            builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
    });

            //Add session
            builder.Services.AddSession(options =>
            {
                // Set a short timeout for easy testing.
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                // Make the session cookie essential
                options.Cookie.IsEssential = true;
            });

            builder.Services.ConfigureApplicationCookie(options =>
            {
                // Cookie tạm thời (cho phiên làm việc) nếu RememberMe = false
                options.ExpireTimeSpan = TimeSpan.FromDays(30); // Thời gian tồn tại khi RememberMe = true
                options.SlidingExpiration = true; // Reset thời gian tồn tại khi user hoạt động
            });



            // Thêm services vào container
            builder.Services.AddDbContext<DBContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Đăng ký Identity
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<DBContext>()
                .AddDefaultTokenProviders();


            builder.Services.Configure<IdentityOptions>(options =>
            {
                options.Lockout.AllowedForNewUsers = false; // Tắt lockout cho user mới
                options.Lockout.MaxFailedAccessAttempts = 100; // Số lần thử tối đa (đặt lớn để vô hiệu hóa)
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.Zero; // Thời gian lock ngắn

                // Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 8;
                options.Password.RequiredUniqueChars = 0;

                // User settings.
                options.User.RequireUniqueEmail = false;
            });



            builder.Services.AddScoped<DBContext>();

            builder.Services.AddDbContext<DBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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

            app.UseSession();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Login}/{action=Login}/{id?}");

            app.Run();
        }
    }
}
