// -----------------------------------------------------------------------
// <copyright file="ConfigurableCronSchedule.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// ----------------------------------------------------------------------- 

using System.Configuration;

namespace BillingWebJob.Helpers
{
    /// <summary>
    /// Create a schedule for Cron Job
    /// </summary>
    public class ConfigurableCronSchedule : Microsoft.Azure.WebJobs.Extensions.Timers.CronSchedule
    {
        internal static string CronExpression;

        /// <summary>
        /// Constructor for the class
        /// </summary>
        public ConfigurableCronSchedule() : base(CronExpression)
        {
        }

        /// <summary>
        /// Configure the Cron Job from 'Frequency' in config file
        /// </summary>
        static ConfigurableCronSchedule()
        {
            string frequencyValue = ConfigurationManager.AppSettings["Frequency"];
            CronExpression = ScheduleHelper.GetCronExpression(frequencyValue);
            //// cronexpression = "*/10 * * * * *";
        }
    }
}