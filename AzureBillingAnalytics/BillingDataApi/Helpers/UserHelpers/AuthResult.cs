// -----------------------------------------------------------------------
// <copyright file="AuthResult.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>Auth helper class for auth token creation in the direct user scenario.</summary>
// -----------------------------------------------------------------------

namespace BillingDataApi.UserHelpers
{
    using System.Configuration;
    using System.Globalization;
    using Microsoft.IdentityModel.Clients.ActiveDirectory;

    /// <summary>
    /// Auth helper class for auth token creation in the direct user scenario.
    /// </summary>
    public static class AuthResult
    {
        /// <summary>
        /// Method to generate auth token for direct user scenario.
        /// </summary>
        /// <returns>Auth result with token.</returns>
        public static AuthenticationResult GetAuthenticationResult()
        {
            string tenantId = ConfigurationManager.AppSettings["ida:tenantID"];

            ClientCredential credential = new ClientCredential(
                ConfigurationManager.AppSettings["ida:ClientID"],
                ConfigurationManager.AppSettings["ida:Password"]);
            
            // initialize AuthenticationContext with the token cache of the currently signed in user, as kept in the app's EF DB
            AuthenticationContext authContext = new AuthenticationContext(
                string.Format(CultureInfo.InvariantCulture, ConfigurationManager.AppSettings["ida:Authority"], tenantId));

            AuthenticationResult result =
                authContext.AcquireToken(
                    ConfigurationManager.AppSettings["ida:AzureResourceManagerIdentifier"],
                    credential);

            return result;
        }
    }
}