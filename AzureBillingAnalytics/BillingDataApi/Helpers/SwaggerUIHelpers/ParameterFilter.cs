// -----------------------------------------------------------------------
// <copyright file="ParameterFilter.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>This class is used to apply changes on API operations.</summary>
// -----------------------------------------------------------------------

namespace BillingDataApi.Helpers.SwaggerUIHelpers
{
    using System.Web.Http.Description;
    using Swashbuckle.Swagger;

    /// <summary>
    /// This class has been intentionally left empty. Can be used to apply changes on operations.
    /// </summary>
    public class ParameterFilter : IOperationFilter
    {
        /// <summary>
        /// Method to apply changes on API operations, if required.
        /// </summary>
        /// <param name="operation">Operation which requires filters.</param>
        /// <param name="schemaRegistry">Schema registry.</param>
        /// <param name="apiDescription">API Description.</param>
        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            // enable line of code in SwaggerConfig
            // intentionally left empty. This class can be used to apply changes on operations.
            // Example: 
            ////if (operation.parameters == null)
            ////{
            ////    return;
            ////}

            ////foreach (var parameter in operation.parameters.Where(x => x.@in == "query" && x.name.Contains("MMYYYY")))
            ////{
            ////    parameter.name = "mm-YYYY";
            ////}
        }
    }
}