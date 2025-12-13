namespace HotelMaintenance.Infrastructure.Interfaces;

/// <summary>
/// Email service interface
/// </summary>
public interface IEmailService
{
    Task<bool> SendEmailAsync(
        string to, 
        string subject, 
        string body, 
        bool isHtml = true, 
        CancellationToken cancellationToken = default);
    
    Task<bool> SendEmailAsync(
        IEnumerable<string> to, 
        string subject, 
        string body, 
        bool isHtml = true, 
        CancellationToken cancellationToken = default);
    
    Task<bool> SendEmailWithAttachmentAsync(
        string to, 
        string subject, 
        string body, 
        byte[] attachment, 
        string attachmentName, 
        CancellationToken cancellationToken = default);
}

/// <summary>
/// SMS service interface
/// </summary>
public interface ISmsService
{
    Task<bool> SendSmsAsync(
        string phoneNumber, 
        string message, 
        CancellationToken cancellationToken = default);
    
    Task<bool> SendBulkSmsAsync(
        IEnumerable<string> phoneNumbers, 
        string message, 
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Push notification service interface
/// </summary>
public interface IPushNotificationService
{
    Task<bool> SendPushNotificationAsync(
        int userId, 
        string title, 
        string body, 
        Dictionary<string, string>? data = null, 
        CancellationToken cancellationToken = default);
    
    Task<bool> SendBulkPushNotificationAsync(
        IEnumerable<int> userIds, 
        string title, 
        string body, 
        Dictionary<string, string>? data = null, 
        CancellationToken cancellationToken = default);
}

/// <summary>
/// File storage service interface (for Azure Blob Storage, AWS S3, etc.)
/// </summary>
public interface IFileStorageService
{
    Task<string> UploadFileAsync(
        Stream fileStream, 
        string fileName, 
        string containerName, 
        CancellationToken cancellationToken = default);
    
    Task<byte[]> DownloadFileAsync(
        string fileName, 
        string containerName, 
        CancellationToken cancellationToken = default);
    
    Task<bool> DeleteFileAsync(
        string fileName, 
        string containerName, 
        CancellationToken cancellationToken = default);
    
    Task<string> GetFileUrlAsync(
        string fileName, 
        string containerName, 
        TimeSpan? expiryTime = null);
    
    Task<bool> FileExistsAsync(
        string fileName, 
        string containerName, 
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Report generation service interface
/// </summary>
public interface IReportService
{
    Task<byte[]> GenerateExcelReportAsync<T>(
        IEnumerable<T> data, 
        string sheetName, 
        CancellationToken cancellationToken = default);
    
    Task<byte[]> GeneratePdfReportAsync(
        string htmlContent, 
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Date/Time service interface for timezone handling
/// </summary>
public interface IDateTimeService
{
    DateTime UtcNow { get; }
    DateTime Now { get; }
    DateTime ConvertToTimeZone(DateTime utcDateTime, string timeZoneId);
    DateTime ConvertFromTimeZone(DateTime localDateTime, string timeZoneId);
}
