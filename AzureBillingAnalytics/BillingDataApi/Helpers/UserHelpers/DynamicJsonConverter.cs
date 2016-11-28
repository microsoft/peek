// -----------------------------------------------------------------------
// <copyright file="DynamicJsonConverter.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>Helper class for Json object manipulation.</summary>
// -----------------------------------------------------------------------

namespace BillingDataApi.UserHelpers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Web.Helpers;
    using System.Web.Script.Serialization;

    /// <summary>
    /// Helper class for Json object manipulation.
    /// </summary>
    public class DynamicJsonConverter : JavaScriptConverter
    {
        /// <summary>
        /// Supported types for Json Converter.
        /// </summary>
        /// <value>Supported types.</value>
        public override IEnumerable<Type> SupportedTypes
        {
            get { return new ReadOnlyCollection<Type>(new List<Type>(new Type[] { typeof(object) })); }
        }

        /// <summary>
        /// Method to deserialize the given object.
        /// </summary>
        /// <param name="dictionary">Key value pair.</param>
        /// <param name="type">Type of object.</param>
        /// <param name="serializer">Java script serializer object.</param>
        /// <returns>Deserialized object.</returns>
        public override object Deserialize(
            IDictionary<string, object> dictionary,
            Type type,
            JavaScriptSerializer serializer)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException("dictionary");
            }

            if (type == typeof(object))
            {
                return new DynamicJsonObject(dictionary);
            }

            return null;
        }

        /// <summary>
        /// Method to serialize the given object.
        /// </summary>
        /// <param name="obj">Object to be serialized.</param>
        /// <param name="serializer">JavaScript serializer object.</param>
        /// <returns>Serialized as key value pairs.</returns>
        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}