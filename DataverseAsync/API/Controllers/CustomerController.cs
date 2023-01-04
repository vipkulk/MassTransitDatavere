using DOMAIN.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Xrm.Sdk;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Customer customer, [FromServices] ITransitOrganizationService transitOrganizationService)
        {
            var contact = new Entity("contact")
            {
                ["firstname"] = customer.FirstName,
                ["lastname"] = customer.LastName,
            };
            var response =await transitOrganizationService.Update(contact);
            return Accepted(response);
        }
    }
}
