// -----------------------------------------------------------------------
// <copyright file="EaBillingException.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>EA Billing Exception class.</summary>
// -----------------------------------------------------------------------

namespace BillingDataApi.Common
{
    using System;

    /// <summary>
    /// EA Billing Exception class.
    /// </summary>
    [Serializable]
    public class EaBillingException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EaBillingException" /> class.
        /// </summary>
        public EaBillingException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EaBillingException" /> class.
        /// EA Billing Exception which accepts message.
        /// </summary>
        /// <param name="message">Exception message.</param>
        public EaBillingException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EaBillingException" /> class.
        /// EA Billing Exception which accepts message and inner exception.
        /// </summary>
        /// <param name="message">Exception message.</param>
        /// <param name="inner">Inner exception.</param>
        public EaBillingException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}