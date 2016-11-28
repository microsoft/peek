// -----------------------------------------------------------------------
// <copyright file="WebApiConfig.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>WebApiConfig class.</summary>
// -----------------------------------------------------------------------

namespace BillingDataApi
{
    using System.Web.Http;

    /// <summary>
    /// Default Web Api Config.
    /// </summary>
    public static class WebApiConfig
    {
        /// <summary>
        /// Static method to register routes.
        /// </summary>
        /// <param name="config">Http Configuration.</param>
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional });
        }
    }
}