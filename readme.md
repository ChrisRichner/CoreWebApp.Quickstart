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

##Login
- Click the *Login With Profile and Access Token* button
- After the STS MVC Site has loaded click *Register*
- Provide a valid email (must not exist, but be valid foramtted) and a complex password
- Click *Register* button
- Now you're back on the index.html page, still not logged in
- Click the *Login With Profile and Access Token* button again
- Click *Yes, Allow" on the consent page
- You're back at the index.html page, this time you've got a valid ID and Access Token
- Click *Call Service* button to call the *IdentityController*
- Check out the *Ajax Result*
- Congrats, you've called the protected API with an Access Token provided by the STS

##Swagger
- Open http://localhost:5000/swagger/ui
- Open *Identity*
- Open the *GET /api/v1/Identity* action
- Click *Try it out!* Button
- You get a 401 response code because you can't call a protected API without an Access Token
- Click *Authorize* button in the top banner next to *Explore* button
- Choose *api1* and click *Authorize*
- The implicit flow takes you the STS Login Page, in case you're already signed in the redirect happens very fast
- Click *Try it out!* Button again
- This time you get a 200 response code. The response body shows the result returned from the protected API 
- Congrats, you've called the protected API from the Swagger UI Page with an Access Token provided by the STS
