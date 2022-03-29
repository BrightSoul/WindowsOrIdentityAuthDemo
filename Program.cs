using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WindowsOrIdentityAuthDemo.Data;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

const string customScheme = "SELECT_WINDOWS_OR_IDENTITY";
builder.Services
       .AddAuthentication(options =>
       {
           options.DefaultScheme = customScheme;
           options.DefaultChallengeScheme = customScheme;
       })
       .AddNegotiate()
       .AddPolicyScheme(customScheme, customScheme, options =>
       {
           options.ForwardDefaultSelector = context =>
           {
               string cookieName = $"{CookieAuthenticationDefaults.CookiePrefix}{IdentityConstants.ApplicationScheme}";
               if (context.Request.Cookies.ContainsKey(cookieName))
               {
                   // The request has the Identity cookie, then use Identity for authentication
                   return IdentityConstants.ApplicationScheme;
               }

               // Otherwise fallback to Windows authentication
               return NegotiateDefaults.AuthenticationScheme;
           };
       });

builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages().RequireAuthorization();

app.Run();
