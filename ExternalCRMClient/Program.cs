using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;

namespace ExternalCRMClient
{
    class Program
    {
        static void Main(string[] args)
        {
            // e.g. https://yourorg.crm.dynamics.com
            string url = "https://ebuinvest.crm.dynamics.com";
            // e.g. you@yourorg.onmicrosoft.com
            string userName = "contact@ebuinvest.com";
            // e.g. y0urp455w0rd 
            string password = String.Empty;

            string conn = $@"
            Url = {url};
            AuthType = OAuth;
            UserName = {userName};
            Password = {password};
            AppId = 51f81489-12ee-4a9e-aaae-a2591f45987d;
            RedirectUri = app://58145B91-0C36-4500-8554-080854F2AC97;
            LoginPrompt=Auto;
            RequireNewInstance = True";
            
            using (var svc = new CrmServiceClient(conn))
            {
                // TODO: Look into executing custom requests on the webserver, and look into executing multiple requests at the same time.
                // Related link: https://docs.microsoft.com/en-us/dotnet/api/microsoft.xrm.sdk.messages?view=dynamics-general-ce-9

                // Creating a new contact
                // Late Binding

                //Entity newContact = new Entity("contact");
                //newContact.Attributes.Add("firstname", "ConsoleApp");
                //newContact.Attributes.Add("lastname", "Client");

                //Guid newContactGUID = svc.Create(newContact);
                // -- END of THIS SECTION --

                // Creating a new contact
                // Early Binding
                //Contact contact = new Contact();
                //contact.FirstName = "CreatingWith";
                //contact.LastName = "EarlyBindedClass";

                //Guid newContactGUID = svc.Create(contact);
                // -- END of THIS SECTION --

                // Getting the number of total leads by using aggregation.
                //string query = @"<fetch distinct='false' mapping='logical' aggregate='true'>   
                //                   <entity name='lead'>   
                //                      <attribute name='leadid' aggregate='count' alias='NumberOfLeads' />   
                //                   </entity>   
                //                </fetch>";

                //EntityCollection collection = svc.RetrieveMultiple(new FetchExpression(query));

                //foreach (Entity item in collection.Entities)
                //{
                //    Console.WriteLine(((AliasedValue)item.Attributes["NumberOfLeads"]).Value.ToString());
                //}
                // -- END of THIS SECTION -- 

                // Getting filtered contacts with FetchXML generated via Dynamics 365 UI
                //string query = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                //                  <entity name='contact'>
                //                    <attribute name='fullname' />
                //                    <attribute name='emailaddress1' />
                //                    <attribute name='parentcustomerid' />
                //                    <attribute name='telephone1' />
                //                    <attribute name='statecode' />
                //                    <attribute name='contactid' />
                //                    <filter type='and'>
                //                      <condition attribute='emailaddress1' operator='not-null' />
                //                      <condition attribute='address1_city' operator='eq' value='Redmond' />
                //                    </filter>
                //                  </entity>
                //                </fetch>";

                //EntityCollection filteredContacts = svc.RetrieveMultiple(new FetchExpression(query));

                //foreach(Entity contact in filteredContacts.Entities)
                //{
                //    Console.WriteLine(contact.Attributes["fullname"].ToString());
                //}

                //Console.WriteLine("New created Contact Entity's GUID: {0}", newContactGUID);
                // -- END of THIS SECTION -- 

                // Fetching Data with LINQ - Pull Contacts (Early Binding)
                using (svcContext context = new svcContext(svc))
                {

                    // Complex Query
                    var complexQueryResults = from a in context.AccountSet
                                              join c in context.ContactSet
                                              on a.AccountId equals c.ParentCustomerId.Id
                                              where c.ParentCustomerId != null
                                              select new
                                              {
                                                  FullName = c.FullName,
                                                  AccountName = a.Name
                                              };

                    foreach (var record in complexQueryResults)
                    {
                        if (record.FullName != null && record.AccountName != null)
                            Console.WriteLine(record.FullName.ToString() + " - " + record.AccountName.ToString());
                    }

                }
                // Fetching Data with LINQ - Pull Contacts (Late Binding)
                //using (OrganizationServiceContext context = new OrganizationServiceContext(svc))
                //{ 
                //    // Simple Query
                //    var simpleQueryResults = from contact in context.CreateQuery("contact")
                //                             where contact["address1_city"].Equals("Redmond")
                //                             select contact;

                //    // Complex Query
                //    var complexQueryResults = from c in context.CreateQuery("contact")
                //                              join
                //                              a in context.CreateQuery("account")
                //                              on c["parentcustomerid"] equals a["accountid"]
                //                              where c["parentcustomerid"] != null
                //                              select new
                //                              {
                //                                  FullName = c["fullname"],
                //                                  AccountName = a["name"]
                //                              };

                //    foreach(var record in complexQueryResults)
                //    {
                //        if (record.FullName != null && record.AccountName != null)
                //            Console.WriteLine(record.FullName.ToString() + " - " + record.AccountName.ToString());
                //    }

                //}
                // -- END of THIS SECTION -- 


                Console.WriteLine("Press any key to exit.");
                Console.ReadLine();
            }
        }
    }
}
