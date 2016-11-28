// -----------------------------------------------------------------------
// <copyright file="ScenarioSettingsSection.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>This class holds the scenario specific settings section.</summary>
// -----------------------------------------------------------------------

namespace BillingDataApi.CspHelpers
{
    using System.Globalization;

    /// <summary>
    /// Holds the scenario specific settings section.
    /// </summary>
    public class ScenarioSettingsSection : Section
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScenarioSettingsSection"/> class.
        /// </summary>
        public ScenarioSettingsSection() : base("ScenarioSettings")
        {
        }

        /// <summary>
        /// Gets the customer domain suffix.
        /// </summary>
        /// <value>Customer domain suffix.</value>
        public string CustomerDomainSuffix
        {
            get { return this.ConfigurationSection["CustomerDomainSuffix"]; }
        }

        /// <summary>
        /// Gets the ID of the customer to delete from the TIP account.
        /// </summary>
        /// <value>ID of the customer to delete from the TIP account.</value>
        public string CustomerIdToDelete
        {
            get { return this.ConfigurationSection["CustomerIdToDelete"]; }
        }

        /// <summary>
        /// Gets the ID of the customer whose details should be read.
        /// </summary>
        /// <value>ID of the customer whose details should be read.</value>
        public string DefaultCustomerId
        {
            get { return this.ConfigurationSection["DefaultCustomerId"]; }
        }

        /// <summary>
        /// Gets the number of customers to return in each customer page.
        /// </summary>
        /// <value>Number of customers to return in each customer page.</value>
        public int CustomerPageSize
        {
            get { return int.Parse(this.ConfigurationSection["CustomerPageSize"], CultureInfo.InvariantCulture); }
        }

        /// <summary>
        /// Gets the number of offers to return in each offer page.
        /// </summary>
        /// <value>Number of offers to return in each offer page.</value>
        public int DefaultOfferPageSize
        {
            get { return int.Parse(this.ConfigurationSection["DefaultOfferPageSize"], CultureInfo.InvariantCulture); }
        }

        /// <summary>
        /// Gets the number of invoices to return in each invoice page.
        /// </summary>
        /// <value>Number of invoices to return in each invoice page.</value>
        public int InvoicePageSize
        {
            get { return int.Parse(this.ConfigurationSection["InvoicePageSize"], CultureInfo.InvariantCulture); }
        }

        /// <summary>
        /// Gets the configured Invoice ID.
        /// </summary>
        /// <value>Configured Invoice ID.</value>
        public string DefaultInvoiceId
        {
            get { return this.ConfigurationSection["DefaultInvoiceId"]; }
        }

        /// <summary>
        /// Gets the configured partner MPD ID.
        /// </summary>
        /// <value>Configured partner MPD ID.</value>
        public string PartnerMpnId
        {
            get { return this.ConfigurationSection["PartnerMpnId"]; }
        }

        /// <summary>
        /// Gets the configured offer ID.
        /// </summary>
        /// <value>Configured offer ID.</value>
        public string DefaultOfferId
        {
            get { return this.ConfigurationSection["DefaultOfferId"]; }
        }

        /// <summary>
        /// Gets the configured order ID.
        /// </summary>
        /// <value>Configured order ID.</value>
        public string DefaultOrderId
        {
            get { return this.ConfigurationSection["DefaultOrderId"]; }
        }

        /// <summary>
        /// Gets the configured subscription ID.
        /// </summary>
        /// <value>Configured subscription ID.</value>
        public string DefaultSubscriptionId
        {
            get { return this.ConfigurationSection["DefaultSubscriptionId"]; }
        }

        /// <summary>
        /// Gets the service request ID.
        /// </summary>
        /// <value>Service request ID.</value>
        public string DefaultServiceRequestId
        {
            get { return this.ConfigurationSection["DefaultServiceRequestId"]; }
        }

        /// <summary>
        /// Gets the number of service requests to return in each service request page.
        /// </summary>
        /// <value>Number of service requests to return in each service request page.</value>
        public int ServiceRequestPageSize
        {
            get { return int.Parse(this.ConfigurationSection["ServiceRequestPageSize"], CultureInfo.InvariantCulture); }
        }

        /// <summary>
        /// Gets the configured support topic ID for creating new service request.
        /// </summary>
        /// <value>Configured support topic ID for creating new service request.</value>
        public string DefaultSupportTopicId
        {
            get { return this.ConfigurationSection["DefaultSupportTopicId"]; }
        }
    }
}