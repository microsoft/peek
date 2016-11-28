using System;

namespace BillingDataApi.Common
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class CspBillingException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        public CspBillingException()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public CspBillingException(string message) : base(message)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="inner"></param>
        public CspBillingException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}