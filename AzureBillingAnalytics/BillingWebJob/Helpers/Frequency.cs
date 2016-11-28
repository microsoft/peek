// -----------------------------------------------------------------------
// <copyright file="Frequency.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// ----------------------------------------------------------------------- 

namespace BillingWebJob
{
    /// <summary>
    /// Acceptable list of frequencies for CronJob
    /// </summary>
    public enum Frequency
    {
        Daily,
        Hourly,
        EveryWorkWeekday,
        EveryMinute,
        EveryThirtySeconds,
        Monthly
    }
}