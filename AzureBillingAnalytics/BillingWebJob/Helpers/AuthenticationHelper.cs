// -----------------------------------------------------------------------
// <copyright file="ParameterFilter.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace BillingWebJob.Helpers
{
    using System.Configuration;
    using System.Globalization;
    using Microsoft.IdentityModel.Clients.ActiveDirectory;
    using System;    
    /// <summary>
    /// Assists in authentication from AAD
    /// </summary>
    public static class AuthenticationHelper
    {
        /// <summary>
        /// Authenticate using WebApiAADClientSecret from config file
        /// </summary>
        /// <returns>Authentication Id</returns>
        public static AuthenticationResult GetAuthenticationResult()
        {
            try
            {
                string tenantId = ConfigurationManager.AppSettings["WebApiAADDomainName"];

                ClientCredential credential = new ClientCredential(
                    ConfigurationManager.AppSettings["WebApiAADClientId"],
                    ConfigurationManager.AppSettings["WebApiAADClientSecret"]);

                // initialize AuthenticationContext with the token cache of the currently signed in user
                AuthenticationContext authContext = new AuthenticationContext(
                    string.Format(
                        CultureInfo.InvariantCulture, ConfigurationManager.AppSettings["AADTokenAuthority"], 
                        tenantId));

                AuthenticationResult result =
                    authContext.AcquireToken(ConfigurationManager.AppSettings["WebApiAADClientId"], credential);

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(
                    "Could not generate AAD token successfully, please check if correct values for AAD Domain, Client id and Secret have been provided",
                    ex);
            }
        }
    }
}