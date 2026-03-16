# PGLN.Media.Storage.Azure

Azure Blob Storage implementation of `IMediaStorage` for the PGLN.Media ecosystem.

## Features
-  Stores files in Azure Blob Storage with unique blob names
-  Automatically creates container if it doesn't exist
-  Configurable blob prefix (virtual folders)
-  Fully asynchronous with cancellation token support
-  Returns blob URI for public access scenarios

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

// Configure options
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
        
        // If you need the URL (requires public container or SAS token)
        if (_storage is AzureBlobStorage azureStorage)
        {
            var uri = azureStorage.GetBlobUri(blobPath);
            Console.WriteLine($"File available at: {uri}");
        }
        
        return blobPath;
    }
    
    public async Task<Stream> DownloadAsync(string blobPath)
    {
        return await _storage.GetAsync(blobPath);
    }
    
    public async Task<bool> DeleteAsync(string blobPath)
    {
        return await _storage.DeleteAsync(blobPath);
    }
}

Requirements
Azure Storage account

.NET 8.0 or later

