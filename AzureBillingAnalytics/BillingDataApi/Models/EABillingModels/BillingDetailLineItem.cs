using System;
using System.IO;
using BillingDataApi.Models.EABillingModels.Utilities;

namespace BillingDataApi.Models.EABillingModels
{
    public class BillingDetailLineItem
    {
        public string AccountOwnerId { get; set; }
        public string AccountName { get; set; }
        public string ServiceAdministratorId { get; set; }
        public string SubscriptionId { get; set; }
        public string SubscriptionGuid { get; set; }
        public string SubscriptionName { get; set; }
        public DateTime Date { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
        public int Year { get; set; }
        public string Product { get; set; }
        public string ResourceGUID { get; set; }
        public string Service { get; set; }
        public string ServiceType { get; set; }
        public string ServiceRegion { get; set; }
        public string ServiceResource { get; set; }
        public string ResourceQtyConsumed { get; set; }
        public string ServiceSubRegion { get; set; }
        public string ServiceInfo { get; set; }
        public string Component { get; set; }
        public string ServiceInfo1 { get; set; }
        public string ServiceInfo2 { get; set; }
        public string AdditionalInfo { get; set; }
        public string Tags { get; set; }
        public string StoreServiceIdentifier { get; set; }
        public string DepartmentName { get; set; }
        public string CostCenter { get; set; }

        public string DownloadUrl { get; set; }
        public string ResourceRate { get; set; }

        public string ExtendedCost { get; set; }

        public static BillingDetailLineItem Parse(string text)
        {
            var reader = new StringReader(text);
            var fieldParser = new Microsoft.VisualBasic.FileIO.TextFieldParser(reader);

            fieldParser.TextFieldType = Microsoft.VisualBasic.FileIO.FieldType.Delimited;
            fieldParser.SetDelimiters(",");

            var items = fieldParser.ReadFields();

            var b = new BillingDetailLineItem();
            b.AccountOwnerId = items[0].FormatBillingLineItem();
            b.AccountName = items[1].FormatBillingLineItem();
            b.ServiceAdministratorId = items[2].FormatBillingLineItem();
            b.SubscriptionId = items[3].FormatBillingLineItem();
            b.SubscriptionGuid = items[4].FormatBillingLineItem();
            b.SubscriptionName = items[5].FormatBillingLineItem();
            b.Date = items[6].FormatBillingLineItem().ToDateTime();
            b.Month = items[7].FormatBillingLineItem().ToInt();
            b.Day = items[8].FormatBillingLineItem().ToInt();
            b.Year = items[9].FormatBillingLineItem().ToInt();
            b.Product = items[10].FormatBillingLineItem();
            b.ResourceGUID = items[11].FormatBillingLineItem();
            b.Service = items[12].FormatBillingLineItem();
            b.ServiceType = items[13].FormatBillingLineItem();
            b.ServiceRegion = items[14].FormatBillingLineItem();
            b.ServiceResource = items[15].FormatBillingLineItem();
            b.ResourceQtyConsumed = items[16].FormatBillingLineItem();
            b.ResourceRate = items[17].FormatBillingLineItem();
            b.ExtendedCost = items[18].FormatBillingLineItem();
            b.ServiceSubRegion = items[19].FormatBillingLineItem();
            b.ServiceInfo = items[20].FormatBillingLineItem();
            b.Component = items[21].FormatBillingLineItem();
            b.ServiceInfo1 = items[22].FormatBillingLineItem();
            b.ServiceInfo2 = items[23].FormatBillingLineItem();
            b.AdditionalInfo = items[24].FormatBillingLineItem();
            b.Tags = items[25].FormatBillingLineItem();

            if (items.Length > 26)
                b.StoreServiceIdentifier = items[26].FormatBillingLineItem();

            if (items.Length > 27)
                b.DepartmentName = items[27].FormatBillingLineItem();

            if (items.Length > 28)
                b.CostCenter = items[28].FormatBillingLineItem();

            return b;
        }

        public string Key
        {
            get
            {
                var key = string.Format("{0}-{1}-{2}-{3}-{4}-{5}-{6}", Year, Month, Day, ServiceInfo, Component,
                    ServiceInfo1, ServiceInfo2);
                return key;
            }
        }

        public string ResourceKey
        {
            get
            {
                var key = string.Format("{0}-{1}-{2}-{3}-{4}-{5}-{6}", ResourceGUID, Service, ServiceType, Component,
                    ServiceInfo, ServiceInfo1, ServiceInfo2);
                return key;
            }
        }
    }
}