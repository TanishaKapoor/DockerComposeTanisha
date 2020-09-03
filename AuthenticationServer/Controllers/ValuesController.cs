using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ValuesController : ControllerBase
    {
        private IAuthenticateService _authenticateService;

        public ValuesController(IAuthenticateService authenticateService)
        {
            _authenticateService = authenticateService;
        }

        // POST api/values/authenticate to authenticate the user
        //if the username and password matches the one saved then
        //token is being generated otherwise authentication error is shown
        [AllowAnonymous]
        [HttpPost("authenticate")]
        public ActionResult Post([FromBody] User userParams)
        {
            var user = _authenticateService.Authenticate(userParams.Name, userParams.Password);
            if (user == null)
            {
                return BadRequest(new { message = "Username or password is incorrect" });
            }
            return Ok(new { message = "Successfully authenticating", loggedinUser = user });
        }

        [Authorize(Roles = "Customer")]
        public string Get()
        {
            return "M Authenticate";
        }
    }
}
