using System.Collections.Generic;

namespace BillingDataApi.Models
{
    public class RootObject
    {
        public List<UsageAggregate> value { get; set; }
    }
}