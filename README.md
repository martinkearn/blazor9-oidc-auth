# Blazor9 OIDC Authentication
An example of a default template Blazor9 app with automatic interactivity setup to use OIDC authentication.

This sample is using Microsoft personal accounts (e.g. Outlook.com, Hotmail, Live.com) with Microsoft Entra External ID and OAuth2/OpenID Connect.

You don’t need full Azure AD or enterprise setup, just register your app in the Microsoft Entra admin center, configure OAuth, and wire up authentication.

## App Registration
1. Register the app in Microsoft Entra (Azure portal), go to: https://entra.microsoft.com
1. Navigate to App registrations > `New registration`
1. Set the folooiwng options: 
   - `Name`: "Blazor9OIDC or your app name"
   - `Supported account types`: "Accounts in any organizational directory (Any Microsoft Entra ID tenant - Multitenant) and personal Microsoft accounts (e.g. Skype, Xbox)",
   - `Redirect URI`: "Web" and "http://localhost:5296/signin-oidc" (change your port if it runs on a different port locally. When deploying, pay attention to HTTPS too).
   - Enable `Access tokens (used for implicit flows)` 
   - Enable `ID tokens (used for implicit and hybrid flows)`
1. After creating, make a note of the `Application (client) ID` from the Overview page
1. Go to Certificates & secrets > `Add a client secret`
   - Create a new client secret
   - Give it a description
   - Give it an expiry
   - Make a note of the `Value` and `Secret ID` (these are secrets, so store them securely)

## Create Blazor App
1. Create the app with `dotnet new blazor -int Auto -au None -n Blazor9OIDC`
   - `blazor` is the new Blazor 9 default template for a Blazor Web App featuring server and client projects.
   - `int Auto` sets the rendering mode to Automatic which uses Server while downloading WebAssembly assets, then uses WebAssembly
   - `-au None` ensure no templated authentication components. We will set this up manually. OIDC and other auth options are not offered, only "Individual authentication" which uses a local database.
   - `-n Blazor9OIDC` just sets the name of the project

## Blazor Server project setup
On the server project (Blazor9OIDC).

1. `dotnet add package Microsoft.AspNetCore.Authentication.OpenIdConnect`
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

1. In program.cs, add this before `app.Run()`:

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

1. In Components > App.razor, add this as the first line `@using Microsoft.AspNetCore.Components.Authorization`

1. In Components > App.razor, update `<Routes />` to be as follows:

```
<CascadingAuthenticationState>
    <Routes/>
</CascadingAuthenticationState>
```

1. In Components > Pages > Home.razor, add this code:

```
@using Microsoft.AspNetCore.Components.Authorization
<AuthorizeView>
    <Authorized>
        <p>Hello, @context.User.Claims.AsQueryable().Single(c => c.Type == "name").Value!</p>
        <p>The claims are:</p>
        <ul>
            @foreach(var c in context.User.Claims)
            {
                <li>@c.Type: @c.Value</li>
            }
        </ul>
        <a href="/signout">Logout</a>
    </Authorized>
    <NotAuthorized>
        <a href="/signin">Login</a>
    </NotAuthorized>
</AuthorizeView>
```

1. Allthough the client project is not yet configured. You should be able to run the app (`dotnet run`) and sign-in on the server pages. On first sign in, you will need to "Let this app access your info". If sucessfull, the home page of your app will show all the claims.

## Blazor client project setup
On the server project (Blazor9OIDC.Client).

1. `dotnet add package Microsoft.AspNetCore.Components.WebAssembly.Authentication`
1. In Program.cs (the client project version), add this directly after `var builder = WebAssemblyHostBuilder.CreateDefault(args);`:

```
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthenticationStateDeserialization();
```

1. In `_imports.razor`, add the line to the bottom of the file: `@using Microsoft.AspNetCore.Components.Authorization`

1. In Pages > `Counter.razor`, replace `@rendermode InteractiveAuto` with `@rendermode InteractiveWebAssembly`. This forces the page to run on the client/Web Assembly

1. In Pages > `Counter.razor`, add this code. The Render mode part is not required but useful to see that the page is running on Web Assembly:

```
<p>Render Mode: @(RendererInfo.IsInteractive ? "Interactive" : "Not Interactive") (@RendererInfo.Name)</p>

<AuthorizeView>
    <Authorized>
        <p>Hello, @context.User.Claims.AsQueryable().Single(c => c.Type == "name").Value!</p>
        <p>The claims are:</p>
        <ul>
            @foreach(var c in context.User.Claims)
            {
                <li>@c.Type: @c.Value</li>
            }
        </ul>
        <a href="/signout">Logout</a>
    </Authorized>
    <NotAuthorized>
        <a href="/signin">Login</a>
    </NotAuthorized>
</AuthorizeView>
```

1. Run the server app (server, not client), login as you did before and navigate to the Counter page. You will see the same claims. Also note "Interactive (WebAssembly)" which proves it is running on the client.

## Add a basic API
In this step, we'll add a basic API, without security initially, to the server project. This API will later be secured with OIDC.

1. In the server project, add a new API controller via "Scaffolded item" > `API Controller - Empty`. Name it `SecureApiController.cs`.
1. In the new controller, add this code:

```
[HttpGet]
public IActionResult Get()
{
   return Ok("You are authorized to access this secure API endpoint.");
}
```
1. In Program.cs in the server project, add this after `var builder = WebApplication.CreateBuilder(args);`:

```
builder.Services.AddControllers(); 
builder.Services.AddHttpClient();
```

1. In Program.cs in the server project, add this after `app.UseAuthorization();`:

```
app.MapControllers();
```

1. Run the Server project and execute the following command in a terminal: `curl -i http://localhost:5296/api/secureapi`. You should receive a `HTTP/1.1 200 OK` response with the message "You are authorized to access this secure API endpoint". This proves that your API is running, but it is not yet secured.

## Call the Api from the client project
In this step, we'll call the API from the client project and display the result.

1. In Program.cs (in the client project), add this after `builder.Services.AddAuthenticationStateDeserialization();`

```
builder.Services.AddScoped(_ => new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
});
```

1. Add this code to the `Counter.razor` page in the client project at the top, below `@rendermode InteractiveWebAssembly`:

```
@inject HttpClient Http
```

1. Add this code to the `Counter.razor` page in the client project just above `@code {`:

```
<button class="btn btn-primary" @onclick="CallApi">@message</button>
```

1. Add this code to the `Counter.razor` page in the client project just beneath `private int currentCount = 0;`:
```
 private string? message = "Call API";

 private async Task CallApi()
 {
     message = await Http.GetStringAsync("api/secureapi");
 }
```
1. Run the server project and navigate to the Counter page in the client project. Click the "Call API" button. You should see the message "You are authorized to access this secure API endpoint." displayed as the button text. This proves that you have called the unsecure API from the client project.