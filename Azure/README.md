# Azure Deployment

This folder contains all the necessary files for deploying the E-Commerce Platform to Microsoft Azure.

## 📁 Files Overview

- **`azure-deploy.json`** - ARM template defining Azure infrastructure
- **`azure-deploy.parameters.json`** - Default parameters for ARM template
- **`deploy-azure.ps1`** - PowerShell deployment script (automated)
- **`deploy.bat`** - Batch deployment script (simplified)
- **`azure-pipelines.yml`** - Azure DevOps CI/CD pipeline configuration

## 🚀 Quick Deployment

### Option 1: Automated PowerShell Script
```powershell
.\Azure\deploy-azure.ps1 -AppName "your-unique-app-name"
```

### Option 2: Simple Batch Script
```cmd
.\Azure\deploy.bat
```

### Option 3: Azure CLI Manual
```bash
az deployment group create \
  --resource-group "ecommerce-rg" \
  --template-file ".\Azure\azure-deploy.json" \
  --parameters @.\Azure\azure-deploy.parameters.json
```

## 💰 Azure Resources & Costs

| Resource | SKU | Monthly Cost |
|----------|-----|--------------|
| App Service Plan | B1 | ~$13 |
| Azure SQL Database | Basic | ~$5 |
| Application Insights | Free tier | $0 |
| **Total** | | **~$18/month** |

## 🔧 Prerequisites

1. **Azure subscription** with active payment method
2. **Azure CLI** installed (`winget install Microsoft.AzureCLI`)
3. **Unique app name** (globally unique across Azure)
4. **.NET 9.0 SDK** for local building

## 📋 Deployment Parameters

Update `azure-deploy.parameters.json` with your values:

```json
{
  "appName": "your-unique-app-name-2025",
  "sqlServerName": "your-sql-server-2025",
  "sqlAdminUsername": "youradmin",
  "sqlAdminPassword": "YourSecurePassword123!"
}
```

## 🎯 Post-Deployment

After successful deployment:

1. **Test the application**: `https://your-app-name.azurewebsites.net`
2. **Check Application Insights**: Monitor performance and errors
3. **Verify database**: Ensure SQL Database is accessible
4. **Test authentication**: Register/login functionality

## 🔍 Troubleshooting

- **App name conflicts**: Try a different unique name
- **SQL password requirements**: Must meet Azure complexity requirements
- **Region availability**: Try different Azure regions if resources unavailable
- **Deployment logs**: Check Azure Portal for detailed error messages

## 📞 Support

For deployment issues:
- Check Azure Portal deployment logs
- Verify Azure subscription limits
- Ensure all required permissions are granted
- Review Azure service health status
