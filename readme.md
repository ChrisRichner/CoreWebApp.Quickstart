#Core Web App Quickstart

##Features
- AspNetCore 1.0
    - https Kestrel (uncomment Code for https)
- Thinktecture IdentityServer4
    - [AspNetIdentity](https://github.com/IdentityServer/IdentityServer4.AspNetIdentity)
    - [EntityFrameworkStorage](https://github.com/IdentityServer/IdentityServer4.Samples/tree/dev/Quickstarts/8_EntityFrameworkStorage)
    - Social Logins (Google, Twitter)
- [Swagger](http://swagger.io/)
	- Swagger is a simple yet powerful representation of your RESTful API 
    - IdentityServer OAuth Integration
- Unit Tests based on [XUnit](https://xunit.github.io/docs/getting-started-dotnet-core.html)

##Test Drive
- Open CoreWebApp.Quickstart.sln
- Set CoreWebApp as the startup project
- Choose CoreWebApp (Kestrel) as your debug configuration. Alternatively you can still use IIS Express for debugging
- Hit F5 to build and start debugging
- The console window starts and kestrel serves the API and STS at http://localhost:5000
- Start your favorite browser and open http://localhost:5000
