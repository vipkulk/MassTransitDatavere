using Microsoft.AspNetCore.Mvc;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CEController : ControllerBase
    {
        [HttpPost]
        public ActionResult Create([FromBody] Customer customer, [FromServices] IOrganizationServiceAsync2 service)
        {
            var contact = new Entity("contact")
            {
                ["firstname"] = customer.FirstName,
                ["lastname"] = customer.LastName,
            };
            service.Create(contact);
            return Ok();
        }
    }
}
