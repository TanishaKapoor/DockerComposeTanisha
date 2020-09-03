using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace OrderService.Controllers
{
    [Route("banana")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        List<Order> _orderDb = new List<Order>();

        private readonly IConfiguration _config;
        private readonly IBusControl _bus;

        public ValuesController(IBusControl bus, IConfiguration config)
        {
            _config = config;
            _bus = bus;
            _orderDb = OrdersDatabase.orderDb;
        }
        // GET api/values
        [HttpGet]
        public async Task<ActionResult<IEnumerable<string>>> Get()
        {
            Uri uri = new Uri($"rabbitmq://{_config.GetValue<string>("RabbitMQHostName")}/appleBananaToCart");
            var applesBanana = new AppleBananaCommon.AppleBananaEvent();
            applesBanana.AppleBananaID = 1;
            applesBanana.AppleBananaName = "apple banana name";
            var endPoint = await _bus.GetSendEndpoint(uri);
            await endPoint.Send(applesBanana);
            return Ok(_orderDb);
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            var customerOrders = _orderDb.Where(c => c.customerId == id).Select(c => new { c.orderId, c.orderAmount, c.orderDate }).ToList();
            return Ok(customerOrders);
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
