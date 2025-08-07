# Azure Monitoring and Diagnostics Guide

This guide explains how to use the Azure monitoring and diagnostics capabilities that have been added to troubleshoot issues like the login failures with demo credentials.

## Overview

The infrastructure now includes comprehensive monitoring using Azure's cheapest tiers:

- **Application Insights** (Free tier: 1GB/month free)
- **Log Analytics Workspace** (Free tier: 5GB/month free) 
- **App Service Diagnostic Logs** (Included with App Service)

## Monitoring Components

### 1. Application Insights
- Tracks application performance, errors, and custom telemetry
- Automatically collects .NET application logs and metrics
- Provides real-time monitoring and alerting capabilities

### 2. Log Analytics Workspace
- Centralized storage for all logs and metrics
- Powerful KQL (Kusto Query Language) querying capabilities
- Integrated with Application Insights for unified monitoring

### 3. App Service Diagnostics
- HTTP access logs
- Console output logs
- Application logs
- Platform logs
- Audit logs

## Accessing Monitoring Data

### Azure Portal
1. Navigate to Azure Portal → Resource Groups → `rg-todolist-{environment}`
2. Click on the Application Insights resource (`todolist-insights-{environment}-{suffix}`)
3. Use the following sections:
   - **Overview**: High-level metrics and health
   - **Failures**: Failed requests and exceptions
   - **Performance**: Response times and dependencies
   - **Logs**: Custom KQL queries

### Sample KQL Queries

#### View Application Logs
```kql
traces
| where timestamp > ago(1h)
| order by timestamp desc
| project timestamp, message, severityLevel
```

#### Database Seeding Logs
```kql
traces
| where message contains "seeding" or message contains "user" or message contains "database"
| where timestamp > ago(1h)
| order by timestamp desc
```

#### Login Attempt Analysis
```kql
traces
| where message contains "Login attempt" or message contains "Password verification" or message contains "User found" or message contains "User not found"
| where timestamp > ago(2h)
| order by timestamp desc
| project timestamp, message, severityLevel
```

#### Failed Requests
```kql
requests
| where success == false
| where timestamp > ago(1h)
| order by timestamp desc
| project timestamp, name, url, resultCode, duration
```

#### Database Connection Issues
```kql
traces
| where message contains "database" or message contains "connection"
| where timestamp > ago(1h)
| order by timestamp desc
```

## Troubleshooting Login Issues

Based on the original issue where login fails with demo credentials (alice/password123, bob/password123, carol/password123), here's how to diagnose:

### Step 1: Check if Database is Seeded
```kql
traces
| where message contains "Total users in database" or message contains "Created" and message contains "users"
| where timestamp > ago(1h)
| order by timestamp desc
```

### Step 2: Check Login Attempts
```kql
traces
| where message contains "Login attempt"
| where timestamp > ago(1h)
| extend username = extract(@"username: (\w+)", 1, message)
| project timestamp, username, message
| order by timestamp desc
```

### Step 3: Check Password Verification
```kql
traces
| where message contains "Password verification failed" or message contains "Password verified successfully"
| where timestamp > ago(1h)
| order by timestamp desc
```

### Step 4: Check User Lookup
```kql
traces
| where message contains "User not found" or message contains "User found"
| where timestamp > ago(1h)
| order by timestamp desc
```

## Database Configuration Fixed

The monitoring also revealed the root cause of the login issue:

**Previous Issue**: Application was using SQLite database in `/tmp` directory which gets cleared on Azure App Service restarts.

**Solution**: Changed to InMemory database for Azure dev environment:
```csharp
if (isAzure)
{
    // Use InMemory database for Azure dev environment to avoid file system issues
    options.UseInMemoryDatabase("TodoListDb");
}
```

This ensures:
- Database is recreated on each restart
- Data is always seeded with demo users
- No file system dependency issues
- Consistent behavior across restarts

## Enhanced Logging

The application now includes detailed logging for:

1. **Database Seeding**:
   - User creation details
   - Database connection status
   - Seeding completion confirmation

2. **Login Process**:
   - Login attempt logging with username
   - User lookup results
   - Password verification status
   - JWT token generation confirmation

3. **Error Handling**:
   - Detailed error messages for troubleshooting
   - Contextual information for debugging

## Cost Management

All monitoring resources use the cheapest tiers:

- **Application Insights**: Free tier (1GB/month)
- **Log Analytics**: Free tier (5GB/month) 
- **Retention**: 7 days to minimize storage costs
- **No premium features**: Basic monitoring without expensive add-ons

## Setting Up Alerts (Optional)

To get notified of issues:

1. Go to Application Insights → Alerts
2. Create alert rules for:
   - High error rate (> 5% failed requests)
   - Login failures (custom query on traces)
   - Application availability issues

## Integration with CI/CD

The infrastructure deployment now outputs monitoring resource information:

```yaml
- 📊 Application Insights: {app-insights-name}
- 📝 Log Analytics Workspace: {log-analytics-name}
```

This allows for automated monitoring setup and integration with other tools.

## Next Steps

With monitoring in place, you can now:

1. **Monitor real-time application health**
2. **Diagnose login and authentication issues** 
3. **Track database seeding success**
4. **Set up proactive alerts for issues**
5. **Analyze user behavior and application performance**

The combination of InMemory database + comprehensive monitoring should resolve the login issues and provide ongoing visibility into application health.