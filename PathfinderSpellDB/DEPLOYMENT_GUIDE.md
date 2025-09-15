# Deployment Guide: Pathfinder 2E Spell Database

This guide covers various deployment options for the .NET Blazor application, from local development to production cloud deployment.

## 🚀 **Quick Start**

### Prerequisites
- .NET 8 SDK
- Docker (optional)
- Git

### Local Development
```bash
git clone <repository-url>
cd PathfinderSpellDB
dotnet restore
dotnet run
```

Access at: `http://localhost:5128`

## 🐳 **Docker Deployment**

### Using Dockerfile

1. **Build the image:**
```bash
docker build -t pathfinder-spell-db .
```

2. **Run the container:**
```bash
docker run -p 8080:80 pathfinder-spell-db
```

3. **Access the application:**
```
http://localhost:8080
```

### Using Docker Compose

1. **Start the services:**
```bash
docker-compose up -d
```

2. **View logs:**
```bash
docker-compose logs -f
```

3. **Stop the services:**
```bash
docker-compose down
```

### Docker Configuration Files

#### Dockerfile
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["PathfinderSpellDB.csproj", "."]
RUN dotnet restore "PathfinderSpellDB.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "PathfinderSpellDB.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PathfinderSpellDB.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PathfinderSpellDB.dll"]
```

#### docker-compose.yml
```yaml
version: '3.8'

services:
  pathfinder-spell-db:
    build: .
    ports:
      - "8080:80"
      - "8443:443"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ASPNETCORE_URLS=http://+:80
    volumes:
      - ./Data:/app/Data
    restart: unless-stopped
```

## ☁️ **Cloud Deployment Options**

### 1. Azure App Service

#### Prerequisites
- Azure CLI installed
- Azure subscription

#### Deployment Steps

1. **Create Azure App Service:**
```bash
az group create --name pathfinder-rg --location eastus
az appservice plan create --name pathfinder-plan --resource-group pathfinder-rg --sku B1 --is-linux
az webapp create --resource-group pathfinder-rg --plan pathfinder-plan --name pathfinder-spell-db --runtime "DOTNET|8.0"
```

2. **Deploy from Git:**
```bash
az webapp deployment source config --resource-group pathfinder-rg --name pathfinder-spell-db --repo-url <your-repo-url> --branch main --manual-integration
```

3. **Configure Application Settings:**
```bash
az webapp config appsettings set --resource-group pathfinder-rg --name pathfinder-spell-db --settings ASPNETCORE_ENVIRONMENT=Production
```

#### Azure Configuration

**appsettings.Production.json:**
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Server=tcp:your-server.database.windows.net,1433;Initial Catalog=pathfinder;Persist Security Info=False;User ID=your-username;Password=your-password;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
  }
}
```

### 2. AWS Elastic Beanstalk

#### Prerequisites
- AWS CLI configured
- EB CLI installed

#### Deployment Steps

1. **Initialize Elastic Beanstalk:**
```bash
eb init
```

2. **Create environment:**
```bash
eb create production
```

3. **Deploy application:**
```bash
eb deploy
```

#### AWS Configuration

**Dockerrun.aws.json:**
```json
{
  "AWSEBDockerrunVersion": "1",
  "Image": {
    "Name": "pathfinder-spell-db:latest",
    "Update": "true"
  },
  "Ports": [
    {
      "ContainerPort": "80"
    }
  ]
}
```

### 3. Google Cloud Run

#### Prerequisites
- Google Cloud SDK installed
- Docker installed

#### Deployment Steps

1. **Build and push to Google Container Registry:**
```bash
gcloud builds submit --tag gcr.io/PROJECT-ID/pathfinder-spell-db
```

2. **Deploy to Cloud Run:**
```bash
gcloud run deploy --image gcr.io/PROJECT-ID/pathfinder-spell-db --platform managed --region us-central1 --allow-unauthenticated
```

### 4. DigitalOcean App Platform

#### Prerequisites
- DigitalOcean account
- GitHub repository

#### Deployment Steps

1. **Create App in DigitalOcean Console**
2. **Connect GitHub repository**
3. **Configure build settings:**
   - Build Command: `dotnet publish -c Release -o ./publish`
   - Run Command: `dotnet PathfinderSpellDB.dll`
   - Source Directory: `/`

## 🏗️ **Traditional Server Deployment**

### Linux (Ubuntu/CentOS)

#### Prerequisites
- .NET 8 Runtime
- Nginx (reverse proxy)
- Systemd (service management)

#### Installation Steps

1. **Install .NET 8 Runtime:**
```bash
# Ubuntu
wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
sudo apt-get update
sudo apt-get install -y dotnet-runtime-8.0

# CentOS
sudo rpm -Uvh https://packages.microsoft.com/config/centos/7/packages-microsoft-prod.rpm
sudo yum install -y dotnet-runtime-8.0
```

2. **Deploy Application:**
```bash
sudo mkdir -p /var/www/pathfinder-spell-db
sudo cp -r . /var/www/pathfinder-spell-db/
sudo chown -R www-data:www-data /var/www/pathfinder-spell-db
```

3. **Create Systemd Service:**
```ini
# /etc/systemd/system/pathfinder-spell-db.service
[Unit]
Description=Pathfinder Spell Database
After=network.target

[Service]
Type=notify
ExecStart=/usr/bin/dotnet /var/www/pathfinder-spell-db/PathfinderSpellDB.dll
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=pathfinder-spell-db
User=www-data
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=ASPNETCORE_URLS=http://localhost:5000

[Install]
WantedBy=multi-user.target
```

4. **Configure Nginx:**
```nginx
# /etc/nginx/sites-available/pathfinder-spell-db
server {
    listen 80;
    server_name your-domain.com;

    location / {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
        proxy_cache_bypass $http_upgrade;
    }
}
```

5. **Enable and Start Services:**
```bash
sudo systemctl enable pathfinder-spell-db
sudo systemctl start pathfinder-spell-db
sudo systemctl enable nginx
sudo systemctl restart nginx
```

### Windows Server

#### Prerequisites
- .NET 8 Runtime
- IIS with ASP.NET Core Module

#### Installation Steps

1. **Install .NET 8 Runtime:**
   - Download from Microsoft website
   - Install ASP.NET Core Runtime 8.0

2. **Deploy to IIS:**
   - Copy application files to `C:\inetpub\wwwroot\pathfinder-spell-db`
   - Configure IIS site
   - Set application pool to "No Managed Code"

3. **Configure web.config:**
```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <location path="." inheritInChildApplications="false">
    <system.webServer>
      <handlers>
        <add name="aspNetCore" path="*" verb="*" modules="AspNetCoreModuleV2" resourceType="Unspecified" />
      </handlers>
      <aspNetCore processPath="dotnet" arguments=".\PathfinderSpellDB.dll" stdoutLogEnabled="false" stdoutLogFile=".\logs\stdout" hostingModel="inprocess" />
    </system.webServer>
  </location>
</configuration>
```

## 🔧 **Production Configuration**

### Environment Variables

```bash
# Production settings
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://+:80
ASPNETCORE_HTTPS_PORT=443

# Database (if using SQL Server)
ConnectionStrings__DefaultConnection="Server=localhost;Database=PathfinderSpellDB;Trusted_Connection=true;"

# Logging
Logging__LogLevel__Default=Information
Logging__LogLevel__Microsoft=Warning

# Security
ASPNETCORE_FORWARDEDHEADERS_ENABLED=true
```

### Performance Optimization

1. **Enable Response Compression:**
```csharp
// Program.cs
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
});
```

2. **Configure Caching:**
```csharp
// Program.cs
builder.Services.AddMemoryCache();
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379";
});
```

3. **Optimize SignalR:**
```csharp
// Program.cs
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = false;
    options.MaximumReceiveMessageSize = 32 * 1024;
});
```

## 📊 **Monitoring and Logging**

### Application Insights (Azure)

```csharp
// Program.cs
builder.Services.AddApplicationInsightsTelemetry();
```

### Serilog (Alternative)

```csharp
// Program.cs
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));
```

### Health Checks

```csharp
// Program.cs
builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy());

app.MapHealthChecks("/health");
```

## 🔒 **Security Considerations**

### HTTPS Configuration

1. **Development:**
```bash
dotnet dev-certs https --trust
```

2. **Production:**
```csharp
// Program.cs
app.UseHttpsRedirection();
app.UseHsts();
```

### Security Headers

```csharp
// Program.cs
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    await next();
});
```

## 🚀 **CI/CD Pipeline**

### GitHub Actions

```yaml
# .github/workflows/deploy.yml
name: Deploy to Azure

on:
  push:
    branches: [ main ]

jobs:
  build-and-deploy:
    runs-on: ubuntu-latest
    
    steps:
    - uses: actions/checkout@v2
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v1
      with:
        dotnet-version: '8.0.x'
    
    - name: Build
      run: dotnet build --configuration Release
    
    - name: Test
      run: dotnet test --configuration Release --no-build
    
    - name: Deploy to Azure
      uses: azure/webapps-deploy@v2
      with:
        app-name: 'pathfinder-spell-db'
        publish-profile: ${{ secrets.AZURE_WEBAPP_PUBLISH_PROFILE }}
        package: .
```

## 📋 **Deployment Checklist**

### Pre-Deployment
- [ ] All tests passing
- [ ] Environment variables configured
- [ ] Database migrations applied (if applicable)
- [ ] SSL certificates installed
- [ ] Monitoring configured

### Post-Deployment
- [ ] Health check endpoint responding
- [ ] Application accessible via HTTPS
- [ ] Performance monitoring active
- [ ] Logs being collected
- [ ] Backup strategy in place

## 🆘 **Troubleshooting**

### Common Issues

1. **Port Already in Use:**
```bash
sudo lsof -i :80
sudo kill -9 <PID>
```

2. **Permission Denied:**
```bash
sudo chown -R www-data:www-data /var/www/pathfinder-spell-db
sudo chmod -R 755 /var/www/pathfinder-spell-db
```

3. **Service Won't Start:**
```bash
sudo journalctl -u pathfinder-spell-db -f
```

4. **Nginx 502 Bad Gateway:**
```bash
sudo systemctl status pathfinder-spell-db
sudo nginx -t
```

This deployment guide provides comprehensive instructions for deploying the Pathfinder 2E Spell Database application across various platforms and environments.