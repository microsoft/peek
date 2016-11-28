// -----------------------------------------------------------------------
// <copyright file="UserAuthenticationSection.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>This class holds Holds a user authentication section settings.</summary>
// -----------------------------------------------------------------------

namespace BillingDataApi.CspHelpers
{
    using System;

    /// <summary>
    /// Holds a user authentication section settings.
    /// </summary>
    public class UserAuthenticationSection : Section
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserAuthenticationSection"/> class.
        /// </summary>
        /// <param name="sectionName">The application authentication section name.</param>
        public UserAuthenticationSection(string sectionName) : base(sectionName)
        {
        }

        /// <summary>
        /// Gets the AAD application ID.
        /// </summary>
        /// <value>AAD application ID.</value>
        public string ApplicationId
        {
            get { return this.ConfigurationSection["ApplicationId"]; }
        }

        /// <summary>
        /// Gets the resource the application is attempting to access, i.e. the partner API service.
        /// </summary>
        /// <value>The resource the application is attempting to access.</value>
        public Uri ResourceUrl
        {
            get { return new Uri(this.ConfigurationSection["ResourceUrl"]); }
        }

        /// <summary>
        /// Gets the application redirect URL.
        /// </summary>
        /// <value>The application redirect URL.</value>
        public Uri RedirectUrl
        {
            get { return new Uri(this.ConfigurationSection["RedirectUrl"]); }
        }

        /// <summary>
        /// Gets the AAD user name.
        /// </summary>
        /// <value>AAD user name.</value>
        public string UserName
        {
            get { return this.ConfigurationSection["UserName"]; }
        }

        /// <summary>
        /// Gets the AAD password.
        /// </summary>
        /// <value>AAD password.</value>
        public string Password
        {
            get { return this.ConfigurationSection["Password"]; }
        }
    }
}