using System;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace HelloWorldPlugin
{
    public class PreEntityImageDemo : IPlugin
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
                    string modifiedLeadBizPhone = entity.Attributes["telephone1"].ToString();

                    Entity preEntityImage = (Entity)context.PreEntityImages["PreImage"];
                    string oldLeadBizPhone = preEntityImage.Attributes["telephone1"].ToString();

                    throw new InvalidPluginExecutionException("Phone number is changed from " + oldLeadBizPhone + " to " + modifiedLeadBizPhone);
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
