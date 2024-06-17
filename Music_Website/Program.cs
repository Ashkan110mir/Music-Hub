using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Music_Website.Data;
using Music_Website.Data.Admin_Data;
using Music_Website.Data.Albums_Data;
using Music_Website.Data.Comment_Data;
using Music_Website.Data.Contact_Us_Data;
using Music_Website.Data.Music_Data;
using Music_Website.Data.Music_Video_Data;
using Music_Website.Data.Remix_Data;
using Music_Website.Data.Singer;
using System.Drawing.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<ISingerData, SingerData>();
builder.Services.AddScoped<IMusicData, MusicData>();
builder.Services.AddScoped<IMusicVideo_Data, MusicVideo_Data>();
builder.Services.AddScoped<IRemixData, RemixData>();
builder.Services.AddScoped<IAlbumsData, AlbumsData>();
builder.Services.AddScoped<ICommentData, CommentData>();
builder.Services.AddScoped<IContactusDATA, ContactusDATA>();
builder.Services.AddScoped<IAdminData, AdminData>();
builder.Services.AddDbContext<Context>(option => option.UseSqlServer(connectionString: @"Data Source=DESKTOP-GADH0HK\ASHKANSQL;Initial Catalog=MusicWebsiteDB;Integrated Security=true;TrustServerCertificate=true"));
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(option =>
    {
        option.LoginPath = "/Admin/Admin_Login";
        option.LogoutPath = "/Admin/Admin_Logout";
        option.ExpireTimeSpan = TimeSpan.FromDays(2);
     
    });
builder.Services.Configure<FormOptions>(option =>
{
    option.MultipartBodyLengthLimit = 100 * 1024 * 1024;
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithRedirects("Error/Not_found");
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "admin",
    pattern: "admin/{controller=Admin}/{action=admin_page}/{id?}");
app.Run();
