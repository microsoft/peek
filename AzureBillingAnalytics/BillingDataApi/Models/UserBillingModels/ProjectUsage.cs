using System.Collections.Generic;

namespace BillingDataApi.Models
{
    public class ProjectUsage
    {
        public string ProjectName { get; set; }

        public IEnumerable<MeterIDTotal> MeterIdTotals { get; set; }

        public ProjectUsage(string projectname, IEnumerable<MeterIDTotal> meteridtotals)
        {
            this.ProjectName = projectname;
            this.MeterIdTotals = meteridtotals;
        }
    }
}