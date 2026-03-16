# PGLN.Media.Storage.Azure

Azure Blob Storage implementation of `IMediaStorage` for the PGLN.Media ecosystem.

[![NuGet](https://img.shields.io/nuget/v/PGLN.Media.Storage.Azure.svg)](https://www.nuget.org/packages/PGLN.Media.Storage.Azure/)

## Features
-  Store files in Azure Blob Storage with unique blob names
-  Automatically create container if it doesn't exist
-  Configurable blob prefix (virtual folders)
-  Fully asynchronous with cancellation token support
-  Retrieve blob URI for public access scenarios

## Installation
```bash
dotnet add package PGLN.Media.Storage.Azure

Configuration
appsettings.json
json
{
  "AzureBlobStorage": {
    "ConnectionString": "DefaultEndpointsProtocol=https;AccountName=...;AccountKey=...;EndpointSuffix=core.windows.net",
    "ContainerName": "media-files",
    "BlobPrefix": "uploads/",
    "CreateContainerIfNotExists": true
  }
}

Program.cs / Startup.cs
csharp
using PGLN.Media.Storage.Azure;
using PGLN.Media.Abstractions;

// Bind configuration
services.Configure<AzureBlobOptions>(
    Configuration.GetSection("AzureBlobStorage"));

// Register storage
services.AddSingleton<IMediaStorage, AzureBlobStorage>();
Usage
csharp
public class FileUploadService
{
    private readonly IMediaStorage _storage;
    
    public FileUploadService(IMediaStorage storage)
    {
        _storage = storage;
    }
    
    public async Task<string> UploadAsync(Stream fileStream, string fileName, string contentType)
    {
        // Save to Azure Blob Storage
        var blobPath = await _storage.SaveAsync(fileStream, fileName, contentType);
        return blobPath; // e.g., "uploads/12345abcde.jpg"
    }
    
    public async Task<Stream> DownloadAsync(string blobPath)
    {
        return await _storage.GetAsync(blobPath);
    }
    
    public async Task<bool> DeleteAsync(string blobPath)
    {
        return await _storage.DeleteAsync(blobPath);
    }
    
    // Get public URI (if container allows anonymous access or you generate SAS)
    public Uri GetBlobUri(string blobPath)
    {
        if (_storage is AzureBlobStorage azureStorage)
        {
            return azureStorage.GetBlobUri(blobPath);
        }
        throw new InvalidOperationException("Storage is not AzureBlobStorage");
    }
}
Requirements
.NET 8.0 or later

Azure Storage account with a valid connection string

Running Tests
Tests use a real Azure Storage account. Set the environment variable:

bash
$env:AZURE_STORAGE_CONNECTION_STRING = "your-connection-string"
dotnet test