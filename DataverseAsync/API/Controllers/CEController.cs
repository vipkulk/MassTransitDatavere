using DOMAIN.Messages;
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
                ["jobtitle"] = customer.JobTitle,
                ["emailaddress1"] = customer.Email,
                ["mobilephone"] = customer.MobilePhone,
                ["address1_line1"] = customer.Address,
                ["address1_city"] = customer.City,
                ["address1_country"] = customer.Country
            };
            var crmId = service.Create(contact);
            var response = new SubmitResponse()
            {
                Id = crmId,
                isSumbitted = true,
            };
            return Ok(response);
        }
    }
}
