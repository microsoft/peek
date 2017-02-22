#Deployment Steps

We recommend that you use the [Accounts Template](/Documentation/AccountsTemplate.docx) to collect all relevant accounts information in one place as you go through the whole deployment process.

1. ##Azure Pre-configurations

  1.	Login into Azure portal at http://portal.azure.com

  2.	[Optional] We recommend that you create a new resource group for all deployments related to the solution so that access management is easier. Note the location where RG is created. Update Deployment Resources Table: [RESOURCE GROUP NAME]  
  ![solution_arch](/Documentation/Images/image3.png)

  3.	[Optional] Ensure any user account(s) which will be used for continuous deployment have at least “Contributor” access on this Resource Group. Update Deployment Resources Table: [ACCOUNTS WITH CONRIBUTOR ACCESS ON RG]

  4.	Next, Create Azure Storage account(s) for
    1. Web Job dashboard
    2. Web Job Storage
    3. Custom data backup
    
    The first two accounts are pre-requisites for Azure Web Job and are used for storing web job dashboard information and logs etc. The third account is used for keeping backups of the data that is stored in DB. We can use same or different storage accounts for all three. For the sample deployment covered in this document, we will create only one and use it for all three.
    When you create the Storage account
      1. Ensure Deployment model is “Resource Manager”
      2. Ensure Account kind is “General Purpose”
      3. Ensure correct resource group created in step 2 is selected.
      4. Ensure location is same as resource group created in step 2.  
    ![deployment_guide](/Documentation/Images/image4.png)
 
  5. Once the Storage account(s) is/are successfully created, note the Storage Account Name(s) and Access Key(s) for later use:   
  ![deployment_guide](/Documentation/Images/image5.png)
  *Update Deployment Resources Table: Storage Accounts Sections*

  6.   Next, create a new SQL database in Azure.  
  ![deployment_guide](/Documentation/Images/image6.png)

  7.	Provide appropriate values while creating a new database & database server:
    1.	Ensure the correct RG is selected, the one which was created in earlier steps.
    2.	Ensure “Blank database” is selected under “select source”.
    3.	Ensure the Server created is version v12.
    4. 	Ensure server region is same as other resources created so far.
    5.	Take note of server admin login and password provided in this screen.
    6.	Ensure “Allow Azure services to access server” is checked.  
    ![deployment_guide](/Documentation/Images/image7.png)

  8.	Wait for the database server and database to be successfully created.
  *Update Deployment Resources Table: SQL Server and Database details section*

2. ##Data Extraction Service (Azure Web API) deployment
  
  1.	From the source code, open AzureBillingAnalytics.sln in Visual Studio and look at the “BillingDataApi” project.

  2.	Now open web.config (if using Visual Studio for deployment) or open ~\BillingDataApi\bin\BillingDataApi.dll.config (if using FileZilla dor deployment). Different sections must be updated in accordance with the following sections for proper tokens in place for different types of subscriptions.

  2.	Target accounts access token generation

    1. For EA Account
    Ensure the correct values for following entries are provided in appSettings section  
      1.  EA.EnrolmentNumber
      2.  EA.APIKey
     
       *Description*
       The Billing API is only available for those who have Azure on their Microsoft Enterprise Agreement. When one enrolls Azure into EA they get an Enrollment Number. It’s the enrollment that will be invoiced for the total consumption. In order to use the Billing API we need to find the Enrollment Number by logging into https://ea.azure.com (EA portal).  
       ![deployment_guide](/Documentation/Images/image9.png)
       The second value we need is an Access Key used for Authentication. If we go into the Manage Access area on the start page, we can find it at the bottom. To create a key, press Generate and copy the key to the clipboard.  
       ![deployment_guide](/Documentation/Images/image10.png)
       *Update Deployment Resources Table: EA Account Section*
       Once both Enrollment number and key are captured, provide the correct values in web.config:  
       ![deployment_guide](/Documentation/Images/image11.png)
       
    2. For CSP Account
    Ensure the correct values for following entries are provided in UserAuthentication section
      1. UserName
      2. Password
      3. ApplicationId
  
      *Description*
      In order to be able to access Usage and Billing APIs under Partner Center managed API, we must ensure App+User authentication style. Thus the following steps must be followed to generate Username, Password and Application Id. 
      1.	Login into Partner Centre portal at https://partnercenter.microsoft.com/ as a Global Admin.
      
      2.	Go to Dashboard -> Account Settings -> User Management -> Add user.
      
      3.	Add a new user (or edit an existing user) to ensure they have following privileges:
        1. Assists your customer as: “Admin Agent”
        2. Manages your company as: “Global Admin”  
          ![deployment_guide](/Documentation/Images/image12.png)

      4.	Once user is successfully created, note the username and password. These are Username and Password values that need to be provided in web.config. Ensure password is NOT temporary. If temporary password is generated, ensure to login with the account and change password.  
      ![deployment_guide](/Documentation/Images/image13.png)

      5.	Now, we must create a native client app. For this step, in the partner center portal, go to Dashboard -> Account settings -> App Management.  
      ![deployment_guide](/Documentation/Images/image14.png)

      6.	Click on “Add new native app”. Once the app is successfully created, note down the App ID.
      
      7.	Now provide the username, password and App ID values in config file under the User Authentication section.  
      ![deployment_guide](/Documentation/Images/image15.png)
      *Update Deployment Resources Table: CSP Account: App+User Section*
    [Optional] The current implementation requires App+User credentials only hence the above steps are sufficient. However, if the user wants to change this behavior by enabling AppAuthentication, then they must ensure the correct values for following entries are provided in **AppAuthentication** section:
      * ApplicationId
      * ApplicationSecret
      * Domain

      *Note:* Some APIs under Partner Center managed API can also be accessed by App standalone authentication style. Since our solution requires App+User authentication, this step is optional. However, if the user intends to modify this solution and start using App Authentication, the following steps must be followed to generate ApplicationId, Application secret and Domain. 
      1.	Login into Partner Center portal at https://partnercenter.microsoft.com/ as a Global Admin.
      
      2.	Go to Dashboard -> Account Settings -> App Management.
      
      3.	Click on “Add new web app” to register a new Azure AD App.  
      ![deployment_guide](/Documentation/Images/image16.png)
 
      4.	After successful registration, note the App ID and Domain Name. 
      
      5.	Register a new key under Key management and note the key value.  
      ![deployment_guide](/Documentation/Images/image17.png)
 
      6.	Provide these values in config file under AppAuthentication section.  
      ![deployment_guide](/Documentation/Images/image18.png)
      *Update Deployment Resources Table: CSP Account: App Section*

    3.	For Direct Azure Subscriptions
 To be supported in future iterations.

  3.	Publish to Azure
    1. After all configurations have been updated successfully, rebuild the solution. Ensure it is successfully built.
      
    2. Right click on the solution and click “Publish”.
      
    3. Under “Select a publish target”, select “Microsoft Azure App Service”.  
      ![deployment_guide](/Documentation/Images/image19.png)
      *Note:* Depending on the version of Visual Studio/Azure SDK, the options may look slightly different, e.g. you may see “Microsoft Azure Web Apps” instead of “Microsoft Azure App Service”.  
      ![deployment_guide](/Documentation/Images/image20.png)
    4. Ensure you are logged in with the correct Account (which has admin access in Azure RG published previously). If this is the first time publishing the App Service, click on “New”.  
      ![deployment_guide](/Documentation/Images/image21.png)
    5.	Provide appropriate name to the App Service Name, select appropriate subscription and previously created Resource Group.  
      ![deployment_guide](/Documentation/Images/image22.png)      
    6. Under App Service Plan, click new and create a new plan and ensure the size is at least Basic. This is to ensure “Always On” capability is available to our App Service.  
      ![deployment_guide](/Documentation/Images/image23.png)
      *Note:* Depending on the version of Visual Studio/Azure SDK, the options may look slightly different, e.g. you may see slightly different User interface with additional options e.g. ability to add DB server. Set it as “No Database” and continue.  
      ![deployment_guide](/Documentation/Images/image24.png)
    7. Click on Create. This will start deployment of the App service in Azure. Once complete, you will start seeing Connection details in “Connection” tab.
    8. Click on “Validate Connection” to ensure connection is successful. Now click on “Publish”.  
      ![deployment_guide](/Documentation/Images/image25.png)
    9. Wait for Publishing process to complete successfully. You can view the status in Output Window.  
      ![deployment_guide](/Documentation/Images/image26.png)
      
  4. Deployment Verification
    1. Once Publishing process is successfully complete, the URL for the Service App will be opened. Note the URL and also ensure the Success message is visible on the Browser.  
    ![deployment_guide](/Documentation/Images/image32.png)
    
    2.	Our Web API is now successfully published. It is not secured yet. For testing, add “/swagger” to the URL to verify all API controllers are available.  
    ![deployment_guide](/Documentation/Images/image33.png)
    
    3. To test any URL, simply browse to the URL by adding the appropriate suffix to the path. E.g. to view CspUsage data, browse to http://[webappname]/api/cspusage
    The result should be available in JSON format. (depending on the browser used, the response could be as a downloadable file or on-screen).  
    ![deployment_guide](/Documentation/Images/image34.png)
    Now, we have ensured that the Web API is successfully deployed. Now, we must enable Authentication to ensure only verified accounts can access the data.
  5. Enable Authentication/Authorization
    1. Now you will see new Service App and plan under the appropriate RG. Select the App Service to view Settings.  
    ![deployment_guide](/Documentation/Images/image35.png)
    
    2. In the Settings blade, scroll down to “Authentication/Authorization” section.  
    ![deployment_guide](/Documentation/Images/image36.png)
    
    3. Set “App Service Authentication” to “On” and under “Authentication providers” select “Azure Active Directory”. Under “Management Mode” choose “Express”.
    
    4. Choose option “Create New AD App” and click “OK”. Alternately an existing app can also be used if created previously.  
    ![deployment_guide](/Documentation/Images/image37.png)
    
    5. In the Authentication/Authorization blade, click “Save”.
    
    6. Wait for deployment to successfully complete.
    
    7. Close and re-open the “Authentication/Authorization” blade again to view the details of the created app. Note the client Id created. This will be used in later sections.  
    ![deployment_guide](/Documentation/Images/image38.png)
    
    8. Now, our API is secure. To test this, try to browse the URL again in a new browser window. (Ensure in-private session in case you are already logged in with a valid Azure AD account). Now you will be re-directed to sign-in page. Now only a valid Azure AD user (relevant to the AD of Azure account under which the deployment has been completed) can access the data when they provide the correct credentials.
    
    9. We are interested to access the data by our web job, hence we would need to generate client secret for the above client id which can then access the data programmatically. To perform this step, open “Azure Active Directory” service in the Azure portal:  
    ![deployment_guide](/Documentation/Images/image39.png)
    
    10. Here you should see the Application that was created in step 7 available in the list under “App registrations”. Select it to view details. Go to Settings -> Keys  
    ![deployment_guide](/Documentation/Images/image40.png)
    
    11. In the “keys” blade, create a new key and click save. Once successfully saved, key value will be shown. Ensure to copy the value of the key that was generated:  
    ![deployment_guide](/Documentation/Images/image41.png)
    
    12. Go to “Domains names” tab and copy the default domain name.  
    ![deployment_guide](/Documentation/Images/image42.png)
    Update Deployment Resources Table: Data Extraction (web api) section
    
    13. Now our Web API is successfully deployed which can be used to access EA, CSP or Direct subscriptions data.

  *IMPORTANT:* The Auth/Auth mechanism mentioned above is for sample purposes only which performs Active Directory authentication to grant access to the URL to both a valid user or a valid application account. For production deployment, user must review this security model thoroughly and revise if necessary.

3. ##Persistent Data Storage (Azure database) deployment
  1. Code/binaries Access 
    1.	From the source code, open "AzureBillingAnalytics.sln" in Visual Studio and look at “BillingDataDb” project. Once the solution is successfully opened, rebuild it and ensure it rebuilds successfully.  

  2. Publish to Azure
    1. For deployment using Visual Studio
      1.	Right click on the project and click “Publish”. The Publish database popup will appear.
        
      2.  Under Target Database Connection, click on “Edit”.
        
      3.	In the “Connect popup”, go to browse tab and click on “Azure”. Ensure the appropriate account and subscription are selected.  
        ![deployment_guide](/Documentation/Images/image44.png)

      4. Select the database which was created in pre-configuration steps. Provide the correct password.
        1. Ensure all details like Server name and database name are correct. 
        2. Ensure “Sql Server Authentication” is selected.
        
      5. Click on “Test Connection”. You may be prompted to add your subnet into the firewall rule. Ensure you select either your IP or the subnet range.  
      ![deployment_guide](/Documentation/Images/image45.png)

      6. Ensure “Test Connection succeeded” message is seen. Click ok.
        
      7. Now, in the publish database screen, click on “Publish”.  
        ![deployment_guide](/Documentation/Images/image46.png)

      8. Ensure the publishing process was successfully completed. This can be monitored from the “Data Tools Operation window”.  
        ![deployment_guide](/Documentation/Images/image47.png)
        
  3. Deployment Verification
    1. Once the deployment is successful, we can use any SQL UI client (like SSMS, Linqpad etc) to connect with the server and verify the database has been configured correctly. E.g. connect from SSMS by providing the correct Server name, username and password. Ensure SQL Server Authentication is selected as the authentication mode.  
    ![deployment_guide](/Documentation/Images/image48.png)
   
    2. On successful connection, the database and tables should be visible  
    ![deployment_guide](/Documentation/Images/image51.png)

4. ##Data Aggregation Engine (Azure Web job) deployment
  
  1. From the source code, open AzureBillingAnalytics.sln in Visual Studio and look at "BillingWebJob” project. Once the solution is successfully opened, rebuild it and ensure it rebuilds successfully.  
    
  2.  Update Configurations
    1. Open Configuration file.
      1. Open app.config.
    
      Each section must be updated with relevant information from previous sections. Update web job storage account connection strings. Provide the appropriate values which were created in section 1, step 3. Ensure both accountname and accountkey are updated in proper format:
      *The format of the connection string is "DefaultEndpointsProtocol=https;AccountName=[NAME];AccountKey=[KEY]"*  
      ![deployment_guide](/Documentation/Images/image53.png)
 
      2. Next, update database connection string with appropriate values based on the deployment completed in Section 1. 
      *The format of the connection string is "data source=[SERVERNAME].database.windows.net;initial catalog=[DATABASENAME];persist security info=True;user id=[LOGINUSERNAME];password=[LOGINPASSWORD];MultipleActiveResultSets=True;App=EntityFramework"*  
      ![deployment_guide](/Documentation/Images/image54.png)
      
      3. Next, update URL for Web API section with appropriate values based on the deployment completed in Section 2.  
      ![deployment_guide](/Documentation/Images/image55.png)
 
      4.	Next, update Web API application credentials with appropriate values based on the deployment completed in Section 2 – Auth/Auth section.  
      ![deployment_guide](/Documentation/Images/image56.png)

      5.	Next, provide the frequency at which the web job should be run. Default value is “Daily”.  
      ![deployment_guide](/Documentation/Images/image57.png)
 
      6.	Next, provide the Storage connection string created earlier for Data backup. 
      *The format of the connection string is "DefaultEndpointsProtocol=https;AccountName=[NAME];AccountKey=[KEY]"*  
      ![deployment_guide](/Documentation/Images/image58.png)

      7.	Next, provide the Customer Type for which the job should be run. You can provide a comma separated value to collect data for multiple types.  
      ![deployment_guide](/Documentation/Images/image59.png)

      8.	Next provide the start and end month for each customer type for which the job should be run. Leave End date as empty to ensure the last date is always current month.  
      ![deployment_guide](/Documentation/Images/image60.png)
  3. Publish to Azure
    1.	Rebuild the solution and ensure it rebuilds successfully.
      
    2.	Right click on the project and select “Publish as Azure Web Job”.
      
    3.	In the Publish web dialog box, go to “Profile” section & select Microsoft Azure App service.
      
    4.	In the popup, select appropriate account and subscription.
      
    5.	Ensure that you select the same App Service that was published in section 2 for API.  
      ![deployment_guide](/Documentation/Images/image61.png)
      
    6.	In the Connection tab, click on “Validate Connection” and ensure success message appears.
      
    7.	Click on “Publish”. Monitor progress in Output window.  
      ![deployment_guide](/Documentation/Images/image62.png)
 
   

  4.	Deployment Verification
    1.	To verify that the job has been successfully configured, login into Azure portal.
    
    2.	Go to the App Service which was created in Section 2.
    
    3.	In the Settings blade, scroll down to “WebJobs” section.
    
    4.	Note that the newly created webjob should appear in the list and should be in “running” state.  
    ![deployment_guide](/Documentation/Images/image65.png)
 
    5.	Select the job and click on “Logs”. This will open Webjobs dashboard.  
    ![deployment_guide](/Documentation/Images/image66.png)
 
    6.	Click on the job “BillingWebJob” to view details. In the details page, click on “Toggle Output” button to make logs appear/disappear. The logs will provide user friendly messages on the operations being performed by the web job. Go through the logs to ensure no error messages are seen. If any error message appears, fix the issue and redeploy the job.  
    ![deployment_guide](/Documentation/Images/image67.png)
    *Note:* Restarting or Redeploying the job will automatically re-trigger the job routine immediately irrespective of the scheduling preferences. 

    7.	Monitor the job to ensure data is being updated in the DB. If the job is run for the first time and billing periods provided in the config files is huge, this may take a while.  
    ![deployment_guide](/Documentation/Images/image68.png)
 
    8.	Once all data transfer is complete, we can verify the data by connecting to the DB. “AuditData” table will capture the details about every job run per subscriptionType, including error logs, if any. Check for records in other tables for relevant billing/usage data for each type.  
    ![deployment_guide](/Documentation/Images/image69.png)
  5.	Configure Always-On
  Since the Web job must run continuously, we have a need to configure Always-On on the App Service, otherwise, the App Service may enter into idle mode if no traffic is seen for a considerable period of time. 
  Since the Web job must run continuously, we have a need to configure Always-On on the App Service, otherwise, the App Service may enter into idle mode if no traffic is seen for a considerable period of time. 
    
    1.	To enable always-on, login into Azure portal.
    
    2.	Select the App Service we created and go to “Application Settings”.  
    ![deployment_guide](/Documentation/Images/image70.png)
    
    3.	Find the “Always on” setting, and flip it to “ON”. Click Save.

4. ##Visualizations and Analytics (Power BI) deployment
  
  1. Source files access
    1.	From the source code link, download “PowerBISamples” folder.
    
    2.	Unzip the folder to access sample Power BI source files.  
    ![deployment_guide](/Documentation/Images/image71.png)
  2. Update Configurations
    1.	Open any one of the files in Power BI Desktop.
    
    2.	Go to File -> Options and Settings -> Data source settings.  
    ![deployment_guide](/Documentation/Images/image72.png)
    
    3. Click on “Change Source…” button.
    
    4. In the popup that appears, provide SQL Server name and Database name.  
    ![deployment_guide](/Documentation/Images/image73.png)
    
    5.	Click ok and close.
    
    6.	Now, click on the “Apply changes” button appearing on the top of the screen.  
    ![deployment_guide](/Documentation/Images/image74.png)
    
    7.	In the popup that appears, select “database” tab and provide username and password to access db server.  
    ![deployment_guide](/Documentation/Images/image75.png)
    
    8.	Wait for new data model to load. Depending on the number of items, this may take a while.
    
    9.	Now the report is connected with the new data model. Browse to each tab to verify the data.
  3.  Publish to Power BI Service

    1.	Edit or modify the report as desired. When ready, publish the report to Power BI service, by clicking on the “Publish” button on the ribbon.  
    ![deployment_guide](/Documentation/Images/image76.png)

    2.	If prompted, sign into the Power BI service with a valid account.  
    ![deployment_guide](/Documentation/Images/image77.png)

    3.	On successful sign in, you maybe prompted to select the workspace under which the report should be published (incase multiple workspaces exist). Select the appropriate one and click “Publish”. It is recommended that you create a new workspace specific for these reports so that access can be managed at the workspace level.  
    ![deployment_guide](/Documentation/Images/image78.png)

    4.	Once the report is successfully published, the URL will be flashed on screen.  
    ![deployment_guide](/Documentation/Images/image79.png)

    5.	Click on the URL to go to the Power BI Web Portal. The report is now published successfully.

    6.	Repeat the same process for other reports.

  4.	Enable Automatic Data Refresh

    1.	We must enable automatic refresh on data so that the published report always shows latest data. In the Power BI web Portal. In the Power BI web portal, scroll through the panel on the left and go to the “DataSets” section.
    
    2.	Here, look for the relevant data set which must be refreshed. For the above sample, the name of the dataset is same as the report name “EACustomerBillingAndUsage.”
    
    3.	Click on the elipses and click on “Schedule Refresh”.  
    ![deployment_guide](/Documentation/Images/image80.png)
    
    4.	Choose the appropriate settings, e.g. refresh either daily or weekly, depending on the need and click “apply”. You can optionally choose to get an email if refresh job fails.  
    ![deployment_guide](/Documentation/Images/image81.png)
    
    5.	Once the configurations are successfully saved, a success message will be flashed.  
    ![deployment_guide](/Documentation/Images/image82.png)
 
    6.	Repeat the same process for other reports.
  
  5.	Create dashboards

    1.	Once the reports are successfully published, any number of charts out of these can be used to create custom dashboards.
    
    2.	In Power BI web Portal, create a new Dashboard in the current workspace by clicking on the + sign near “Dashboards” on the left panel.  
    ![deployment_guide](/Documentation/Images/image83.png)
    
    3.	Give an appropriate name to the dashboard:  
    ![deployment_guide](/Documentation/Images/image84.png)

    4.	Now, go back to the report just published and select any chart which needs to be highlighted in Dashboard. On the top right, look for the “Pin Visual” button.  
    ![deployment_guide](/Documentation/Images/image85.png)

    5.	In the popup, select the new dashboard and click “Pin”.  
    ![deployment_guide](/Documentation/Images/image86.png)
    
    6.	Go back to the dashboard and notice that a new tile has been created. This tile can be resized and more such tiles can be added as appropriate. Click on any tile will take the user to the detailed report.

    7.	Using the above steps, one can create interesting Dashboards.

  6.	##Share dashboards

    1.	When the dashboards are ready to be shared with other colleagues, click on the ellipses near the dashboard name and click “Share”.  
    ![deployment_guide](/Documentation/Images/image87.png)

    2.	Provide the email address for the colleagues with whom the dashboard must be shared. We can also include an optional email message.  
    ![deployment_guide](/Documentation/Images/image88.png)

    3.	Wait for the success message on the portal.  
    ![deployment_guide](/Documentation/Images/image89.png)
 
    4.	The dashboard (and the reports associated with it) are now accessible to all the users with whom it has been shared.
