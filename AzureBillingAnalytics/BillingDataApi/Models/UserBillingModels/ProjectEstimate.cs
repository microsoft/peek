namespace BillingDataApi.Models
{
    public class ProjectEstimate
    {
        public string ProjectName { get; set; }
        public double Estimate { get; set; }

        public ProjectEstimate(string projectname, double estimate)
        {
            this.ProjectName = projectname;
            this.Estimate = estimate;
        }
    }
}