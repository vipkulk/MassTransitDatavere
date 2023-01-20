# MassTransitDatavere
This library is useful when you want to develop Publish/Subscribe pattern based api to push data inside Dataverse.This libarary provides a wrapper around organization service of dataverse, methods exposed by this library are same as methods of organization service, however in background operations done in asynchronous manner. Library handles creation of Azure resources (Service Bus Topics for OperaionCompleted data, Error data) required for its functioning and you can subcribe to those topics to access operation status and related data.

Generally below pattern is followed to while developing Publish/Subscribe pattern based api for Dataverse

![image](https://user-images.githubusercontent.com/69874658/212418895-240b48c9-1a66-4690-bd4c-f9fa8a0a2ed2.png)

In this case you need to maintain Azure Infrastructure yourself and if you want to increase number of apis you have to keep on increasing Azure functions/webjobs and keep maintianing them.

This library maintains Azure infrastructure for you and gives generic interface like IOrganizationService to create or update any data in Dataverse

## Architecture  with library

![image](https://user-images.githubusercontent.com/69874658/212421107-e39449fb-7602-43f2-9cd2-fc7ff84b149a.png)


## How to Use Library
 
If you are using .Net6 or above:

Add below using statement on the top of Progarm.cs file
 ```c#
 using MassTransit;
 ```
Add below lines in Program.cs if you are using .Net6 or above
 ```c#
builder.Services.ConfigureTransit(builder.Configuration["Dataverse"],BusType.AzureServiceBus,azureServicebusConfiguration:(context, cfg) =>
{
    cfg.Host(builder.Configuration["ServiceBus"]);
    cfg.ConfigureEndpoints(context);
});
```

If you are using .Net5 Or below: 

Add below using statement on the top of Startup.cs file
 ```c#
 using MassTransit;
 ```
Add below lines under ConfigureServices method in Startup.cs file
 ```c#
services.ConfigureTransit(Configuration["Dataverse"], BusType.AzureServiceBus, azureServicebusConfiguration: (context, cfg) =>
{
                cfg.Host(Configuration["ServiceBus"]);
                cfg.ConfigureEndpoints(context);
});
 ```
 
 
 Here 
 "Dataverse" : Connection string for datverse please refer this link for more information https://docs.microsoft.com/en-us/powerapps/developer/common-data-service/xrm-tooling/use-connection-strings-xrm-tooling-connect 
 
 "ServiceBus": Connection string for Azure Service Bus Namespace. Please make sure SAS connection string has access to create Azure Service Bus Topics. Please use Azure Service Bus on Standard Tier. 




