// -----------------------------------------------------------------------
// <copyright file="ConfigurationManager.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>This class encapsulates the sample application configuration read from app.config.</summary>
// -----------------------------------------------------------------------

namespace BillingDataApi.CspHelpers
{
    using System;

    /// <summary>
    /// Encapsulates the sample application configuration read from app.config.
    /// </summary>
    public class ConfigurationManager
    {
        /// <summary>
        /// A lazy reference to a <see cref="ConfigurationManager"/> instance.
        /// </summary>
        private static Lazy<ConfigurationManager> instance =
            new Lazy<ConfigurationManager>(() => new ConfigurationManager());

        /// <summary>
        /// A reference to the partner service settings.
        /// </summary>
        private Lazy<PartnerServiceSettingsSection> partnerServiceSettings =
            new Lazy<PartnerServiceSettingsSection>(() => new PartnerServiceSettingsSection());

        /// <summary>
        /// A reference to the user authentication configuration.
        /// </summary>
        private Lazy<UserAuthenticationSection> userAuthentication =
            new Lazy<UserAuthenticationSection>(() => new UserAuthenticationSection("UserAuthentication"));

        /// <summary>
        /// A reference to the application authentication configuration.
        /// </summary>
        private Lazy<ApplicationAuthenticationSection> appAuthentication =
            new Lazy<ApplicationAuthenticationSection>(() => new ApplicationAuthenticationSection("AppAuthentication"));

        /// <summary>
        /// A reference to the scenario settings.
        /// </summary>
        private Lazy<ScenarioSettingsSection> scenarioSettings =
            new Lazy<ScenarioSettingsSection>(() => new ScenarioSettingsSection());

        /// <summary>
        /// Prevents a default instance of the <see cref="ConfigurationManager"/> class from being created.
        /// </summary>
        private ConfigurationManager()
        {
        }

        /// <summary>
        /// Gets the singleton instance of the <see cref="ConfigurationManager"/> class.
        /// </summary>
        /// <value>Configuration Manager instance Object.</value>
        public static ConfigurationManager Instance
        {
            get { return ConfigurationManager.instance.Value; }
        }

        /// <summary>
        /// Gets the partner service settings section.
        /// </summary>
        /// <value>Partner Service Section.</value>
        public PartnerServiceSettingsSection PartnerService
        {
            get { return this.partnerServiceSettings.Value; }
        }

        /// <summary>
        /// Gets the user authentication section.
        /// </summary>
        /// <value>User Authentication section.</value>
        public UserAuthenticationSection UserAuthentication
        {
            get { return this.userAuthentication.Value; }
        }

        /// <summary>
        /// Gets the application authentication section.
        /// </summary>
        /// <value>Application authentication section.</value>
        public ApplicationAuthenticationSection ApplicationAuthentication
        {
            get { return this.appAuthentication.Value; }
        }

        /// <summary>
        /// Gets the scenario settings section.
        /// </summary>
        /// <value>Scenario settings section.</value>
        public ScenarioSettingsSection Scenario
        {
            get { return this.scenarioSettings.Value; }
        }
    }
}