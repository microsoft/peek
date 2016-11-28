// -----------------------------------------------------------------------
// <copyright file="PartnerServiceSettingsSection.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>This class holds the partner service settings section.</summary>
// -----------------------------------------------------------------------

namespace BillingDataApi.CspHelpers
{
    using System;

    /// <summary>
    /// Holds the partner service settings section.
    /// </summary>
    public class PartnerServiceSettingsSection : Section
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PartnerServiceSettingsSection"/> class.
        /// </summary>
        public PartnerServiceSettingsSection() : base("PartnerServiceSettings")
        {
        }

        /// <summary>
        /// Gets the partner service API endpoint.
        /// </summary>
        /// <value>Partner service API endpoint.</value>
        public Uri PartnerServiceApiEndpoint
        {
            get { return new Uri(this.ConfigurationSection["PartnerServiceApiEndpoint"]); }
        }

        /// <summary>
        /// Gets the authentication authority (AAD) endpoint.
        /// </summary>
        /// <value>Authentication authority (AAD) endpoint.</value>
        public Uri AuthenticationAuthorityEndpoint
        {
            get { return new Uri(this.ConfigurationSection["AuthenticationAuthorityEndpoint"]); }
        }

        /// <summary>
        /// Gets the graph API end point.
        /// </summary>
        /// <value>Graph API end point.</value>
        public Uri GraphEndpoint
        {
            get { return new Uri(this.ConfigurationSection["GraphEndpoint"]); }
        }

        /// <summary>
        /// Gets the AAD common domain.
        /// </summary>
        /// <value>AAD common domain.</value>
        public string CommonDomain
        {
            get { return this.ConfigurationSection["CommonDomain"]; }
        }
    }
}