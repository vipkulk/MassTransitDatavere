using DOMAIN.Messages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using System.Globalization;

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
                ["address1_country"] = customer.Country,
                ["ownerid"] = new EntityReference("systemuser", new Guid("d4b0cf0f-597e-ed11-81ad-000d3aa88402")),
                ["preferredcontactmethodcode"] = new OptionSetValue(2),
                ["creditlimit"] = new Money(100),
                ["donotfax"] = true,
                ["new_decimal"] = Convert.ToDecimal(100.56),
                ["importsequencenumber"] = Convert.ToInt32(100)
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
