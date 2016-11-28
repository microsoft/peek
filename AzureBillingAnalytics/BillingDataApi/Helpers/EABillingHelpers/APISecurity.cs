// -----------------------------------------------------------------------
// <copyright file="APISecurity.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>Helper class for managing security tokens for EA connection.</summary>
// -----------------------------------------------------------------------

namespace BillingDataApi.Helpers.EABillingHelpers
{
    using System;
    using System.Configuration;
    using System.Net;

    /// <summary>
    /// Helper class for managing security tokens for EA connection.
    /// </summary>
    public class APISecurity
    {
        /// <summary>
        /// Gets the Enrollment number as declared in the configuration file.
        /// </summary>
        /// <value>Enrollment number as declared in the configuration file.</value>
        public static string EnrolmentNumber
        {
            get { return ConfigurationManager.AppSettings["EA.EnrolmentNumber"]; }
        }

        /// <summary>
        ///  Gets the API key as declared in the configuration file.
        /// </summary>
        /// <value>API key as declared in the configuration file.</value>
        public static string APIKey
        {
            get { return ConfigurationManager.AppSettings["EA.APIKey"]; }
        }

        /// <summary>
        /// Adds Auth Header to the request.
        /// </summary>
        /// <param name="request">Request as input.</param>
        /// <param name="token">Auth token.</param>
        public static void AddHeaders(WebRequest request, string token)
        {
            string bearerTokenHeader = BearerToken.FromAccessKey(token).BearerTokenHeader;
            request.Headers.Add("authorization", bearerTokenHeader);
        }
    }
}