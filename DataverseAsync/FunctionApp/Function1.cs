using System;
using System.Text.Json;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;

namespace FunctionApp
{
    public class Function1
    {
        [FunctionName("Function1")]
        public void Run([ServiceBusTrigger("sbsend", Connection = "SBConnection")]string myQueueItem, ILogger log)
        {
            var customer = JsonSerializer.Deserialize<Customer>(myQueueItem);
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
                ["donotfax"] = true,
                ["new_decimal"] = Convert.ToDecimal(100.56),
                ["importsequencenumber"] = Convert.ToInt32(100)
            };
            using (var crmClient = new ServiceClient(System.Environment.GetEnvironmentVariable("Dataverse")))
            {
                try
                {
                    var crmId = crmClient.Create(contact);
                }
                catch (Exception ex)
                {

                    throw new Exception(ex.Message);
                }

            }          
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");
        }
    }
}
