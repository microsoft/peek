// -----------------------------------------------------------------------
// <copyright file="CsvRows.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// ----------------------------------------------------------------------- 

using System.Collections.Generic;

namespace BillingWebJob.Helpers
{
    /// <summary>
    /// Type of row to be written in CSV
    /// </summary>
    public class CsvRow : List<string>
    {
        public string LineText { get; set; }
    }
}