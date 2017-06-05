using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BillingWebJob.AzureAnalyticsDb
{
    [Table("CspUtilizationData")]
    public partial class CspUtilizationData
    {
        public int Id { get; set; }

        public DateTime? UsageStartTime { get; set; }

        public DateTime? UsageEndTime { get; set; }

        [StringLength(1024)]
        public string ResourceId { get; set; }

        [StringLength(1024)]
        public string ResourceName { get; set; }

        [StringLength(1024)]
        public string ResourceCategory { get; set; }

        [StringLength(1024)]
        public string ResourceSubCategory { get; set; }

        public double? Quantity { get; set; }

        [StringLength(256)]
        public string Unit { get; set; }

        [StringLength(1024)]
        public string infoFields { get; set; }

        [StringLength(1024)]
        public string InstanceDataResourceUri { get; set; }

        [StringLength(1024)]
        public string InstanceDataLocation { get; set; }

        [StringLength(1024)]
        public string InstanceDataPartNumber { get; set; }

        [StringLength(1024)]
        public string InstanceDataOrderNumber { get; set; }

        public string InstanceDatatags { get; set; }

        [StringLength(1024)]
        public string Attributes { get; set; }
    }
}
