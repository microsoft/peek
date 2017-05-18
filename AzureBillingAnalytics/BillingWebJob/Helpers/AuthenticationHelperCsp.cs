//using Microsoft.IdentityModel.Clients.ActiveDirectory;
//using Microsoft.Store.PartnerCenter;
//using Microsoft.Store.PartnerCenter.Extensions;
//using System;
//using System.Collections.Generic;
//using System.Configuration;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace BillingWebJob.Helpers
//{
//    class AuthenticationHelperCsp
//    {
//        /// <summary>
//        /// A lazy reference to an user based partner operations.
//        /// </summary>
//        private IAggregatePartner userPartnerOperations = null;

//        /// <summary>
//        /// A lazy reference to an application based partner operations.
//        /// </summary>
//        private IAggregatePartner appPartnerOperations = null;

//        /// <summary>
//        /// Gets a partner operations instance which is application based authenticated.
//        /// </summary>
//        /// <value>Application Partner Operations object.</value>
//        public IAggregatePartner AppPartnerOperations
//        {
//            get
//            {
//                if (this.appPartnerOperations == null)
//                {
//                    IPartnerCredentials appCredentials = PartnerCredentials.Instance.GenerateByApplicationCredentials(
//                        this.Configuration.ApplicationAuthentication.ApplicationId,
//                        this.Configuration.ApplicationAuthentication.ApplicationSecret,
//                        this.Configuration.ApplicationAuthentication.Domain,
//                        this.Configuration.PartnerService.AuthenticationAuthorityEndpoint.OriginalString,
//                        this.Configuration.PartnerService.GraphEndpoint.OriginalString);

//                    this.appPartnerOperations = PartnerService.Instance.CreatePartnerOperations(appCredentials);
//                }

//                return this.appPartnerOperations;
//            }
//        }

//        /// <summary>
//        /// Gets a configuration instance.
//        /// </summary>
//        /// <value>Configuration Manager object.</value>
//        public ConfigurationManager Configuration
//        {
//            get { return ConfigurationManager.Instance; }
//        }

//        /// <summary>
//        /// Gets a partner operations instance which is user based authenticated.
//        /// </summary>
//        /// <value>User Partner Operations object.</value>
//        public IAggregatePartner UserPartnerOperations
//        {
//            get
//            {
//                if (this.userPartnerOperations == null)
//                {
//                    var aadAuthenticationResult = this.LoginUserToAad();

//                    // Authenticate by user context with the partner service
//                    IPartnerCredentials userCredentials = PartnerCredentials.Instance.GenerateByUserCredentials(
//                        this.Configuration.UserAuthentication.ApplicationId,
//                        new AuthenticationToken(
//                            aadAuthenticationResult.AccessToken,
//                            aadAuthenticationResult.ExpiresOn),
//                        delegate
//                        {
//                            // token has expired, re-Login to Azure Active Directory
//                            var aadToken = this.LoginUserToAad();

//                            // give the partner SDK the new add token information
//                            return Task.FromResult(new AuthenticationToken(aadToken.AccessToken, aadToken.ExpiresOn));
//                        });
//                    this.userPartnerOperations = PartnerService.Instance.CreatePartnerOperations(userCredentials);
//                }

//                return this.userPartnerOperations;
//            }
//        }

//        /// <summary>
//        /// Logs in to AAD as a user and obtains the user authentication token.
//        /// </summary>
//        /// <returns>The user authentication result.</returns>
//        private AuthenticationResult LoginUserToAad()
//        {
//            var addAuthority = new UriBuilder(this.Configuration.PartnerService.AuthenticationAuthorityEndpoint)
//            {
//                Path = this.Configuration.PartnerService.CommonDomain
//            };

//            UserCredential userCredentials = new UserCredential(
//                this.Configuration.UserAuthentication.UserName,
//                this.Configuration.UserAuthentication.Password);

//            AuthenticationContext authContext = new AuthenticationContext(addAuthority.Uri.AbsoluteUri);

//            return authContext.AcquireToken(
//                this.Configuration.UserAuthentication.ResourceUrl.OriginalString,
//                this.Configuration.UserAuthentication.ApplicationId,
//                userCredentials);
//        }
//    }
//}
