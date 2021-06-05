using System;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace HelloWorldPlugin
{
    public class HelloWorld : IPlugin
    {
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
                Entity entity = (Entity)context.InputParameters["Target"];

                // Obtain the organization service reference which you will need for  
                // web service calls.  
                IOrganizationServiceFactory serviceFactory =
                    (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

                try
                {
                    // Plug-in business logic goes here.

                    // Adding a shared variable in pipeline scope. We can fetch this shared variable from other plugins located in the same pipeline.
                    context.SharedVariables.Add("key1", "Some info.");

                    // Read form attributes.
                    string firstName = entity.Attributes["firstname"].ToString();
                    string lastName = entity.Attributes["lastname"].ToString();

                    // Assign data to attributes.
                    entity.Attributes.Add("description", "Hello World " + firstName + lastName);
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
