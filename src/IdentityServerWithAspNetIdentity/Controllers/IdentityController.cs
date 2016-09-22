using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace Api
{
    [Produces("application/json")]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class IdentityController : Controller
    {
        public IdentityController(ILogger<IdentityController> logger, IHostingEnvironment env)
        {
            Logger = logger;
            HostingEnvironment = env;
        }

        ILogger Logger { get; }
        IHostingEnvironment HostingEnvironment { get; }
        [HttpGet(Name = "Identity")]
        public IActionResult Get()
        {
            Logger.LogDebug("Get called");

            var claims = User.Claims.Select(c => new { c.Type, c.Value });
            return new JsonResult(new
            {
                Utc = DateTimeOffset.UtcNow,
                WebRootPath = HostingEnvironment.WebRootPath,
                ContentRootPath = HostingEnvironment.ContentRootPath,
                AuthenticationSchemes = this.ControllerContext.HttpContext.Authentication.GetAuthenticationSchemes(),
                Claims = claims,
                Env = Environment.GetEnvironmentVariables(),
            });
        }
    }

    [Produces("application/json")]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class ValuesController : ControllerBase
    {
        public ValuesController(ILogger<ValuesController> logger, IHostingEnvironment env)
        {
            Logger = logger;
            HostingEnvironment = env;
        }
        ILogger Logger { get; }
        IHostingEnvironment HostingEnvironment { get; }
        [HttpGet(Name = "Values")]
        public IActionResult Get()
        {
            Logger.LogDebug("Get called");

            var claims = User.Claims.Select(c => new { c.Type, c.Value });
            return new JsonResult(new
            {
                WebRootPath = HostingEnvironment.WebRootPath,
                ContentRootPath = HostingEnvironment.ContentRootPath,
                AuthenticationSchemes = this.ControllerContext.HttpContext.Authentication.GetAuthenticationSchemes(),
                Claims = claims,
                Env = Environment.GetEnvironmentVariables(),
            });
        }
    }
}