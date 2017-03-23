using BillingDataApi.CspHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Store.PartnerCenter.Models;
using Microsoft.Store.PartnerCenter.Models.Customers;
using System.Diagnostics;

namespace BillingDataApi.Controllers
{
    [RoutePrefix("api/csputilization")]
    public class CspUtilizationController : ApiController
    {
        private AuthenticationHelper authHelper = new AuthenticationHelper();
       
        [Route(@"")]
        // public List<Microsoft.Store.PartnerCenter.Models.Utilizations.AzureUtilizationRecord> GetDataForCustomerSubscription()
      
        public void  GetDataForCustomerSubscription()
        {
            List<Customer> customers2 = new List<Customer>();
        var partnerOperations = this.authHelper.UserPartnerOperations;
            List<Microsoft.Store.PartnerCenter.Models.Utilizations.AzureUtilizationRecord> ResourceUtilizationList = new List<Microsoft.Store.PartnerCenter.Models.Utilizations.AzureUtilizationRecord>();


            SeekBasedResourceCollection<Customer> customersPage = partnerOperations.Customers.Get();
            List<Customer> customers = customersPage.Items.ToList();

            try
            {

               foreach (var customer  in customers)
                {
                Debug.WriteLine("{0}, Thread Id= {1}", customer, Thread.CurrentThread.ManagedThreadId);

                var subscriptionsPage = partnerOperations.Customers.ById(customer.Id).Subscriptions.Get();


                    List<Microsoft.Store.PartnerCenter.Models.Subscriptions.Subscription> currentSubscriptions =
                        subscriptionsPage.Items.ToList();


                    foreach (
                        Microsoft.Store.PartnerCenter.Models.Subscriptions.Subscription subscription in
                        currentSubscriptions)
                    {


                        var subscriptionPage =
                            partnerOperations.Customers.ById(customer.Id).Subscriptions.ById(subscription.Id);


                        var utilizationRecords = subscriptionPage.Utilization.Azure.Query(
                   DateTimeOffset.Now.AddYears(-6),
                   DateTimeOffset.Now, Microsoft.Store.PartnerCenter.Models.Utilizations.AzureUtilizationGranularity.Daily, size: 500);

                        var utilizationRecordEnumerator = partnerOperations.Enumerators.Utilization.Azure.Create(utilizationRecords);

                         while (utilizationRecordEnumerator.HasValue)
                        {
                            foreach (var item in utilizationRecordEnumerator.Current.Items)
                            {
                                ResourceUtilizationList.Add(item);
                            }
                            utilizationRecordEnumerator.Next();
                        }





                    }
                    customers2.Add(customer);
                  //  Thread.Sleep(10);
                }
                   //);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                
            }
            Console.WriteLine(customers2[1].ToString());


           // var utilizationRecords = partnerOperations.Customers[""].Subscriptions[""].Utilization.Azure.Query(
           //     DateTimeOffset.Now.AddYears(-6),
           //     DateTimeOffset.Now, Microsoft.Store.PartnerCenter.Models.Utilizations.AzureUtilizationGranularity.Daily, size: 500);

           //var utilizationRecordEnumerator = partnerOperations.Enumerators.Utilization.Azure.Create(utilizationRecords);

           // List<Microsoft.Store.PartnerCenter.Models.Utilizations.AzureUtilizationRecord> ResourceUtilizationList = new List<Microsoft.Store.PartnerCenter.Models.Utilizations.AzureUtilizationRecord>();
           // while (utilizationRecordEnumerator.HasValue)
           // {
           //     foreach (var item in utilizationRecordEnumerator.Current.Items)
           //     {
           //         ResourceUtilizationList.Add(item);
           //     }
           //     utilizationRecordEnumerator.Next();
           // }

        //    return ResourceUtilizationList;

            //return "";
        }
    }
}
