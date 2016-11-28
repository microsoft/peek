// -----------------------------------------------------------------------
// <copyright file="ScheduleHelper.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// ----------------------------------------------------------------------- 

using System;

namespace BillingWebJob
{
    /// <summary>
    /// Helper to schedule cron job
    /// </summary>
    public static class ScheduleHelper
    {
        /// <summary>
        /// To get the cron expression for a particular frequency to configure it's scheduling
        /// </summary>
        /// <param name="frequencyValue"></param>
        /// <returns>cron expression</returns>
        public static string GetCronExpression(string frequencyValue)
        {
            Frequency frequency = (Frequency) Enum.Parse(typeof(Frequency), frequencyValue, true);

            switch (frequency)
            {
                case Frequency.Daily:
                    return "0 0 9 * * *"; // everyday at 9 am
                case Frequency.EveryWorkWeekday:
                    return "0 0 9 * * 1-5";
                case Frequency.Hourly:
                    return "0 0 * * * *";
                case Frequency.Monthly: // 8th of every month
                    return "0 0 9 8 * *";
                case Frequency.EveryMinute:
                    return "0 */1 * * * *";
                case Frequency.EveryThirtySeconds:
                    return "*/30 * * * * *";
                default:
                    return "0 0 9 * * *";
            }
        }
    }
}