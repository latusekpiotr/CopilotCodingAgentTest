# CI/CD Pipelines for TodoList Application

This repository contains GitHub Actions workflows for deploying a .NET 8 + React application to Azure App Service.

## Overview

The CI/CD setup includes three manually-triggered workflows:

1. **Deploy Infrastructure** - Creates Azure resources using Bicep
2. **Destroy Infrastructure** - Removes all Azure resources
3. **Deploy Application** - Builds and deploys the application to existing infrastructure

## Prerequisites

### Azure Setup

1. **Azure Subscription** - You need an active Azure subscription
2. **Service Principal** - Create a service principal with Contributor role

### GitHub Setup

You need to configure the following GitHub secret:

#### `AZURE_CREDENTIALS`

Create a service principal and store its credentials as a GitHub secret:

```bash
# Create service principal
az ad sp create-for-rbac --name "todolist-github-actions" --role contributor --scopes /subscriptions/{subscription-id} --sdk-auth
```

Copy the entire JSON output and store it as `AZURE_CREDENTIALS` in GitHub Secrets.

Example format:
```json
{
  "clientId": "your-client-id",
  "clientSecret": "your-client-secret",
  "subscriptionId": "your-subscription-id",
  "tenantId": "your-tenant-id"
}
```

## Workflows

### 1. Deploy Infrastructure (`deploy-infrastructure.yml`)

**Purpose**: Creates Azure resources using Infrastructure as Code (Bicep)

**Trigger**: Manual (workflow_dispatch)

**Parameters**:
- `environment`: Target environment (dev/staging/prod)
- `location`: Azure region (default: West Europe)

**Resources Created**:
- Resource Group
- App Service Plan (Basic B1)
- App Service (Linux, .NET 8.0)

**Usage**:
1. Go to Actions tab in GitHub
2. Select "Deploy Infrastructure to Azure"
3. Click "Run workflow"
4. Select environment and location
5. Click "Run workflow"

### 2. Destroy Infrastructure (`destroy-infrastructure.yml`)

**Purpose**: Removes all Azure resources for the specified environment

**Trigger**: Manual (workflow_dispatch)

**Parameters**:
- `environment`: Environment to destroy (dev/staging/prod)
- `confirm_destroy`: Must type "DESTROY" exactly to proceed

**Safety Features**:
- Requires explicit confirmation
- Lists resources before deletion
- Graceful handling of non-existent resources

**Usage**:
1. Go to Actions tab in GitHub
2. Select "Destroy Azure Infrastructure"
3. Click "Run workflow"
4. Select environment
5. Type "DESTROY" in confirmation field
6. Click "Run workflow"

### 3. Deploy Application (`deploy-application.yml`)

**Purpose**: Builds and deploys the TodoList application

**Trigger**: Manual (workflow_dispatch)

**Parameters**:
- `environment`: Target environment (dev/staging/prod)

**Build Process**:
1. Builds .NET backend
2. Builds React frontend
3. Packages both into deployment zip
4. Deploys to Azure App Service
5. Serves frontend as static files from wwwroot

**Usage**:
1. Go to Actions tab in GitHub
2. Select "Deploy Application to Azure"
3. Click "Run workflow"
4. Select environment
5. Click "Run workflow"

## Architecture

### Application Structure
```
├── ToDoList.Backend/          # .NET 8.0 Web API
├── todolist-frontend/         # React TypeScript app
└── infrastructure/            # Azure Bicep templates
```

### Azure Resources
```
Resource Group
└── App Service Plan (Basic B1)
    └── App Service (Linux)
        ├── Backend API (/api/*)
        ├── Frontend (/)
        └── Swagger (/swagger)
```

### Deployment Package
```
deployment.zip
├── [.NET backend files]
└── wwwroot/
    └── [React build files]
```

## Cost Optimization

- **App Service Plan**: Basic B1 tier (~$13/month)
- **No API Management**: Avoided due to high cost
- **No diagnostics**: Can be added later if needed
- **Auto-scale disabled**: Keeps costs predictable

## Security Features

- HTTPS only enforced
- TLS 1.2 minimum
- FTP disabled
- Azure credentials stored in GitHub Secrets
- Resource groups isolated by environment

## Monitoring

After deployment, your application will be available at:
- **Frontend**: `https://{app-name}.azurewebsites.net/`
- **API**: `https://{app-name}.azurewebsites.net/api/`
- **Swagger**: `https://{app-name}.azurewebsites.net/swagger/`

## Troubleshooting

### Common Issues

1. **"No App Service found"**
   - Run "Deploy Infrastructure" workflow first
   - Check the resource group exists in Azure Portal

2. **Authentication failures**
   - Verify `AZURE_CREDENTIALS` secret is correctly formatted
   - Ensure service principal has Contributor role

3. **Build failures**
   - Check .NET and Node.js versions in workflows
   - Verify dependencies can be restored

4. **Deployment hangs**
   - Check App Service logs in Azure Portal
   - Verify deployment package size (< 2GB limit)

### Getting Help

1. Check workflow logs in GitHub Actions
2. Review App Service logs in Azure Portal
3. Verify resource status in Azure Portal
4. Test local builds before deploying

## Development Workflow

Recommended order of operations:

1. **First time setup**:
   ```
   Deploy Infrastructure → Deploy Application
   ```

2. **Regular updates**:
   ```
   Deploy Application (infrastructure already exists)
   ```

3. **Environment cleanup**:
   ```
   Destroy Infrastructure
   ```

## Next Steps

After successful deployment:

1. **Custom Domain**: Configure custom domain in Azure Portal
2. **SSL Certificate**: Add custom SSL if using custom domain
3. **Monitoring**: Enable Application Insights for detailed monitoring
4. **Scaling**: Consider upgrading to Standard tier for auto-scaling
5. **Backup**: Configure App Service backup for production