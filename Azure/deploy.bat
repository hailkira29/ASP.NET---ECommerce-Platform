@echo off
echo.
echo ========================================
echo   E-Commerce Platform - Azure Deploy
echo ========================================
echo.

REM Check if Azure CLI is installed
az --version >nul 2>&1
if %errorlevel% neq 0 (
    echo [ERROR] Azure CLI not found!
    echo Please install from: https://docs.microsoft.com/en-us/cli/azure/install-azure-cli
    echo Or run: winget install Microsoft.AzureCLI
    pause
    exit /b 1
)

echo [OK] Azure CLI found

REM Get app name from user
set /p APP_NAME="Enter your unique app name (e.g., ecommerce-yourname-2025): "
if "%APP_NAME%"=="" (
    echo [ERROR] App name cannot be empty
    pause
    exit /b 1
)

echo.
echo [INFO] App Name: %APP_NAME%
echo [INFO] Resource Group: ecommerce-rg
echo [INFO] Location: East US
echo.

REM Login to Azure
echo [STEP 1] Logging into Azure...
az account show >nul 2>&1
if %errorlevel% neq 0 (
    echo Please login to Azure...
    az login
    if %errorlevel% neq 0 (
        echo [ERROR] Azure login failed
        pause
        exit /b 1
    )
)
echo [OK] Azure login confirmed

REM Create resource group
echo [STEP 2] Creating resource group...
az group create --name ecommerce-rg --location "East US"
if %errorlevel% neq 0 (
    echo [ERROR] Failed to create resource group
    pause
    exit /b 1
)
echo [OK] Resource group created

REM Deploy ARM template
echo [STEP 3] Deploying Azure resources...
az deployment group create ^
    --resource-group ecommerce-rg ^
    --template-file azure-deploy.json ^
    --parameters appName=%APP_NAME% ^
                sqlServerName=%APP_NAME%-sql ^
                sqlAdminUsername=ecommerceadmin ^
                sqlAdminPassword=SecurePassword123!

if %errorlevel% neq 0 (
    echo [ERROR] Failed to deploy Azure resources
    pause
    exit /b 1
)
echo [OK] Azure resources deployed

REM Build application
echo [STEP 4] Building application...
dotnet build --configuration Release
if %errorlevel% neq 0 (
    echo [ERROR] Build failed
    pause
    exit /b 1
)
echo [OK] Build completed

REM Publish application
echo [STEP 5] Publishing application...
if exist publish rmdir /s /q publish
dotnet publish --configuration Release --output publish
if %errorlevel% neq 0 (
    echo [ERROR] Publish failed
    pause
    exit /b 1
)
echo [OK] Publish completed

REM Create deployment package
echo [STEP 6] Creating deployment package...
if exist publish.zip del publish.zip
powershell -Command "Compress-Archive -Path 'publish\*' -DestinationPath 'publish.zip' -Force"
if %errorlevel% neq 0 (
    echo [ERROR] Failed to create deployment package
    pause
    exit /b 1
)
echo [OK] Deployment package created

REM Deploy to Azure App Service
echo [STEP 7] Deploying to Azure App Service...
az webapp deployment source config-zip ^
    --resource-group ecommerce-rg ^
    --name %APP_NAME% ^
    --src publish.zip

if %errorlevel% neq 0 (
    echo [ERROR] Failed to deploy to App Service
    pause
    exit /b 1
)
echo [OK] Application deployed to Azure!

echo.
echo ========================================
echo          DEPLOYMENT COMPLETE!
echo ========================================
echo.
echo Your app is available at:
echo https://%APP_NAME%.azurewebsites.net
echo.
echo Resource Group: ecommerce-rg
echo SQL Server: %APP_NAME%-sql.database.windows.net
echo Database: ECommercePlatformDB
echo.
echo Next Steps:
echo 1. Visit your app URL above
echo 2. Test user registration and cart functionality
echo 3. Monitor in Azure Portal
echo.

REM Cleanup
if exist publish rmdir /s /q publish
if exist publish.zip del publish.zip

pause
