using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SBController : ControllerBase
    {
        private readonly ServiceBusClient _serviceBusClient;

        public SBController(ServiceBusClient  serviceBusClient)
        {
            _serviceBusClient = serviceBusClient;
        }

        [HttpPost]
        public async Task<IActionResult> Send([FromBody] Customer customer) {
            var message = JsonConvert.SerializeObject(customer);
            var sender = _serviceBusClient.CreateSender("sbsend");
            await sender.SendMessageAsync(new ServiceBusMessage(message));
            await sender.CloseAsync();
            return Accepted(message);
        }
    }
}
