﻿using DOMAIN.Interfaces;
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
                ["jobtitle"] = customer.JobTitle,
                ["emailaddress1"] = customer.Email,
                ["mobilephone"] = customer.MobilePhone,
                ["address1_line1"] = customer.Address,
                ["address1_city"] = customer.City,
                ["address1_country"] = customer.Country,
                ["preferredcontactmethodcode"] = new OptionSetValue(2),
                ["creditlimit"] = new Money(100),
                ["address1_latitude"] = 10.89,
                ["donotfax"] = true,
                ["new_decimal"] = Convert.ToDecimal(100.34),
                ["numberofchildren"] =10
            };
            var response =await transitOrganizationService.Create(contact,customer);
            return Accepted(response);
        }

        [HttpPatch]
        public async Task<IActionResult> Update([FromBody] CustomerUpdate customer, [FromServices] ITransitOrganizationService transitOrganizationService)
        {
            var contact = new Entity("contact")
            {
                ["firstname"] = customer.FirstName
            };
            contact.Id = customer.Id;
            var response = await transitOrganizationService.Update(contact);
            return Accepted(response);
        }

    }
}
