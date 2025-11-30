# ☁️ Azure Deployment Guide for QMS

This guide will help you deploy the **QMS Web Application** to Azure and connect your **Printer Clients**.

## 1. Prerequisites

- An active **Azure Subscription**.
- **GitHub Repository** for your code (to use the created workflow).
- **Azure CLI** installed (optional, but helpful).

## 2. Azure Resources Setup

### Option A: Use SQLite (Simplest & Cheapest)
If you use SQLite, you **DO NOT** need to create an Azure SQL Database. The database will be a file stored directly on the Web App's file system.

**Important**: On Azure Web App (Linux), you must ensure the database file is stored in a persistent location so it isn't lost during deployments.
1. We will configure the connection string to point to `/home/site/wwwroot/qms.db` or `/home/data/qms.db`.

### Option B: Create an Azure SQL Database (Recommended for Production)
1. Go to the [Azure Portal](https://portal.azure.com).
2. Search for **SQL Databases** and click **Create**.
3. **Resource Group**: Create a new one (e.g., `rg-qms-prod`).
4. **Server**: Create a new server (e.g., `sql-qms-server`).
   - **Authentication**: Use SQL authentication. Remember your `Admin Login` and `Password`.
5. **Database Name**: `QMS_DB`.
6. **Pricing Tier**: Select **Basic** or **Standard** (DTU-based) for cost-effectiveness.

### C. Create an Azure Web App
1. Search for **App Services** and click **Create**.
2. **Resource Group**: Select the one created above (`rg-qms-prod`).
3. **Name**: Choose a unique name (e.g., `qms-web-app`).
4. **Publish**: Code.
5. **Runtime Stack**: `.NET 8 (LTS)`.
6. **Operating System**: Linux (recommended).
7. **Region**: Choose a region close to you.
8. **Plan**: Basic (B1) or Standard (S1). Free (F1) works for testing but has limits.

## 3. Configuration

### If using SQLite (Option A)
1. Go to your **App Service** > **Settings** > **Environment variables**.
2. Add a new **Connection String**:
   - **Name**: `DefaultConnection`
   - **Value**: `Data Source=/home/site/wwwroot/qms.db`
   - **Type**: Custom

### If using Azure SQL (Option B)
1. Go to your **App Service** > **Settings** > **Environment variables**.
2. Add a new **Connection String**:
   - **Name**: `DefaultConnection`
   - **Value**: `Server=tcp:sql-qms-server.database.windows.net,1433;Initial Catalog=QMS_DB;Persist Security Info=False;User ID={your_admin_username};Password={your_password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;`
   - **Type**: SQLAzure

### B. GitHub Actions Deployment
1. Go to your **App Service** > **Overview** > **Get publish profile**. Download the file.
2. Go to your **GitHub Repository** > **Settings** > **Secrets and variables** > **Actions**.
3. Create a **New repository secret**:
   - **Name**: `AZUREAPPSERVICE_PUBLISHPROFILE`
   - **Value**: Paste the content of the downloaded publish profile file.
4. Open `.github/workflows/azure-webapps-dotnet-core.yml` in your code.
5. Update `app-name: 'your-app-name'` with your actual Azure Web App name (e.g., `qms-web-app`).
6. Push your code to the `main` branch. The deployment will start automatically!

## 4. Database Migration

Since the app uses Entity Framework Core, you need to apply migrations to the production database.
You can do this via your local machine if you allowed your IP in step 2.A.7:

1. Update your local `appsettings.json` connection string to point to the **Azure SQL Database**.
2. Run:
   ```bash
   dotnet ef database update --project src/QMS.Web
   ```
   *Alternatively, you can configure the app to apply migrations on startup (not recommended for high-load prod, but fine for this scale).*

## 5. Printer Client Setup

The Printer Client runs on the physical machines at the branches. It needs to know where the Server is.

### Update the Client Code (Optional but recommended)
You can change the default URL in `src/QMS.PrinterClient/Program.cs`:
```csharp
var serverUrl = "https://qms-web-app.azurewebsites.net/printerhub"; // Your Azure URL
```

### Or Run via Command Line
You don't need to recompile if you don't want to. You can just run the client and it will ask for the server URL if you modify the code to support it, OR just hardcode the production URL before building the client for distribution.

**Recommendation:**
1. Update `serverUrl` in `Program.cs` to your Azure URL.
2. Build the client as a standalone executable:
   ```bash
   dotnet publish src/QMS.PrinterClient -c Release -r win-x64 --self-contained -p:PublishSingleFile=true
   ```
3. Copy the `.exe` file to the branch computers.
4. Run it: `QMS.PrinterClient.exe "Printer Name" "Location" [BranchId]`

## 6. Troubleshooting

- **Log Stream**: Go to App Service > **Log stream** to see real-time logs if the app fails to start.
- **Console Errors**: Check the browser console (F12) for any SignalR connection errors.
