# Blazor9 OIDC Authentication
An example of a default template Blazor9 app with automatic interactivity setup to use OIDC authentication with Blazor Server pages, Blazor client (WASM/Web Assembly) pages, web api and minimal api.

This sample is using Microsoft personal accounts (e.g. Outlook.com, Hotmail, Live.com) with Microsoft Entra External ID and OAuth2/OpenID Connect.

You don’t need full Azure AD or enterprise setup, just register your app in the Microsoft Entra admin center, configure OAuth, and wire up authentication.

## App Registration
1. Register the app in Microsoft Entra (Azure portal), go to: https://entra.microsoft.com
1. Navigate to App registrations > `New registration`
1. Set the folooiwng options: 
   - `Name`: "Blazor9OIDC" or your app name
   - `Supported account types`: "Accounts in any organizational directory (Any Microsoft Entra ID tenant - Multitenant) and personal Microsoft accounts (e.g. Skype, Xbox)",
   - `Redirect URI`: "Web" and "http://localhost/signin-oidc" (No port required. When deploying, pay attention to HTTPS too).
   - `Grant admin consent to openid and offline_access permissions` - keep checked
1. After creating, make a note of the `Application (client) ID` and `Directory (tenant) ID` from the Overview page
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
