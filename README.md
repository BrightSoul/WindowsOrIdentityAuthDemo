# Windows or ASP.NET Core Identity Authentication demo

This is an ASP.NET Core 6 sample application, demonstrating both Windows (NTLM) and ASP.NET Core Identity (Cookie) authentication in place.

The application attempts at authenticating the user via Windows authentication. Then, if the user then logs in via the Identity UI by providing username and password, it will be authenticated by the cookie provided by Identity on each subsequent request.

## Getting started

 * Launch the application on your Windows machine in VSCode by pressing `F5`
 * You should see your Windows user name displated in the homepage;
 * To to the Login page, i.e. `/Identity/Account/Login` and use the following credentials:
   * Email: `user@example.com`
   * Password: `Password1!`
 * You should now see the user name has changed to `user@example.com` in the homepage.

## Show me the code

A custom policy scheme is registered which is forwarding the authentication to `IdentityConstants.ApplicationScheme` or `NegotiateDefaults.AuthenticationScheme` depending on the Identity application cookie being present in the request or not. You can see this in the [Program](Program.cs#L18) class.

```csharp
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
                   // Contiene il cookie, selezioniamo l'autenticazione con Identity
                   return IdentityConstants.ApplicationScheme;
               }

               // Altrimenti fallback su Windows authentication
               return NegotiateDefaults.AuthenticationScheme;
           };
       });

```