using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Options;
using Xunit;

namespace PGLN.Media.Storage.Azure.Tests;

/// <summary>
/// Test fixture that creates a unique container and disposes it after tests.
/// </summary>
public class StorageTestFixture : IDisposable
{
    /// <summary>
    /// Configuration options for Azure Blob Storage, including the connection string
    /// and a randomly generated container name.
    /// </summary>
    public AzureBlobOptions Options { get; }

    private readonly BlobContainerClient _containerClient;

    /// <summary>
    /// Initializes a new instance of <see cref="StorageTestFixture"/>.
    /// Reads the connection string from the environment variable
    /// AZURE_STORAGE_CONNECTION_STRING and creates a test container.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the environment variable is not set.
    /// </exception>
    public StorageTestFixture()
    {
        var connectionString = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING")
            ?? throw new InvalidOperationException(
                "Environment variable AZURE_STORAGE_CONNECTION_STRING not set. " +
                "Please set it to your Azure Storage connection string.");

        Options = new AzureBlobOptions
        {
            ConnectionString = connectionString,
            ContainerName = "test-container-" + Guid.NewGuid().ToString("N"),
            CreateContainerIfNotExists = true,
            BlobPrefix = null // default, can be overridden per test
        };

        _containerClient = new BlobContainerClient(connectionString, Options.ContainerName);
        _containerClient.CreateIfNotExists();
    }

    /// <summary>
    /// Deletes the test container after all tests have run.
    /// </summary>
    public void Dispose()
    {
        _containerClient.DeleteIfExists();
    }
}

/// <summary>
/// Test suite for <see cref="AzureBlobStorage"/> using a real Azure Storage account.
/// </summary>
public class AzureBlobStorageTests : IClassFixture<StorageTestFixture>
{
    private readonly AzureBlobStorage _storage;
    private readonly StorageTestFixture _fixture;

    /// <summary>
    /// Initializes a new instance of <see cref="AzureBlobStorageTests"/> with the shared fixture.
    /// </summary>
    /// <param name="fixture">The test fixture providing the storage options.</param>
    public AzureBlobStorageTests(StorageTestFixture fixture)
    {
        _fixture = fixture;
        var optionsWrapper = Options.Create(fixture.Options);
        _storage = new AzureBlobStorage(optionsWrapper);
    }

    /// <summary>
    /// Tests that saving a file and then retrieving it returns the same content.
    /// </summary>
    [Fact]
    public async Task SaveAndGet_ShouldWork()
    {
        // Arrange
        var content = "Hello, World!";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        var fileName = "test.txt";
        var contentType = "text/plain";

        // Act
        var path = await _storage.SaveAsync(stream, fileName, contentType);
        var resultStream = await _storage.GetAsync(path);
        using var reader = new StreamReader(resultStream);
        var result = await reader.ReadToEndAsync();

        // Assert
        Assert.Equal(content, result);
    }

    /// <summary>
    /// Tests that a file saved with a prefix is stored in a virtual folder.
    /// </summary>
    [Fact]
    public async Task SaveWithPrefix_ShouldStoreInPrefixedPath()
    {
        // Arrange
        _fixture.Options.BlobPrefix = "uploads/";
        var optionsWrapper = Options.Create(_fixture.Options);
        var storageWithPrefix = new AzureBlobStorage(optionsWrapper);

        var content = "File with prefix";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        var fileName = "prefixed.txt";
        var contentType = "text/plain";

        // Act
        var path = await storageWithPrefix.SaveAsync(stream, fileName, contentType);

        // Assert
        Assert.StartsWith("uploads/", path);
    }

    /// <summary>
    /// Tests that attempting to get a non-existent blob throws <see cref="FileNotFoundException"/>.
    /// </summary>
    [Fact]
    public async Task GetNonExistent_ShouldThrowFileNotFoundException()
    {
        // Arrange
        var nonExistentPath = "non-existent-file.txt";

        // Act & Assert
        await Assert.ThrowsAsync<FileNotFoundException>(() =>
            _storage.GetAsync(nonExistentPath));
    }

    /// <summary>
    /// Tests that deleting a non-existent blob returns false.
    /// </summary>
    [Fact]
    public async Task DeleteNonExistent_ShouldReturnFalse()
    {
        // Arrange
        var nonExistentPath = "non-existent-file.txt";

        // Act
        var result = await _storage.DeleteAsync(nonExistentPath);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// Tests that saving a file with a null stream throws <see cref="ArgumentNullException"/>.
    /// </summary>
    [Fact]
    public async Task SaveWithNullStream_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _storage.SaveAsync(null!, "test.txt", "text/plain"));
    }

    /// <summary>
    /// Tests that saving a file with an empty file name throws <see cref="ArgumentException"/>.
    /// </summary>
    [Fact]
    public async Task SaveWithEmptyFileName_ShouldThrowArgumentException()
    {
        // Arrange
        var stream = new MemoryStream(Encoding.UTF8.GetBytes("content"));

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _storage.SaveAsync(stream, "", "text/plain"));
    }

    /// <summary>
    /// Tests that <see cref="AzureBlobStorage.GetBlobUri"/> returns a valid URI.
    /// </summary>
    [Fact]
    public async Task GetBlobUri_ShouldReturnValidUri()
    {
        // Arrange
        var content = "Test content for URI";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        var fileName = "uri-test.txt";
        var path = await _storage.SaveAsync(stream, fileName, "text/plain");

        // Act
        var uri = _storage.GetBlobUri(path);

        // Assert
        Assert.NotNull(uri);
        Assert.Contains(path, uri.ToString());
    }

    /// <summary>
    /// Tests that saving a file with an extremely long file name still works.
    /// </summary>
    [Fact]
    public async Task SaveWithLongFileName_ShouldWork()
    {
        // Arrange
        var longFileName = new string('a', 200) + ".txt";
        var content = "Long file name test";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

        // Act
        var path = await _storage.SaveAsync(stream, longFileName, "text/plain");

        // Assert
        Assert.NotNull(path);
    }

    /// <summary>
    /// Tests that saving a file and then deleting it removes it from storage.
    /// </summary>
    [Fact]
    public async Task SaveAndDelete_ShouldRemoveFile()
    {
        // Arrange
        var content = "To be deleted";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        var fileName = "delete-me.txt";
        var path = await _storage.SaveAsync(stream, fileName, "text/plain");

        // Act
        var deleteResult = await _storage.DeleteAsync(path);

        // Assert
        Assert.True(deleteResult);

        // Verify the file no longer exists
        await Assert.ThrowsAsync<FileNotFoundException>(() =>
            _storage.GetAsync(path));
    }
}