// -----------------------------------------------------------------------
// <copyright file="CspBillingController.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>This api controller class exposes usage line items from the invoices already generated (historic usage)..</summary>
// -----------------------------------------------------------------------

namespace BillingDataApi.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Web.Http;
    using Common;
    using CspHelpers;
    using Helpers.ParameterValidators;
    using Microsoft.Store.PartnerCenter.Models;
    using Microsoft.Store.PartnerCenter.Models.Invoices;
    using Models;

    /// <summary>
    /// This api controller exposes usage line items from the invoices already generated (historic usage).
    /// </summary>
    [RoutePrefix("api/cspbilling")]
    public class CspBillingController : ApiController
    {
        /// <summary>
        /// Authentication helper which generates the token for accessing Partner Center APIs. 
        /// </summary>
        private AuthenticationHelper authHelper = new AuthenticationHelper();

        /// <summary>
        /// Configuration Manager object to read values from the configuration file.
        /// </summary>
        private static ConfigurationManager SettingsConfiguration => ConfigurationManager.Instance;

        /// <summary>
        /// Api which returns CSP Usage line items for all months and years since the CSP account has
        /// been active.
        /// </summary>
        /// <returns>CSP Usage line items for all months and years since the CSP account has been active.</returns>
        [Route(@"")]
        public IEnumerable<CspUsageLineItem> GetAllData()
        {
            try
            {
                List<InvoiceLineItem> completeList = this.GetCompleteList(0, 0, 0, 0);
                List<CspUsageLineItem> dataToBeStored = new List<CspUsageLineItem>();

                // extract properties
                foreach (InvoiceLineItem lineItem in completeList)
                {
                    CspUsageLineItem cspUsageItem = this.GetSingleInvoiceData(lineItem);
                    dataToBeStored.Add(cspUsageItem);
                }

                return dataToBeStored;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Api which returns CSP Usage line items for the current month only.
        /// </summary>
        /// <returns>CSP Usage line items for the current month.</returns>
        [Route(@"currentmonth")]
        public IEnumerable<CspUsageLineItem> GetCurrentMonthData()
        {
            try
            {
                List<InvoiceLineItem> completeList = this.GetCompleteList(DateTime.Now.Month, DateTime.Now.Year, 0, 0);
                List<CspUsageLineItem> dataToBeStored = new List<CspUsageLineItem>();
                
                // extract properties
                foreach (InvoiceLineItem lineItem in completeList)
                {
                    CspUsageLineItem cspUsageItem = this.GetSingleInvoiceData(lineItem);
                    dataToBeStored.Add(cspUsageItem);
                }

                return dataToBeStored;
            }
            catch (Exception ex)
            {
                throw new CspBillingException(ex.Message, ex);
            }
        }

        /// <summary>
        /// Api which returns CSP Usage line items for the given month input.
        /// </summary>
        /// <param name="monthYear">Provide month and year in "mm-yyyy" format. Example: "02-2016".</param>
        /// <returns>CSP Usage line items for the given month input.</returns>
        [Route(@"bymonth/{monthYear}")]
        public IEnumerable<CspUsageLineItem> GetSingleMonthData([FromUri] string monthYear)
        {
            UrlParameterValidation.ValidateMonthYearFormat(monthYear);
            try
            {
                int month = int.Parse(monthYear.Split('-')[0], CultureInfo.InvariantCulture);
                int year = int.Parse(monthYear.Split('-')[1], CultureInfo.InvariantCulture);

                List<InvoiceLineItem> completeList = this.GetCompleteList(month, year, 0, 0);
                List<CspUsageLineItem> dataToBeStored = new List<CspUsageLineItem>();
                
                // extract properties
                foreach (InvoiceLineItem lineItem in completeList)
                {
                    CspUsageLineItem cspUsageItem = this.GetSingleInvoiceData(lineItem);
                    dataToBeStored.Add(cspUsageItem);
                }

                return dataToBeStored;
            }
            catch (Exception ex)
            {
                throw new CspBillingException(ex.Message, ex);
            }
        }

        /// <summary>
        /// Api which returns CSP Usage line items for the given month range input.
        /// </summary>
        /// <param name="startMonthYear">Provide range start month and year in "mm-yyyy" format.</param>
        /// <param name="endMonthYear">Provide range end month and year in "mm-yyyy" format.</param>
        /// <returns>CSP Usage line items for the given month range input.</returns>
        [Route(@"bymonthrange/{startMonthYear}/{endMonthYear}")]
        public IEnumerable<CspUsageLineItem> GetDataForMonthRange([FromUri] string startMonthYear, [FromUri] string endMonthYear)
        {
            UrlParameterValidation.ValidateMonthYearFormat(startMonthYear);
            UrlParameterValidation.ValidateMonthYearFormat(endMonthYear);
            UrlParameterValidation.ValidateStartEndDate(startMonthYear, endMonthYear);
            try
            {
                int startMonth = int.Parse(startMonthYear.Split('-')[0], CultureInfo.InvariantCulture);
                int startYear = int.Parse(startMonthYear.Split('-')[1], CultureInfo.InvariantCulture);
                int endMonth = int.Parse(endMonthYear.Split('-')[0], CultureInfo.InvariantCulture);
                int endYear = int.Parse(endMonthYear.Split('-')[1], CultureInfo.InvariantCulture);

                List<InvoiceLineItem> completeList = this.GetCompleteList(startMonth, startYear, endMonth, endYear);
                List<CspUsageLineItem> dataToBeStored = new List<CspUsageLineItem>();
                
                // extract properties
                foreach (InvoiceLineItem lineItem in completeList)
                {
                    CspUsageLineItem cspUsageItem = this.GetSingleInvoiceData(lineItem);
                    dataToBeStored.Add(cspUsageItem);
                }

                return dataToBeStored;
            }
            catch (Exception ex)
            {
                throw new CspBillingException(ex.Message, ex);
            }
        }

        /// <summary>
        /// Method to fetch the complete list of Detailed billing Line items from Partner center APIs, depending on the input dates provided.
        /// </summary>
        /// <param name="startMonth">Start month of date range.</param>
        /// <param name="startYear">Start year of date range.</param>
        /// <param name="endMonth">End month of date range.</param>
        /// <param name="endYear">End year of date range.</param>
        /// <returns>List of Billing Line items from Partner center APIs.</returns>
        private List<InvoiceLineItem> GetCompleteList(int startMonth, int startYear, int endMonth, int endYear)
        {
            try
            {
                // authenticate user:
                var partnerOperations = this.authHelper.UserPartnerOperations;

                // query invoices
                ResourceCollection<Invoice> invoicesPage = partnerOperations.Invoices.Get();
                List<Invoice> invoices = invoicesPage.Items.ToList();

                // authenticate app:
                // current Partner Centre APIs being used in this project are not all accessible
                // via app authentication alone. If this changes in future, it is recommended to use
                // app authentication instead. The sample code below has been provided.

                ////var partnerOperations = authHelper.AppPartnerOperations;
                ////ResourceCollection<Invoice> invoicesPage = partnerOperations.Invoices.Get();
                ////List<Invoice> invoices = invoicesPage.Items.ToList();

                int invoicePageSize = SettingsConfiguration.Scenario.InvoicePageSize;

                List<InvoiceLineItem> completeList = new List<InvoiceLineItem>();

                DateTime startDate, endDate;
                if (startMonth == 0 || startYear == 0)
                {
                    startDate = new DateTime(1900, 01, 01);
                    endDate = new DateTime(2100, 01, 01);
                }
                else
                {
                    startDate = new DateTime(startYear, startMonth, 01);
                    if (endMonth == 0 || endYear == 0)
                    {
                        endDate = new DateTime(startYear, startMonth, 1).AddMonths(1);
                    }
                    else
                    {
                        endDate = new DateTime(endYear, endMonth, 1).AddMonths(1);
                    }
                }

                foreach (Invoice invoiceItem in invoices)
                {
                    if (invoiceItem.InvoiceDate >= startDate && invoiceItem.InvoiceDate < endDate)
                    {
                        var invoiceOperations = partnerOperations.Invoices.ById(invoiceItem.Id);
                        var invoice = invoiceOperations.Get();

                        if ((invoice.InvoiceDetails != null) && (invoice.InvoiceDetails.Count() > 0))
                        {
                            foreach (var invoiceDetail in invoice.InvoiceDetails)
                            {
                                if (invoiceDetail.BillingProvider == BillingProvider.Azure &&
                                    invoiceDetail.InvoiceLineItemType == InvoiceLineItemType.UsageLineItems)
                                {
                                    // Get the invoice line items
                                    var invoiceLineItemsCollection = (invoicePageSize <= 0)
                                        ? invoiceOperations.By(
                                            invoiceDetail.BillingProvider,
                                            invoiceDetail.InvoiceLineItemType).Get()
                                        : invoiceOperations.By(
                                            invoiceDetail.BillingProvider,
                                            invoiceDetail.InvoiceLineItemType).Get(invoicePageSize, 0);

                                    var invoiceLineItemEnumerator =
                                        partnerOperations.Enumerators.InvoiceLineItems.Create(invoiceLineItemsCollection);

                                    while (invoiceLineItemEnumerator.HasValue)
                                    {
                                        var current = invoiceLineItemEnumerator.Current;
                                        completeList.AddRange(current.Items.ToList());

                                        // Get the next list of invoice line items
                                        invoiceLineItemEnumerator.Next();
                                    }
                                }
                            }
                        }
                    }
                }

                return completeList;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Converts an object of type CspUsageLineItem from the input InvoiceLineItem values.
        /// </summary>
        /// <param name="lineItem">InvoiceLineItem object which needs to be converted.</param>
        /// <returns>Converted CspUsageLineItem.</returns>
        private CspUsageLineItem GetSingleInvoiceData(InvoiceLineItem lineItem)
        {
            try
            {
                CspUsageLineItem cspUsageItem = new CspUsageLineItem();
                DailyUsageLineItem lineItemWithBaseProperties = (DailyUsageLineItem)lineItem;

                cspUsageItem.BillingProvider = lineItemWithBaseProperties.BillingProvider.ToString();
                cspUsageItem.ChargeEndDate = lineItemWithBaseProperties.ChargeEndDate;
                cspUsageItem.ChargeStartDate = lineItemWithBaseProperties.ChargeStartDate;
                cspUsageItem.ConsumedQuantity =
                    float.Parse(
                        lineItemWithBaseProperties.ConsumedQuantity.ToString(CultureInfo.InvariantCulture),
                        CultureInfo.InvariantCulture);
                cspUsageItem.CustomerBillableAccount = lineItemWithBaseProperties.CustomerBillableAccount;
                cspUsageItem.CustomerCompanyName = lineItemWithBaseProperties.CustomerCompanyName;
                cspUsageItem.invoiceNumber = lineItemWithBaseProperties.InvoiceNumber;
                cspUsageItem.MpnId = lineItemWithBaseProperties.MpnId.ToString(CultureInfo.InvariantCulture);
                cspUsageItem.orderId = lineItemWithBaseProperties.OrderId;
                cspUsageItem.PartnerBillableAccountId = lineItemWithBaseProperties.PartnerBillableAccountId;
                cspUsageItem.PartnerId = lineItemWithBaseProperties.PartnerId;
                cspUsageItem.PartnerName = lineItemWithBaseProperties.PartnerName;
                cspUsageItem.Region = lineItemWithBaseProperties.Region;
                cspUsageItem.ResourceGuid = lineItemWithBaseProperties.ResourceGuid;
                cspUsageItem.ResourceName = lineItemWithBaseProperties.ResourceName;
                cspUsageItem.ServiceName = lineItemWithBaseProperties.ServiceName;
                cspUsageItem.ServiceType = lineItemWithBaseProperties.ServiceType;
                cspUsageItem.SubscriptionDescription = lineItemWithBaseProperties.SubscriptionDescription;
                cspUsageItem.SubscriptionId = lineItemWithBaseProperties.SubscriptionId;
                cspUsageItem.SubscriptionName = lineItemWithBaseProperties.SubscriptionName;
                cspUsageItem.TierMpnId = lineItemWithBaseProperties.Tier2MpnId.ToString(CultureInfo.InvariantCulture);
                cspUsageItem.UsageDate = lineItemWithBaseProperties.UsageDate;

                return cspUsageItem;
            }
            catch
            {
                throw;
            }
        }
    }
}