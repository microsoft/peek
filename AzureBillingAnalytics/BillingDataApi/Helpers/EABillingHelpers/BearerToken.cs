// -----------------------------------------------------------------------
// <copyright file="BearerToken.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>This class is a representation of a bearer token, the token in the authorization header.</summary>
// -----------------------------------------------------------------------

namespace BillingDataApi.Helpers.EABillingHelpers
{
    using System;

    /// <summary>
    /// This class is a representation of a bearer token, the token in the authorization header.
    /// </summary>
    public class BearerToken
    {
        /// <summary>
        /// Prevents a default instance of the <see cref="BearerToken" /> class from being created.
        /// </summary>
        private BearerToken()
        {
        }

        /// <summary>
        /// Gets the access key.
        /// </summary>
        /// <value>Access key.</value>
        public string Token { get; private set; }

        /// <summary>
        /// Gets bearer and access key.
        /// </summary>
        /// <value>Bearer and access key.</value>
        public string BearerTokenHeader { get; private set; }

        /// <summary>
        /// Parse the authorization header into BearerToken, which is used as auth token in our case.
        /// header looks like
        /// authorization: bearer {access key}.
        /// </summary>
        /// <param name="tokenHeader">Token header as a string.</param>
        /// <returns>Bearer token.</returns>
        public static BearerToken Parse(string tokenHeader)
        {
            if (!tokenHeader.StartsWith("bearer", StringComparison.InvariantCultureIgnoreCase) || tokenHeader.Length < 8)
            {
                // meaning the string after "bearer " is empty
                throw new InvalidOperationException("not a valid bearer token");
            }

            return new BearerToken() { Token = tokenHeader.Substring(7), BearerTokenHeader = tokenHeader };
        }

        /// <summary>
        /// Convert access key string to BearerToken object.
        /// </summary>
        /// <param name="accessKey">Access key as string value.</param>
        /// <returns>Bearer token.</returns>
        public static BearerToken FromAccessKey(string accessKey)
        {
            string bearerToken = string.Concat("bearer ", accessKey);
            return Parse(bearerToken);
        }
    }
}