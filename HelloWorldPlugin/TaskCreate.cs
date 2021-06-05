using Microsoft.Xrm.Sdk;
using System;
using System.ServiceModel;

namespace HelloWorldPlugin
{
    public class TaskCreate : IPlugin
    {
        int tax;
        
        // We create constructor to fetch configuration from PluginRegistration Tool.
        public TaskCreate(string unsecureConfig, string secureConfig)
        {
            tax = Convert.ToInt32(unsecureConfig);
        }
        public void Execute(IServiceProvider serviceProvider)
        {
            // Obtain the tracing service
            ITracingService tracingService =
            (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            // Obtain the execution context from the service provider.  
            IPluginExecutionContext context = (IPluginExecutionContext)
                serviceProvider.GetService(typeof(IPluginExecutionContext));

            // The InputParameters collection contains all the data passed in the message request.  
            if (context.InputParameters.Contains("Target") &&
                context.InputParameters["Target"] is Entity)
            {
                // Obtain the target entity from the input parameters.  
                Entity contact = (Entity)context.InputParameters["Target"];

                // Obtain the organization service reference which you will need for  
                // web service calls.  
                IOrganizationServiceFactory serviceFactory =
                    (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));

                // Here we pass user id from context. Basically we can create new IOrganizationService with different User ID's
                // to run code *blocks* with under a different user than given by the default context.
                IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

                try
                {
                    // Plug-in business logic goes here.

                    // Fetching a shared variable from another plugin located in the same pipeline.
                    string sharedVariable = String.Empty;
                    if (context.SharedVariables.Contains("key1"))
                        sharedVariable = context.SharedVariables["key1"].ToString();

                    Entity newTask = new Entity("task");

                    // Single Line of Text
                    newTask.Attributes.Add("subject", "Follow up");
                    newTask.Attributes.Add("description", "Follow up with the new created contact.");
                    
                    // Date
                    newTask.Attributes.Add("scheduledend", DateTime.Now.AddDays(2));

                    // Option-set - e.g. [Low, Normal, High] -> [0, 1, 2]
                    newTask.Attributes.Add("prioritycode", new OptionSetValue(2));

                    // Link Parent Record
                    // 2 Ways to handle this
                    // 1- newTask.Attributes.Add("regardingobjectid", new EntityReference("contact", contact.Id);
                    newTask.Attributes.Add("regardingobjectid", contact.ToEntityReference());

                    Guid taskGuid = service.Create(newTask);

                    tracingService.Trace(
                        "Task created with this GUID: {0}\n" +
                        "Tax value from unsecure configuration: {1}\n" +
                        "Shared Variable from HelloWorld Plugin: {2}",
                        taskGuid.ToString(),
                        tax.ToString(),
                        sharedVariable.ToString()
                    );
                }

                catch (FaultException<OrganizationServiceFault> ex)
                {
                    throw new InvalidPluginExecutionException("An error occurred in HelloWorldPlugin.", ex);
                }

                catch (Exception ex)
                {
                    tracingService.Trace("HelloWorldPlugin: {0}", ex.ToString());
                    throw;
                }
            }
        }
    }
}
