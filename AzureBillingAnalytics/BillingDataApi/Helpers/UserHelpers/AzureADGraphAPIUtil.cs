// -----------------------------------------------------------------------
// <copyright file="AzureADGraphAPIUtil.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>Helper class for all interactions with Azure AD Graph APIs for Direct User scenario.</summary>
// -----------------------------------------------------------------------

namespace BillingDataApi.UserHelpers
{
    using System.Configuration;
    using System.Globalization;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Security.Claims;
    using System.Web.Helpers;
    using Microsoft.IdentityModel.Clients.ActiveDirectory;

    /// <summary>
    /// Helper class for all interactions with Azure AD Graph APIs for Direct User scenario.
    /// </summary>
    public static class AzureADGraphAPIUtil
    {
        /// <summary>
        /// Method to get the organization Display Name for the input value of organization id.
        /// </summary>
        /// <param name="organizationId">Organization Id.</param>
        /// <returns>Organization display name.</returns>
        public static string GetOrganizationDisplayName(string organizationId)
        {
            string displayName = null;

            try
            {
                // Acquire Access Token to call Azure AD Graph API
                ClientCredential credential = new ClientCredential(
                    ConfigurationManager.AppSettings["ida:ClientID"],
                    ConfigurationManager.AppSettings["ida:Password"]);

                // initialize AuthenticationContext with the token cache of the currently signed in user
                AuthenticationContext authContext = new AuthenticationContext(
                    string.Format(
                        CultureInfo.InvariantCulture, 
                        ConfigurationManager.AppSettings["ida:Authority"],
                        organizationId));

                AuthenticationResult result =
                    authContext.AcquireToken(ConfigurationManager.AppSettings["ida:GraphAPIIdentifier"], credential);

                // Get a list of Organizations of which the user is a member
                string requestUrl = string.Format(
                    CultureInfo.InvariantCulture,
                    "{0}{1}/tenantDetails?api-version={2}",
                    ConfigurationManager.AppSettings["ida:GraphAPIIdentifier"],
                    organizationId,
                    ConfigurationManager.AppSettings["ida:GraphAPIVersion"]);

                // Make the GET request
                HttpResponseMessage response = GetRequestResponse(result, requestUrl);

                // Endpoint returns JSON with an array of Tenant Objects
                if (response.IsSuccessStatusCode)
                {
                    string responseContent = response.Content.ReadAsStringAsync().Result;
                    var organizationPropertiesResult = Json.Decode(responseContent).value;
                    if (organizationPropertiesResult != null && organizationPropertiesResult.Length > 0)
                    {
                        displayName = organizationPropertiesResult[0].displayName;
                        if (organizationPropertiesResult[0].verifiedDomains != null)
                        {
                            foreach (var verifiedDomain in organizationPropertiesResult[0].verifiedDomains)
                            {
                                if (verifiedDomain["default"])
                                {
                                    displayName += " (" + verifiedDomain.name + ")";
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                // intentionally swallowing the exception - this info is optional and should not be breaking the routine if unavailable.
            }

            return displayName;
        }

        /// <summary>
        /// Method to get Principal Id for given application and organization.
        /// </summary>
        /// <param name="organizationId">Organization Id.</param>
        /// <param name="applicationId">Application Id.</param>
        /// <returns>Principal id.</returns>
        public static string GetObjectIdOfServicePrincipalInOrganization(string organizationId, string applicationId)
        {
            string objectId = null;

            try
            {
                // Aquire App Only Access Token to call Azure Resource Manager - Client Credential OAuth Flow
                ClientCredential credential = new ClientCredential(
                    ConfigurationManager.AppSettings["ida:ClientID"],
                    ConfigurationManager.AppSettings["ida:Password"]);

                // initialize AuthenticationContext with the token cache of the currently signed in user
                AuthenticationContext authContext =
                    new AuthenticationContext(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            ConfigurationManager.AppSettings["ida:Authority"], 
                            organizationId));

                AuthenticationResult result =
                    authContext.AcquireToken(ConfigurationManager.AppSettings["ida:GraphAPIIdentifier"], credential);

                // Get a list of Organizations of which the user is a member
                string requestUrl = string.Format(
                    CultureInfo.InvariantCulture,
                    "{0}{1}/servicePrincipals?api-version={2}&$filter=appId eq '{3}'",
                    ConfigurationManager.AppSettings["ida:GraphAPIIdentifier"], 
                    organizationId,
                    ConfigurationManager.AppSettings["ida:GraphAPIVersion"], 
                    applicationId);

                // Make the GET request
                HttpResponseMessage response = GetRequestResponse(result, requestUrl);

                // Endpoint should return JSON with one or none serviePrincipal object
                if (response.IsSuccessStatusCode)
                {
                    string responseContent = response.Content.ReadAsStringAsync().Result;
                    var servicePrincipalResult = Json.Decode(responseContent).value;
                    if (servicePrincipalResult != null && servicePrincipalResult.Length > 0)
                    {
                        objectId = servicePrincipalResult[0].objectId;
                    }
                }
            }
            catch
            {
                // intentionally swallowing the exception - this info is optional and should not be breaking the routine if unavailable.
            }

            return objectId;
        }

        /// <summary>
        /// Method to get display name of organization based on Organization id and AAD object Id.
        /// </summary>
        /// <param name="organizationId">Organization Id.</param>
        /// <param name="objectId">AAD Object Id.</param>
        /// <returns>Display Name of the organization.</returns>
        public static string LookupDisplayNameOfAADObject(string organizationId, string objectId)
        {
            string objectDisplayName = null;

            string signedInUserUniqueName =
                ClaimsPrincipal.Current.FindFirst(ClaimTypes.Name).Value.Split('#')[ClaimsPrincipal.Current.FindFirst(ClaimTypes.Name).Value.Split('#').Length - 1];

            // Acquire Access Token to call Azure AD Graph API
            ClientCredential credential = new ClientCredential(
                ConfigurationManager.AppSettings["ida:ClientID"],
                ConfigurationManager.AppSettings["ida:Password"]);
            
            // Initialize AuthenticationContext with the token cache of the currently signed in user, as kept in the app's EF DB
            AuthenticationContext authContext = new AuthenticationContext(
                string.Format(
                    CultureInfo.InvariantCulture, 
                    ConfigurationManager.AppSettings["ida:Authority"],
                    organizationId));

            AuthenticationResult result =
                authContext.AcquireTokenSilent(
                    ConfigurationManager.AppSettings["ida:GraphAPIIdentifier"], 
                    credential,
                    new UserIdentifier(
                        signedInUserUniqueName, 
                        UserIdentifierType.RequiredDisplayableId));

            string doQueryUrl = string.Format(
                CultureInfo.InvariantCulture,
                "{0}{1}/directoryObjects/{2}?api-version={3}",
                ConfigurationManager.AppSettings["ida:GraphAPIIdentifier"], 
                organizationId,
                objectId, 
                ConfigurationManager.AppSettings["ida:GraphAPIVersion"]);

            HttpResponseMessage response = GetRequestResponse(result, doQueryUrl);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = response.Content;
                string responseString = responseContent.ReadAsStringAsync().Result;
                var directoryObject = System.Web.Helpers.Json.Decode(responseString);
                if (directoryObject != null)
                {
                    objectDisplayName = string.Format(
                        "{0} ({1})",
                        directoryObject.displayName,
                        directoryObject.objectType);
                }
            }

            return objectDisplayName;
        }

        /// <summary>
        /// Method to add auth header, send the request and return the API response.
        /// </summary>
        /// <param name="result">Auth token container object.</param>
        /// <param name="requestUrl">Request Url.</param>
        /// <returns>Response object.</returns>
        private static HttpResponseMessage GetRequestResponse(AuthenticationResult result, string requestUrl)
        {
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);
            HttpResponseMessage response = client.SendAsync(request).Result;
            return response;
        }
    }
}