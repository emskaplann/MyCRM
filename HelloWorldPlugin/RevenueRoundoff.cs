using System;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace HelloWorldPlugin
{
    public class RevenueRoundoff : IPlugin
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
                Entity account = (Entity)context.InputParameters["Target"];

                // Obtain the organization service reference which you will need for  
                // web service calls.  
                IOrganizationServiceFactory serviceFactory =
                    (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

                try
                {
                    tracingService.Trace("Context Depth: {0}", context.Depth.ToString());
                    if (context.Depth > 1)
                        return;

                    // Plug-in business logic goes here.
                    if (account.Attributes["revenue"] != null)
                    {
                        decimal revenue = ((Money)account.Attributes["revenue"]).Value;
                        revenue = Math.Round(revenue, 0);

                        account.Attributes["revenue"] = new Money(revenue);
                    }

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
