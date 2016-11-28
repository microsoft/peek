// -----------------------------------------------------------------------
// <copyright file="CspBillingException.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>Csp Billing Exception class.</summary>
// -----------------------------------------------------------------------

namespace BillingDataApi.Common
{
    using System;

    /// <summary>
    /// Csp Billing Exception class.
    /// </summary>
    [Serializable]
    public class CspBillingException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CspBillingException" /> class.
        /// Default constructor for Csp Billing Exception.
        /// </summary>
        public CspBillingException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CspBillingException" /> class.
        /// Csp Billing Exception which accepts message.
        /// </summary>
        /// <param name="message">Exception message.</param>
        public CspBillingException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CspBillingException" /> class.
        /// Csp Billing Exception which accepts message and inner exception.
        /// </summary>
        /// <param name="message">Exception message.</param>
        /// <param name="inner">Inner Exception.</param>
        public CspBillingException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}