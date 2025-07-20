# Blazor9 OIDC Authentication
An example of a default template Blazor9 app with automatic interactivity setup to use OIDC authentication.

This sample is using Microsoft personal accounts (e.g. Outlook.com, Hotmail, Live.com) with Microsoft Entra External ID and OAuth2/OpenID Connect.

You don’t need full Azure AD or enterprise setup—just register your app in the Microsoft Entra admin center, configure OAuth, and wire up authentication.

## App Registration
1. Register the app in Microsoft Entra (Azure portal), go to: https://entra.microsoft.com
1. Navigate to `App registrations` > `New registration`
1. Set: `Name`: "Blazor9OIDC or your app name", `Supported account types`: "Accounts in any organizational directory (Any Microsoft Entra ID tenant - Multitenant) and personal Microsoft accounts (e.g. Skype, Xbox)", `Redirect URI`: "Web" and "https://localhost:5296/signin-oidc" (change your port if it runs on a different port locally)
1. Disable `Grant admin consent to openid and offline_access permissions`
1. After creating save the `Application (client) ID` from the `Overview` page
1. Go to Certificates & secrets > `Add a client secret` > Give it a description and expiry >  and safely save the `Value` and `Secret ID`

## Create Blazor App
1. Create the app with `dotnet new blazor -int Auto -au None -n Blazor9OIDC`

- `blazor` is the new Blazor 9 default template for a Blazor Web App featuring server and client projects.
- `int Auto` sets the rendering mode to Automatic which uses Server while downloading WebAssembly assets, then uses WebAssembly
- `-au None` ensure no templated authentication components. We will set this up manually. OIDC and other auth options are not offered, only "Individual authentication" which uses a local database.
- `-n Blazor9OIDC` just sets the name of the project

## Blazor Server project setup
1. On the server project (Blazor9OIDC) `dotnet add package Microsoft.AspNetCore.Authentication.OpenIdConnect`
1. In appsettings.Development.json, add this section beneath "Logging". Update YOUR_CLIENT_ID and YOUR_CLIENT_SECRET:

```
"Authentication": {
    "Schemes": {
      "OpenIdConnect": {
        "Authority": "https://login.microsoftonline.com/consumers/v2.0",
        "ClientId": "YOUR_CLIENT_ID",
        "ClientSecret": "YOUR_CLIENT_SECRET",
        "CallbackPath": "/signin-oidc",
        "SignedOutCallbackPath": "/signout-callback-oidc",
        "ResponseType": "code",
        "SaveTokens": true,
        "Scope": [ "openid", "profile", "email" ]
      }
    }
  }
```
 
1. In Program.cs add this at the top of the file:

```
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
```

1. In Program.cs, update `builder.Services.AddRazorComponents` to be as follows:

```
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents()
    .AddAuthenticationStateSerialization(options => options.SerializeAllClaims = true);
```


1. In Program.cs add this after `builder.Services.AddRazorComponents();`

 ```
builder.Services.Configure<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme,
    builder.Configuration.GetSection("Authentication:Schemes:OpenIdConnect"));
builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
    })
    .AddCookie()
    .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, _ => { }); // Leave empty, options are configured above
builder.Services.AddAuthorization();
 ```

1. In Program.cs add this after `var app = builder.Build();`

```
app.UseAuthentication();
app.UseAuthorization();
```

1. in program.cs, add this before `app.Run()`:

```
app.MapGet("/signin", async ctx =>
{
    await ctx.ChallengeAsync(OpenIdConnectDefaults.AuthenticationScheme, new AuthenticationProperties
    {
        RedirectUri = "/"
    });
});

app.MapGet("/signout", async ctx =>
{
    await ctx.SignOutAsync();
    ctx.Response.Redirect("/");
});
```
