using System;

namespace BillingDataApi.Models.EABillingModels.Utilities
{
    public static class StringExtensions
    {
        public static string FormatBillingLineItem(this string item)
        {
            var temp = item.Replace("\"", string.Empty);
            temp = temp.Replace(@"\", string.Empty);
            return temp;
        }

        public static int ToInt(this string item)
        {
            return int.Parse(item);
        }

        public static DateTime ToDateTime(this string item)
        {
            return DateTime.Parse(item);
        }
    }
}