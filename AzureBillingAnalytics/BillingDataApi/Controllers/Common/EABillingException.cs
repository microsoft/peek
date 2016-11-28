using System;

namespace BillingDataApi.Common
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class EaBillingException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        public EaBillingException()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public EaBillingException(string message) : base(message)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="inner"></param>
        public EaBillingException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}