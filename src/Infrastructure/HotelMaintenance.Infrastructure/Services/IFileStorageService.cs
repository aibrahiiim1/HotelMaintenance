using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using HotelMaintenance.Application.Common;
using HotelMaintenance.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace HotelMaintenance.Infrastructure.Services;

/// <summary>
/// Interface for file storage operations
/// </summary>
public interface IFileStorageService
{
    Task<Result<FileUploadResult>> UploadFileAsync(IFormFile file, string containerName, CancellationToken cancellationToken = default);
    Task<Result<FileUploadResult>> UploadFileAsync(byte[] fileContent, string fileName, string contentType, string containerName, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteFileAsync(string filePath, string containerName, CancellationToken cancellationToken = default);
    Task<Result<Stream>> DownloadFileAsync(string filePath, string containerName, CancellationToken cancellationToken = default);
    Task<Result<string>> GetFileUrlAsync(string filePath, string containerName, CancellationToken cancellationToken = default);
}

/// <summary>
/// File upload result containing file metadata
/// </summary>
public class FileUploadResult
{
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public AttachmentType AttachmentType { get; set; }
}

/// <summary>
/// Azure Blob Storage implementation of file storage service
/// Falls back to local file system if Azure is not configured
/// </summary>
public class FileStorageService : IFileStorageService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<FileStorageService> _logger;
    private readonly BlobServiceClient? _blobServiceClient;
    private readonly bool _useAzureStorage;
    private readonly string _localStoragePath;

    // Allowed file extensions and max file sizes
    private const long MaxImageSizeBytes = 10 * 1024 * 1024; // 10MB
    private const long MaxVideoSizeBytes = 100 * 1024 * 1024; // 100MB
    private const long MaxDocumentSizeBytes = 25 * 1024 * 1024; // 25MB

    private static readonly string[] AllowedImageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
    private static readonly string[] AllowedVideoExtensions = { ".mp4", ".avi", ".mov", ".wmv", ".mkv" };
    private static readonly string[] AllowedAudioExtensions = { ".mp3", ".wav", ".m4a", ".aac", ".ogg" };
    private static readonly string[] AllowedDocumentExtensions = { ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".txt", ".csv" };

    public FileStorageService(IConfiguration configuration, ILogger<FileStorageService> logger)
    {
        _configuration = configuration;
        _logger = logger;

        var connectionString = _configuration.GetConnectionString("AzureBlobStorage");
        _useAzureStorage = !string.IsNullOrEmpty(connectionString);

        if (_useAzureStorage)
        {
            try
            {
                _blobServiceClient = new BlobServiceClient(connectionString);
                _logger.LogInformation("Azure Blob Storage initialized successfully");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to initialize Azure Blob Storage, falling back to local storage");
                _useAzureStorage = false;
            }
        }

        // Local storage fallback path
        _localStoragePath = _configuration["FileStorage:LocalPath"] ?? Path.Combine(Directory.GetCurrentDirectory(), "uploads");
        if (!Directory.Exists(_localStoragePath))
        {
            Directory.CreateDirectory(_localStoragePath);
        }
    }

    public async Task<Result<FileUploadResult>> UploadFileAsync(IFormFile file, string containerName, CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate file
            var validationResult = ValidateFile(file);
            if (!validationResult.IsSuccess)
            {
                return Result<FileUploadResult>.Failure(validationResult.Errors);
            }

            // Read file content
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream, cancellationToken);
            var fileContent = memoryStream.ToArray();

            return await UploadFileAsync(fileContent, file.FileName, file.ContentType, containerName, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file {FileName}", file.FileName);
            return Result<FileUploadResult>.Failure($"Error uploading file: {ex.Message}");
        }
    }

    public async Task<Result<FileUploadResult>> UploadFileAsync(byte[] fileContent, string fileName, string contentType, string containerName, CancellationToken cancellationToken = default)
    {
        try
        {
            var sanitizedFileName = SanitizeFileName(fileName);
            var uniqueFileName = GenerateUniqueFileName(sanitizedFileName);
            var attachmentType = DetermineAttachmentType(fileName);

            if (_useAzureStorage && _blobServiceClient != null)
            {
                return await UploadToAzureBlobAsync(fileContent, uniqueFileName, contentType, containerName, attachmentType, cancellationToken);
            }
            else
            {
                return await UploadToLocalStorageAsync(fileContent, uniqueFileName, contentType, containerName, attachmentType);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file {FileName}", fileName);
            return Result<FileUploadResult>.Failure($"Error uploading file: {ex.Message}");
        }
    }

    public async Task<Result<bool>> DeleteFileAsync(string filePath, string containerName, CancellationToken cancellationToken = default)
    {
        try
        {
            if (_useAzureStorage && _blobServiceClient != null)
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
                var blobClient = containerClient.GetBlobClient(filePath);
                await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);
            }
            else
            {
                var localPath = Path.Combine(_localStoragePath, containerName, filePath);
                if (File.Exists(localPath))
                {
                    File.Delete(localPath);
                }
            }

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file {FilePath}", filePath);
            return Result<bool>.Failure($"Error deleting file: {ex.Message}");
        }
    }

    public async Task<Result<Stream>> DownloadFileAsync(string filePath, string containerName, CancellationToken cancellationToken = default)
    {
        try
        {
            if (_useAzureStorage && _blobServiceClient != null)
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
                var blobClient = containerClient.GetBlobClient(filePath);
                var downloadResult = await blobClient.DownloadAsync(cancellationToken);
                return Result<Stream>.Success(downloadResult.Value.Content);
            }
            else
            {
                var localPath = Path.Combine(_localStoragePath, containerName, filePath);
                if (!File.Exists(localPath))
                {
                    return Result<Stream>.Failure("File not found");
                }

                var stream = File.OpenRead(localPath);
                return Result<Stream>.Success(stream);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading file {FilePath}", filePath);
            return Result<Stream>.Failure($"Error downloading file: {ex.Message}");
        }
    }

    public async Task<Result<string>> GetFileUrlAsync(string filePath, string containerName, CancellationToken cancellationToken = default)
    {
        try
        {
            if (_useAzureStorage && _blobServiceClient != null)
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
                var blobClient = containerClient.GetBlobClient(filePath);
                var url = blobClient.Uri.ToString();
                return Result<string>.Success(url);
            }
            else
            {
                // For local storage, return a relative URL
                var url = $"/api/files/{containerName}/{filePath}";
                return Result<string>.Success(url);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting file URL {FilePath}", filePath);
            return Result<string>.Failure($"Error getting file URL: {ex.Message}");
        }
    }

    #region Private Helper Methods

    private async Task<Result<FileUploadResult>> UploadToAzureBlobAsync(byte[] fileContent, string fileName, string contentType, string containerName, AttachmentType attachmentType, CancellationToken cancellationToken)
    {
        var containerClient = _blobServiceClient!.GetBlobContainerClient(containerName.ToLower());
        await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob, cancellationToken: cancellationToken);

        var blobClient = containerClient.GetBlobClient(fileName);

        using var stream = new MemoryStream(fileContent);
        await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = contentType }, cancellationToken: cancellationToken);

        var result = new FileUploadResult
        {
            FileName = fileName,
            FilePath = fileName,
            FileUrl = blobClient.Uri.ToString(),
            FileSize = fileContent.Length,
            ContentType = contentType,
            AttachmentType = attachmentType
        };

        _logger.LogInformation("File uploaded to Azure Blob Storage: {FileName}", fileName);
        return Result<FileUploadResult>.Success(result);
    }

    private async Task<Result<FileUploadResult>> UploadToLocalStorageAsync(byte[] fileContent, string fileName, string contentType, string containerName, AttachmentType attachmentType)
    {
        var containerPath = Path.Combine(_localStoragePath, containerName);
        if (!Directory.Exists(containerPath))
        {
            Directory.CreateDirectory(containerPath);
        }

        var filePath = Path.Combine(containerPath, fileName);
        await File.WriteAllBytesAsync(filePath, fileContent);

        var result = new FileUploadResult
        {
            FileName = fileName,
            FilePath = fileName,
            FileUrl = $"/api/files/{containerName}/{fileName}",
            FileSize = fileContent.Length,
            ContentType = contentType,
            AttachmentType = attachmentType
        };

        _logger.LogInformation("File uploaded to local storage: {FileName}", fileName);
        return Result<FileUploadResult>.Success(result);
    }

    private Result<bool> ValidateFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return Result<bool>.Failure("File is empty");
        }

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        var attachmentType = DetermineAttachmentType(file.FileName);

        // Check file extension
        var isValidExtension = attachmentType switch
        {
            AttachmentType.Photo => AllowedImageExtensions.Contains(extension),
            AttachmentType.Video => AllowedVideoExtensions.Contains(extension),
            AttachmentType.Audio => AllowedAudioExtensions.Contains(extension),
            AttachmentType.Document => AllowedDocumentExtensions.Contains(extension),
            _ => false
        };

        if (!isValidExtension)
        {
            return Result<bool>.Failure($"File type {extension} is not allowed");
        }

        // Check file size
        var maxSize = attachmentType switch
        {
            AttachmentType.Photo => MaxImageSizeBytes,
            AttachmentType.Video => MaxVideoSizeBytes,
            AttachmentType.Audio => MaxDocumentSizeBytes,
            AttachmentType.Document => MaxDocumentSizeBytes,
            _ => MaxDocumentSizeBytes
        };

        if (file.Length > maxSize)
        {
            return Result<bool>.Failure($"File size exceeds maximum allowed size of {maxSize / (1024 * 1024)}MB");
        }

        return Result<bool>.Success(true);
    }

    private static AttachmentType DetermineAttachmentType(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();

        if (AllowedImageExtensions.Contains(extension))
            return AttachmentType.Photo;
        if (AllowedVideoExtensions.Contains(extension))
            return AttachmentType.Video;
        if (AllowedAudioExtensions.Contains(extension))
            return AttachmentType.Audio;
        if (AllowedDocumentExtensions.Contains(extension))
            return AttachmentType.Document;

        return AttachmentType.Other;
    }

    private static string SanitizeFileName(string fileName)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        var sanitized = string.Join("_", fileName.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries));
        return sanitized;
    }

    private static string GenerateUniqueFileName(string originalFileName)
    {
        var extension = Path.GetExtension(originalFileName);
        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(originalFileName);
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var uniqueId = Guid.NewGuid().ToString("N").Substring(0, 8);
        return $"{fileNameWithoutExtension}_{timestamp}_{uniqueId}{extension}";
    }

    #endregion
}
