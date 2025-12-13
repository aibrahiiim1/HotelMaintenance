using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using HotelMaintenance.Infrastructure.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HotelMaintenance.Infrastructure.Services;

/// <summary>
/// File storage service implementation using Azure Blob Storage
/// </summary>
public class AzureBlobStorageService : IFileStorageService
{
    private readonly string _connectionString;
    private readonly ILogger<AzureBlobStorageService> _logger;

    public AzureBlobStorageService(
        IConfiguration configuration,
        ILogger<AzureBlobStorageService> logger)
    {
        _connectionString = configuration.GetConnectionString("AzureBlobStorage") 
            ?? throw new ArgumentNullException("Azure Blob Storage connection string not configured");
        _logger = logger;
    }

    public async Task<string> UploadFileAsync(
        Stream fileStream, 
        string fileName, 
        string containerName, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var containerClient = new BlobContainerClient(_connectionString, containerName.ToLower());
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.None, cancellationToken: cancellationToken);

            var blobClient = containerClient.GetBlobClient(fileName);
            
            // Set content type based on file extension
            var contentType = GetContentType(fileName);
            var blobHttpHeaders = new BlobHttpHeaders { ContentType = contentType };

            await blobClient.UploadAsync(
                fileStream, 
                new BlobUploadOptions { HttpHeaders = blobHttpHeaders }, 
                cancellationToken);

            _logger.LogInformation("File {FileName} uploaded successfully to container {ContainerName}", fileName, containerName);

            return blobClient.Uri.ToString();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file {FileName} to container {ContainerName}", fileName, containerName);
            throw;
        }
    }

    public async Task<byte[]> DownloadFileAsync(
        string fileName, 
        string containerName, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var containerClient = new BlobContainerClient(_connectionString, containerName.ToLower());
            var blobClient = containerClient.GetBlobClient(fileName);

            if (!await blobClient.ExistsAsync(cancellationToken))
            {
                throw new FileNotFoundException($"File {fileName} not found in container {containerName}");
            }

            using var memoryStream = new MemoryStream();
            await blobClient.DownloadToAsync(memoryStream, cancellationToken);
            
            _logger.LogInformation("File {FileName} downloaded successfully from container {ContainerName}", fileName, containerName);
            
            return memoryStream.ToArray();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading file {FileName} from container {ContainerName}", fileName, containerName);
            throw;
        }
    }

    public async Task<bool> DeleteFileAsync(
        string fileName, 
        string containerName, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var containerClient = new BlobContainerClient(_connectionString, containerName.ToLower());
            var blobClient = containerClient.GetBlobClient(fileName);

            var result = await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);
            
            if (result.Value)
            {
                _logger.LogInformation("File {FileName} deleted successfully from container {ContainerName}", fileName, containerName);
            }
            
            return result.Value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file {FileName} from container {ContainerName}", fileName, containerName);
            return false;
        }
    }

    public async Task<string> GetFileUrlAsync(
        string fileName, 
        string containerName, 
        TimeSpan? expiryTime = null)
    {
        try
        {
            var containerClient = new BlobContainerClient(_connectionString, containerName.ToLower());
            var blobClient = containerClient.GetBlobClient(fileName);

            if (!await blobClient.ExistsAsync())
            {
                throw new FileNotFoundException($"File {fileName} not found in container {containerName}");
            }

            // Generate SAS token for temporary access
            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = containerName.ToLower(),
                BlobName = fileName,
                Resource = "b",
                ExpiresOn = DateTimeOffset.UtcNow.Add(expiryTime ?? TimeSpan.FromHours(1))
            };

            sasBuilder.SetPermissions(BlobSasPermissions.Read);

            var sasToken = blobClient.GenerateSasUri(sasBuilder);
            
            return sasToken.ToString();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating URL for file {FileName} in container {ContainerName}", fileName, containerName);
            throw;
        }
    }

    public async Task<bool> FileExistsAsync(
        string fileName, 
        string containerName, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var containerClient = new BlobContainerClient(_connectionString, containerName.ToLower());
            var blobClient = containerClient.GetBlobClient(fileName);

            return await blobClient.ExistsAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking existence of file {FileName} in container {ContainerName}", fileName, containerName);
            return false;
        }
    }

    private string GetContentType(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return extension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".pdf" => "application/pdf",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".xls" => "application/vnd.ms-excel",
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            ".txt" => "text/plain",
            ".csv" => "text/csv",
            ".zip" => "application/zip",
            _ => "application/octet-stream"
        };
    }
}
