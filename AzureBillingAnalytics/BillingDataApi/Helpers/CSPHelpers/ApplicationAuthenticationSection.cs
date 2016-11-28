// -----------------------------------------------------------------------
// <copyright file="ApplicationAuthenticationSection.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>This class holds application authentication section settings.</summary>
// -----------------------------------------------------------------------

namespace BillingDataApi.CspHelpers
{
    /// <summary>
    /// Holds application authentication section settings.
    /// </summary>
    public class ApplicationAuthenticationSection : Section
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationAuthenticationSection"/> class.
        /// </summary>
        /// <param name="sectionName">The application authentication section name.</param>
        public ApplicationAuthenticationSection(string sectionName) : base(sectionName)
        {
        }

        /// <summary>
        /// Gets the AAD application ID.
        /// </summary>
        /// <value>Application ID.</value>
        public string ApplicationId
        {
            get { return this.ConfigurationSection["ApplicationId"]; }
        }

        /// <summary>
        /// Gets AAD application secret.
        /// </summary>
        /// <value>Application Secret.</value>
        public string ApplicationSecret
        {
            get { return this.ConfigurationSection["ApplicationSecret"]; }
        }

        /// <summary>
        /// Gets AAD Domain which hosts the application.
        /// </summary>
        /// <value>Application Domain.</value>
        public string Domain
        {
            get { return this.ConfigurationSection["Domain"]; }
        }
    }
}