using Back.Implementation;
using Back.Interfaces;
using Back.Middleware;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddDataAnnotationsLocalization()
    .AddViewLocalization();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllersWithViews();

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[] { "en", "es" };
    options.SetDefaultCulture(supportedCultures[0]);
    options.AddSupportedCultures(supportedCultures);
    options.AddSupportedUICultures(supportedCultures);
});

builder.Services.AddSingleton<IKeyVaultService, KeyVaultService>();
builder.Services.AddSingleton<IPasswordHashService, PasswordHashService>();
builder.Services.AddSingleton<ITextEncryptionService, AesEncryptionService>();
builder.Services.AddSingleton<IKeyVaultService, KeyVaultService>();
builder.Services.AddScoped<ICompanies, Companies>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<IUsers, Users>();
builder.Services.AddScoped<IUserService, UserService>();


var app = builder.Build();

app.UseStaticFiles();

var localizationOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value;
app.UseRequestLocalization(localizationOptions);

app.UseMiddleware<ApiKeyMiddleware>();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapFallbackToController("Index", "Home");

app.Run();
