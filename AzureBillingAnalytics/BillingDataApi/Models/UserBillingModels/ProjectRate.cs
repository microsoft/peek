using System.Collections.Generic;

namespace BillingDataApi.Models
{
    public class ProjectRate
    {
        public string ProjectName { get; set; }

        public IEnumerable<RatedUsage> Rates { get; set; }

        public ProjectRate(string projectname, IEnumerable<RatedUsage> rates)
        {
            this.ProjectName = projectname;
            this.Rates = rates;
        }
    }
}