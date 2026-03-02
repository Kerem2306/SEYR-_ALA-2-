
using SEYRÝ_ALA.Data.Interfaces;
using SEYRÝ_ALA.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using SEYRÝ_ALA.Data; 

var builder = WebApplication.CreateBuilder(args);


// Repository Kaydý
builder.Services.AddScoped<ICityRepository, CityRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// EF Core DbContext kaydý (ConnectionStrings:DefaultConnection'ý kullanýr) // baðlantý dizesi doðrudan kodda deðil 
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// MVC
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddControllersWithViews()
    .AddViewLocalization(Microsoft.AspNetCore.Mvc.Razor.LanguageViewLocationExpanderFormat.Suffix)
    .AddDataAnnotationsLocalization();
// oturum yönetimi
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();
// authertication ekle - cookie bazlý 
builder.Services.AddAuthentication("CookieAuth") //kimlik doðrulama þemasý 
    .AddCookie("CookieAuth", config =>
    {
        config.Cookie.Name = "UserLoginCookie";
        config.LoginPath = "/Account/Login"; // Yetkisiz giriþte yönlendirilecek sayfa
    });

var supportedCultures = new[] { "tr", "en" };
var localizationOptions = new RequestLocalizationOptions() // çoklu dil desteði ayarlarý
    .SetDefaultCulture("tr")
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

var app = builder.Build();

// Pipeline
// hangi dil de yanýt alýnacaðý ve sistemin güvenliði için gerekli ayarlar 
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Kimlik doðrulamayý aktif eder

app.UseSession();

app.UseAuthorization();
app.UseRequestLocalization(localizationOptions); // güvenlik ayarý

//varsayýlan sayfada açýlmasý için 
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
