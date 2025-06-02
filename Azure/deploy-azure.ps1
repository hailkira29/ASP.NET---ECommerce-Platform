# Azure Deployment Script for E-Commerce Platform
# Run this script to deploy the application to Azure

param(
    [Parameter(Mandatory=$true)]
    [string]$AppName,
    
    [Parameter(Mandatory=$false)]
    [string]$ResourceGroupName = "ecommerce-rg",
    
    [Parameter(Mandatory=$false)]
    [string]$Location = "East US",
    
    [Parameter(Mandatory=$false)]
    [string]$SqlServerName = "",
    
    [Parameter(Mandatory=$false)]
    [string]$SqlAdminUsername = "ecommerceadmin",
    
    [Parameter(Mandatory=$false)]
    [SecureString]$SqlAdminPassword
)

Write-Host "🚀 Starting Azure Deployment for E-Commerce Platform" -ForegroundColor Green
Write-Host "App Name: $AppName" -ForegroundColor Yellow
Write-Host "Resource Group: $ResourceGroupName" -ForegroundColor Yellow
Write-Host "Location: $Location" -ForegroundColor Yellow

# Generate SQL Server name if not provided
if ([string]::IsNullOrEmpty($SqlServerName)) {
    $SqlServerName = "$AppName-sql-$(Get-Random -Minimum 1000 -Maximum 9999)"
}

# Generate secure password if not provided
if ($null -eq $SqlAdminPassword) {
    $SqlAdminPassword = ConvertTo-SecureString -String "ECommerce2025!" -AsPlainText -Force
}

try {
    # Step 1: Check Azure CLI installation
    Write-Host "📋 Checking Azure CLI installation..." -ForegroundColor Cyan
    $azVersion = az --version 2>$null
    if ($LASTEXITCODE -ne 0) {
        Write-Host "❌ Azure CLI not found. Installing..." -ForegroundColor Red
        Write-Host "Please install Azure CLI from: https://docs.microsoft.com/en-us/cli/azure/install-azure-cli" -ForegroundColor Yellow
        exit 1
    }
    Write-Host "✅ Azure CLI found" -ForegroundColor Green

    # Step 2: Login to Azure
    Write-Host "🔐 Checking Azure login status..." -ForegroundColor Cyan
    $accountInfo = az account show 2>$null
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Please login to Azure..." -ForegroundColor Yellow
        az login
        if ($LASTEXITCODE -ne 0) {
            throw "Failed to login to Azure"
        }
    }
    Write-Host "✅ Azure login confirmed" -ForegroundColor Green

    # Step 3: Create Resource Group
    Write-Host "🏗️ Creating resource group: $ResourceGroupName..." -ForegroundColor Cyan
    az group create --name $ResourceGroupName --location $Location
    if ($LASTEXITCODE -ne 0) {
        throw "Failed to create resource group"
    }
    Write-Host "✅ Resource group created" -ForegroundColor Green

    # Step 4: Deploy ARM Template
    Write-Host "🚀 Deploying Azure resources..." -ForegroundColor Cyan
    $plainPassword = [Runtime.InteropServices.Marshal]::PtrToStringAuto([Runtime.InteropServices.Marshal]::SecureStringToBSTR($SqlAdminPassword))
    
    az deployment group create `
        --resource-group $ResourceGroupName `
        --template-file "azure-deploy.json" `
        --parameters appName=$AppName `
                    sqlServerName=$SqlServerName `
                    sqlAdminUsername=$SqlAdminUsername `
                    sqlAdminPassword=$plainPassword

    if ($LASTEXITCODE -ne 0) {
        throw "Failed to deploy Azure resources"
    }
    Write-Host "✅ Azure resources deployed successfully" -ForegroundColor Green

    # Step 5: Build and Publish Application
    Write-Host "🔨 Building application..." -ForegroundColor Cyan
    dotnet build --configuration Release
    if ($LASTEXITCODE -ne 0) {
        throw "Failed to build application"
    }

    Write-Host "📦 Publishing application..." -ForegroundColor Cyan
    dotnet publish --configuration Release --output "./publish"
    if ($LASTEXITCODE -ne 0) {
        throw "Failed to publish application"
    }
    Write-Host "✅ Application built and published" -ForegroundColor Green

    # Step 6: Create deployment package
    Write-Host "📁 Creating deployment package..." -ForegroundColor Cyan
    if (Test-Path "./publish.zip") {
        Remove-Item "./publish.zip" -Force
    }
    Compress-Archive -Path "./publish/*" -DestinationPath "./publish.zip"
    Write-Host "✅ Deployment package created" -ForegroundColor Green

    # Step 7: Deploy to Azure App Service
    Write-Host "🌐 Deploying to Azure App Service..." -ForegroundColor Cyan
    az webapp deployment source config-zip `
        --resource-group $ResourceGroupName `
        --name $AppName `
        --src "./publish.zip"

    if ($LASTEXITCODE -ne 0) {
        throw "Failed to deploy to App Service"
    }
    Write-Host "✅ Application deployed to Azure!" -ForegroundColor Green

    # Step 8: Configure Connection String
    Write-Host "🔗 Configuring database connection..." -ForegroundColor Cyan
    $connectionString = "Server=tcp:$SqlServerName.database.windows.net,1433;Initial Catalog=ECommercePlatformDB;Persist Security Info=False;User ID=$SqlAdminUsername;Password=$plainPassword;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
    
    az webapp config connection-string set `
        --resource-group $ResourceGroupName `
        --name $AppName `
        --connection-string-type SQLAzure `
        --settings DefaultConnection=$connectionString

    Write-Host "✅ Database connection configured" -ForegroundColor Green

    # Cleanup
    Write-Host "🧹 Cleaning up deployment files..." -ForegroundColor Cyan
    if (Test-Path "./publish") {
        Remove-Item "./publish" -Recurse -Force
    }
    if (Test-Path "./publish.zip") {
        Remove-Item "./publish.zip" -Force
    }

    # Success message
    Write-Host "`n🎉 DEPLOYMENT SUCCESSFUL!" -ForegroundColor Green
    Write-Host "🌐 Your application is now live at: https://$AppName.azurewebsites.net" -ForegroundColor Cyan
    Write-Host "📊 Resource Group: $ResourceGroupName" -ForegroundColor Cyan
    Write-Host "🗄️ SQL Server: $SqlServerName.database.windows.net" -ForegroundColor Cyan
    Write-Host "💾 Database: ECommercePlatformDB" -ForegroundColor Cyan
    Write-Host "`n⚡ Next Steps:" -ForegroundColor Yellow
    Write-Host "1. Visit your application at: https://$AppName.azurewebsites.net" -ForegroundColor White
    Write-Host "2. Test user registration and cart functionality" -ForegroundColor White
    Write-Host "3. Monitor with Application Insights in Azure Portal" -ForegroundColor White
    
} catch {
    Write-Host "`n❌ DEPLOYMENT FAILED!" -ForegroundColor Red
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "`n🔧 Troubleshooting:" -ForegroundColor Yellow
    Write-Host "1. Ensure Azure CLI is installed and updated" -ForegroundColor White
    Write-Host "2. Verify you have sufficient Azure permissions" -ForegroundColor White
    Write-Host "3. Check if app name '$AppName' is unique" -ForegroundColor White
    Write-Host "4. Verify Azure subscription is active" -ForegroundColor White
    exit 1
}
