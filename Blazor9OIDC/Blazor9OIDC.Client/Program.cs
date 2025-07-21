using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthenticationStateDeserialization();


// Step 1: Add auth (MSAL or OIDC)
builder.Services.AddOidcAuthentication(options =>
{
    builder.Configuration.Bind("Oidc", options.ProviderOptions);
    options.ProviderOptions.DefaultScopes.Add("read"); // Match your API scopes
});


// Step 2: Register an authorized HttpClient
builder.Services.AddHttpClient("MyApi", client =>
    {
        client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
    })
    .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

// Optional: make the named client the default
builder.Services.AddScoped(sp =>
    sp.GetRequiredService<IHttpClientFactory>().CreateClient("MyApi"));

// builder.Services.AddScoped<AuthorizationMessageHandler>();
// builder.Services.AddScoped(_ => new HttpClient
// {
//     BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
// });

// builder.Services.AddMsalAuthentication(options =>
// {
//     options.ProviderOptions.Authentication.Authority = "https://login.microsoftonline.com/consumers/v2.0\"";
//     options.ProviderOptions.Authentication.ClientId = "api://fedf0f39-a6ec-4270-add9-4a92c98ab1de";
//     options.ProviderOptions.Authentication.ValidateAuthority = false;
//     options.ProviderOptions.DefaultAccessTokenScopes.Add("https://tuesdayfootball.onmicrosoft.com/api/read");
// });

// builder.Services.AddOidcAuthentication(options =>
// {
//     builder.Configuration.Bind("Oidc", options.ProviderOptions);
//     options.ProviderOptions.DefaultScopes.Add("api"); // Match your API scopes
// });






await builder.Build().RunAsync();
