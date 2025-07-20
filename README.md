# Blazor9 OIDC Authentication
An example of a default template Blazor9 app with automatic interactivity setup to use OIDC authentication.

## Create Blazor App
1. Create the app with `dotnet new blazor -int Auto -au None -n Blazor9OIDC`

- `blazor` is the new Blazor 9 default template for a Blazor Web App featuring server and client projects.
- `int Auto` sets the rendering mode to Automatic which uses Server while downloading WebAssembly assets, then uses WebAssembly
- `-au None` ensure no templated authentication components. We will set this up manually. OIDC and other auth options are not offered, only "Individual authentication" which uses a local database.
- `-n Blazor9OIDC` just sets the name of the project

