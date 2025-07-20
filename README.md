# Blazor9 OIDC Authentication
An example of a default template Blazor9 app with automatic interactivity setup to use OIDC authentication.

This sample is using Microsoft personal accounts (e.g. Outlook.com, Hotmail, Live.com) with Microsoft Entra External ID and OAuth2/OpenID Connect.

You don’t need full Azure AD or enterprise setup—just register your app in the Microsoft Entra admin center, configure OAuth, and wire up authentication.

## App Registration
1. Register the app in Microsoft Entra (Azure portal)
  1. Go to: https://entra.microsoft.com
  1. Navigate to “App registrations” → “New registration”
  1. Set: `Name`: "Your app name", `Supported account types`: "Accounts in any Microsoft account (e.g. Skype, Xbox)", `Redirect URI`: "https://localhost:5296/signin-oidc" (change your port if it runs on a different port locally)
1. After creating save the Application (client) ID
1. Go to Certificates & secrets → Add a client secret and safely save the value

## Create Blazor App
1. Create the app with `dotnet new blazor -int Auto -au None -n Blazor9OIDC`

- `blazor` is the new Blazor 9 default template for a Blazor Web App featuring server and client projects.
- `int Auto` sets the rendering mode to Automatic which uses Server while downloading WebAssembly assets, then uses WebAssembly
- `-au None` ensure no templated authentication components. We will set this up manually. OIDC and other auth options are not offered, only "Individual authentication" which uses a local database.
- `-n Blazor9OIDC` just sets the name of the project

