// -----------------------------------------------------------------------
// <copyright file="UrlParameterValidation.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>Class for validating if inputs are acceptable values of month and year or not.</summary>
// -----------------------------------------------------------------------

namespace BillingDataApi.Helpers.ParameterValidators
{
    using System;
    using System.Globalization;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Class for validating if inputs are acceptable values of month and year or not.
    /// </summary>
    public class UrlParameterValidation
    {
        /// <summary>
        /// Method to validate if given input month year in in correct format (mm-yyyy) and is in accepted range - 01-1970 to 12-2099.
        /// </summary>
        /// <param name="monthYear">Input month year value as a string.</param>
        public static void ValidateMonthYearFormat(string monthYear)
        {
            Regex pattern = new Regex("^((0?(1|2|3|4|5|6|7|8|9))|(10|11|12))-(19[7-9][0-9]|20[0-9]{2})$");
            if (!pattern.IsMatch(monthYear))
            {
                throw new ArgumentException(
                    "Date time format (mm-yyyy) is not valid. Accepted range: 01-1970 to 12-2099");
            }
        }

        /// <summary>
        /// Method to validate if end date is greater than or equal to start date.
        /// </summary>
        /// <param name="start">Start date value.</param>
        /// <param name="end">End date value.</param>
        public static void ValidateStartEndDate(string start, string end)
        {
            DateTime startDate = new DateTime(
                int.Parse(start.Split('-')[1], CultureInfo.InvariantCulture),
                int.Parse(start.Split('-')[0], CultureInfo.InvariantCulture),
                1);
            DateTime endDate = new DateTime(
                int.Parse(end.Split('-')[1], CultureInfo.InvariantCulture),
                int.Parse(end.Split('-')[0], CultureInfo.InvariantCulture),
                1);
            if (startDate >= endDate)
            {
                throw new ArgumentException("Start date should be less than end date");
            }
        }
    }
}