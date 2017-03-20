using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BillingDataApi.Controllers
{
    [RoutePrefix("api/csputilization")]
    public class CspUtilizationController : ApiController
    {
        [Route(@"")]
        public string GetAllData()
        {
            return "Here is the controller default method";
        }
    }
}
