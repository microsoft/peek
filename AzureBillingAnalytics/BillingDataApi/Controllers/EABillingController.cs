// -----------------------------------------------------------------------
// <copyright file="EaBillingController.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>This api controller class exposes EA related historic usage data.</summary>
// -----------------------------------------------------------------------

namespace BillingDataApi.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Web.Http;
    using Common;
    using Helpers.EABillingHelpers;
    using Helpers.ParameterValidators;
    using Models.EABillingModels;
    using Newtonsoft.Json;

    /// <summary>
    /// This api controller exposes EA related historic usage data.
    /// </summary>
    [RoutePrefix("api/eabilling")]
    public class EaBillingController : ApiController
    {
        /// <summary>
        /// URL template for EA rest APIs.
        /// </summary>
        private static string urlTemplate = ConfigurationManager.AppSettings["EA.UrlTemplate"];

        /// <summary>
        /// Base URL template for EA rest APIs.
        /// </summary>
        private static string baseUrlTemplate = ConfigurationManager.AppSettings["EA.BaseUrlTemplate"];

        /// <summary>
        /// Api which returns EA usage records for all months and years since the account has been active.
        /// </summary>
        /// <returns>All EA Usage Records.</returns>
        [Route(@"")]
        public List<BillingDetailLineItem> GetAllData()
        {
            return this.GetData(1, 1900, 1, 2100);
        }

        /// <summary>
        /// Api which returns EA usage records for the current month.
        /// </summary>
        /// <returns>Collection of EA billing line items.</returns>
        [Route(@"currentmonth")]
        public List<BillingDetailLineItem> GetCurrentMonthData()
        {
            return this.GetData(0, 0, 0, 0);
        }

        /// <summary>
        /// Api which returns CSP summarized invoice line items for the given month input.
        /// </summary>
        /// <param name="monthYear">Provide month and year in "mm-yyyy" format. Example: "02-2016".</param>
        /// <returns>Collection of EA billing line items.</returns>
        [Route(@"bymonth/{monthYear}")]
        public List<BillingDetailLineItem> GetSingleMonthData([FromUri] string monthYear)
        {
            UrlParameterValidation.ValidateMonthYearFormat(monthYear);
            int month = int.Parse(monthYear.Split('-')[0], CultureInfo.InvariantCulture);
            int year = int.Parse(monthYear.Split('-')[1], CultureInfo.InvariantCulture);
            return this.GetData(month, year, 0, 0);
        }

        /// <summary>
        /// Api which returns CSP summarized invoice line items for the given month range input.
        /// </summary>
        /// <param name="startMonthYear">Provide range start month and year in "mm-yyyy" format. Example: "02-2016".</param>
        /// <param name="endMonthYear">Provide range end month and year in "mm-yyyy" format. Example: "02-2016".</param>
        /// <returns>Collection of EA billing line items.</returns>
        [Route(@"bymonthrange/{startMonthYear}/{endMonthYear}")]
        public List<BillingDetailLineItem> GetDataForMonthRange([FromUri] string startMonthYear, [FromUri] string endMonthYear)
        {
            UrlParameterValidation.ValidateMonthYearFormat(startMonthYear);
            UrlParameterValidation.ValidateMonthYearFormat(endMonthYear);
            UrlParameterValidation.ValidateStartEndDate(startMonthYear, endMonthYear);
            int startMonth = int.Parse(startMonthYear.Split('-')[0], CultureInfo.InvariantCulture);
            int startYear = int.Parse(startMonthYear.Split('-')[1], CultureInfo.InvariantCulture);
            int endMonth = int.Parse(endMonthYear.Split('-')[0], CultureInfo.InvariantCulture);
            int endYear = int.Parse(endMonthYear.Split('-')[1], CultureInfo.InvariantCulture);

            return this.GetData(startMonth, startYear, endMonth, endYear);
        }

        /// <summary>
        /// Private method to talk to the Azure APIs.
        /// </summary>
        /// <param name="url">Url to hit.</param>
        /// <returns>JSON string.</returns>
        private string GetResponse(string url)
        {
            WebRequest request = WebRequest.Create(url);
            APISecurity.AddHeaders(request, APISecurity.APIKey);

            WebResponse response = null;
            try
            {
                response = (WebResponse)request.GetResponse();
            }
            catch
            {
                throw;
                ////response = (WebResponse)ex.Response;
            }

            // you can use etag to decide the version of the report file.
            ////string etag = response.Headers["ETag"];

            // you can use LastModified to decide if the report file is newer than the existing report in your system. this is as backup method. ETag is the primary one.
            ////string lastModified = response.Headers["LastModified"];

            // the response from the service is byte[]. we decode it as string. if you like, you can use binary reader to keep it as byte[]
            StreamReader reader = null;
            string s = string.Empty;
            //// (int)response.StatusCode + "\t" + response.StatusDescription + "\r\n" + reader.ReadToEnd(); //append status code and message for displaying purpose
            try
            {
                reader = new StreamReader(response.GetResponseStream());
                s = reader.ReadToEnd();
            }
            catch (Exception e)
            {
                s = e.ToString();
            }

            reader.Close();
            response.Close();
            return s;
        }

        /// <summary>
        /// Private method to get monthly data. 
        /// </summary>
        /// <param name="jsonResponse">JSON response for each month.</param>
        /// <param name="monthDetailUrl">Month details URL.</param>
        /// <returns>Collection of Billing detail items.</returns>
        private List<BillingDetailLineItem> GetBillingDetailLineItem(string jsonResponse, string monthDetailUrl)
        {
            List<BillingDetailLineItem> items = new List<BillingDetailLineItem>();
            using (var reader = new StringReader(jsonResponse))
            {
                for (int i = 0; i < 3; i++)
                {
                    var lineValue = reader.ReadLine();
                }

                while (true)
                {
                    var line = reader.ReadLine();
                    if (string.IsNullOrEmpty(line))
                    {
                        break;
                    }

                    BillingDetailLineItem billingDetailLineItem = BillingDetailLineItem.Parse(line);
                    billingDetailLineItem.DownloadUrl = monthDetailUrl;
                    items.Add(billingDetailLineItem);
                }
            }

            return items;
        }

        /// <summary>
        /// Method to get billing data of all the months as per given input range.
        /// </summary>
        /// <param name="startMonth">Start month of the range.</param>
        /// <param name="startYear">Start year of the range.</param>
        /// <param name="endMonth">End month of the range.</param>
        /// <param name="endYear">End year of the range.</param>
        /// <returns>Collection of BillingDetailLineItem.</returns>
        private List<BillingDetailLineItem> GetData(int startMonth, int startYear, int endMonth, int endYear)
        {
            string url = string.Format(CultureInfo.InvariantCulture, urlTemplate, APISecurity.EnrolmentNumber);
            string jsonResponse = null;
            UsageReportListApiResponse response = null;
            List<BillingDetailLineItem> items = new List<BillingDetailLineItem>();

            try
            {
                jsonResponse = this.GetResponse(url);
                response = JsonConvert.DeserializeObject<UsageReportListApiResponse>(jsonResponse);
            }
            catch
            {
                throw;
            }

            if (startMonth > -1 && startMonth <= 12 && endMonth > -1 && endMonth <= 12 && startYear > -1 && endYear > -1)
            {
                if (startMonth == 0)
                {
                    startMonth = DateTime.Now.Month;
                }

                if (startYear == 0)
                {
                    startYear = DateTime.Now.Year;
                }

                if (endMonth == 0)
                {
                    if (startMonth == 12)
                    {
                        endMonth = 1;
                        endYear = startYear + 1;
                    }
                    else
                    {
                        endMonth = startMonth + 1;
                        endYear = startYear;
                    }
                }

                DateTime startDate = new DateTime(startYear, startMonth, 1);
                DateTime endDate = new DateTime(endYear, endMonth, 1);

                if (response != null && response.AvailableMonths != null)
                {
                    foreach (var monthData in response.AvailableMonths)
                    {
                        int currentYear = int.Parse(monthData.Month.Split('-')[0], CultureInfo.InvariantCulture);
                        int currentMonth = int.Parse(monthData.Month.Split('-')[1], CultureInfo.InvariantCulture);
                        DateTime currentDate = new DateTime(currentYear, currentMonth, 1);

                        if (currentDate >= startDate && currentDate < endDate)
                        {
                            var monthDetailUrl = string.Format(
                                CultureInfo.InvariantCulture,
                                @"{0}/{1}",
                                baseUrlTemplate,
                                monthData.LinkToDownloadDetailReport);
                            try
                            {
                                var data = this.GetResponse(monthDetailUrl);
                                items.AddRange(this.GetBillingDetailLineItem(data, monthDetailUrl));
                            }
                            catch (Exception ex)
                            {
                                // Can be used in future for logging
                                throw new EaBillingException(ex.Message, ex);
                            }
                        }
                    }
                }

                return items;
            }
            else
            {
                throw new EaBillingException("Invalid Date");
            }
        }
    }
}