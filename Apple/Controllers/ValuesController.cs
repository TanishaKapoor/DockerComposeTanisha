using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Apple;
using AppleBananaCommon;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace UserService.Controllers
{
    [Route("apple")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        List<User> _userDb = new List<User>();
        AppleBananaEvent apple = new AppleBananaEvent();

        public ValuesController()
        {
            _userDb = UserDatabase.userDb;
        }
        // GET users
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            if (!String.IsNullOrEmpty(AppleBananaConsumer.received))
            {
                var Event = AppleBananaConsumer.received;
                AppleBananaEvent json = JsonConvert.DeserializeObject<AppleBananaEvent>(Event);
                apple.AppleBananaID = json.AppleBananaID;
                apple.AppleBananaName = json.AppleBananaName;
            }
            return Ok(apple);
        }

        // GET users/{id}
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            var user = _userDb.Where(c => c.id == id).Select(c => new { c.age, c.name, c.email }).FirstOrDefault();
            return Ok(user);
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
