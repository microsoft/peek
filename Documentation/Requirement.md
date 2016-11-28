# Requirements
###1.	Active Azure Subscription 

   All components shall be deployed using Azure PaaS. The following PaaS services will be spawned as part of the deployment:
  * Azure App Service
  * Azure Web Job
  * Azure Web API
  * Azure SQL Server and Database
  * Azure storage account(s)

###2.	Power BI license

   An active O365 Subscription with a valid professional Power BI license.

###3.	Power BI Desktop

   This can be downloaded from www.powerbi.com. The version used in this guide is Version: 2.35.4399.601 32-bit (May 2016). Ensure same or later version is downloaded.

###4. [OPTION 1]: Visual Studio 2015 with Azure SDK 2.9 (for deployment using Visual Studio)
###4. [OPTION 2]: FileZilla FTP Client (for offline Api deployment) and SQL Server Management Studio 2012 or later (for Database deployment)

The deployment documents provide steps to deploy the components to Azure via:
  * Visual Studio to support continuous deployment/Dev-Ops enablement. 
  
  OR
  
  * FileZilla – Ftp Client is required for offline deployment.
