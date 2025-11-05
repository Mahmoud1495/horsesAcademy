using HorsesPOC.Data;
using HorsesPOC.Services.Auth;
using HorsesPOC.Services.OtpService;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<AuthraizationFilter>();
builder.Services.AddHttpContextAccessor();

builder.Services.Configure<TwilioOptions>(builder.Configuration.GetSection("Twilio"));
builder.Services.AddSingleton<IWhatsAppSender, TwilioWhatsAppSender>();
builder.Services.AddScoped<IOtpService, OtpService>();

builder.Services.AddDbContext<AppDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Cookie authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
	.AddCookie(options =>
	{
		options.LoginPath = "/Account/Login"; // where to redirect if not logged in
		options.LogoutPath = "/Account/Logout";
		options.AccessDeniedPath = "/Account/AccessDenied";
		options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
		options.SlidingExpiration = true;
	});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStaticFiles(); 

//app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
