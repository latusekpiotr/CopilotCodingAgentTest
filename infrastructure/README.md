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
- Application Insights (Free tier)
- Log Analytics Workspace (Free tier)
- Diagnostic Settings (App Service logs → Application Insights)

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
├── App Service Plan (Basic B1)
├── Log Analytics Workspace (Free tier)
├── Application Insights (Free tier)
└── App Service (Linux)
    ├── Backend API (/api/*)
    ├── Frontend (/)
    ├── Swagger (/swagger)
    └── Diagnostics → Application Insights
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
- **Application Insights**: Free tier (1GB/month free, then ~$2.30/GB)
- **Log Analytics Workspace**: Free tier (5GB/month free)
- **No API Management**: Avoided due to high cost
- **Auto-scale disabled**: Keeps costs predictable
- **7-day retention**: Keeps storage costs minimal

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

### Application Insights & Monitoring

The infrastructure includes comprehensive monitoring capabilities:

- **Application Insights**: Tracks application performance, errors, and custom telemetry
- **Log Analytics Workspace**: Centralized logging with powerful query capabilities
- **App Service Diagnostics**: HTTP logs, console logs, application logs, and platform logs
- **Automatic Telemetry**: .NET application automatically sends telemetry to Application Insights

**Accessing Monitoring Data:**
1. Go to Azure Portal → Resource Groups → `rg-todolist-{environment}`
2. Click on the Application Insights resource
3. Use "Logs" for KQL queries, "Failures" for errors, "Performance" for response times

**Sample KQL Queries:**
```kql
// View application logs
traces
| where timestamp > ago(1h)
| order by timestamp desc

// View failed requests
requests
| where success == false
| where timestamp > ago(1h)
| order by timestamp desc

// View custom events (login attempts)
customEvents
| where name == "LoginAttempt"
| where timestamp > ago(1h)
| order by timestamp desc
```

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

5. **Login issues with demo credentials**
   - Check Application Insights logs for authentication errors
   - Query `traces` table for database seeding logs
   - Check `customEvents` for login attempt details
   - Verify database contains seeded users (alice, bob, carol)

### Getting Help

1. Check workflow logs in GitHub Actions
2. Review App Service logs in Azure Portal
3. **Check Application Insights for detailed application logs and telemetry**
4. **Use Log Analytics workspace for advanced log querying**
5. Verify resource status in Azure Portal
6. Test local builds before deploying

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
3. **Monitor Performance**: Use Application Insights to monitor application performance and errors
4. **Set up Alerts**: Configure Application Insights alerts for critical issues
5. **Scaling**: Consider upgrading to Standard tier for auto-scaling
6. **Backup**: Configure App Service backup for production