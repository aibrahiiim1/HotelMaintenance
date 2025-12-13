//using HotelMaintenance.API.Controllers;
//using HotelMaintenance.Application.DTOs.MaintenanceOrders;
//using HotelMaintenance.Application.Features.MaintenanceOrders.Commands;
//using HotelMaintenance.Domain.Entities;
//using HotelMaintenance.Infrastructure.Services;
//using iRely.Common;
//using MediatR;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;

//namespace HotelMaintenance.API.Controllers.V1;

///// <summary>
///// Controller for handling file uploads and downloads for maintenance orders
///// </summary>
//[ApiVersion("1.0")]
//public class FilesController : BaseApiController
//{
//    private readonly IFileStorageService _fileStorageService;
//    private readonly IMediator _mediator;

//    public FilesController(IFileStorageService fileStorageService, IMediator mediator)
//    {
//        _fileStorageService = fileStorageService;
//        _mediator = mediator;
//    }

//    /// <summary>
//    /// Upload an attachment for a maintenance order
//    /// </summary>
//    /// <param name="orderId">The maintenance order ID</param>
//    /// <param name="file">The file to upload</param>
//    /// <param name="cancellationToken">Cancellation token</param>
//    /// <returns>File upload result with URL</returns>
//    [HttpPost("orders/{orderId}/attachments")]
//    [Authorize(Roles = "MaintenanceStaff,Manager,Admin")]
//    [ProducesResponseType(typeof(FileUploadResponseDto), StatusCodes.Status200OK)]
//    [ProducesResponseType(StatusCodes.Status400BadRequest)]
//    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
//    [RequestSizeLimit(100_000_000)] // 100MB limit
//    public async Task<IActionResult> UploadOrderAttachment(int orderId, IFormFile file, CancellationToken cancellationToken)
//    {
//        if (file == null || file.Length == 0)
//        {
//            return BadRequest("No file provided");
//        }

//        // Upload file to storage
//        var uploadResult = await _fileStorageService.UploadFileAsync(file, "maintenance-orders", cancellationToken);

//        if (!uploadResult.IsSuccess)
//        {
//            return BadRequest(uploadResult.Error);
//        }

//        // Create attachment record in database
//        var command = new UploadOrderAttachmentCommand
//        {
//            MaintenanceOrderId = orderId,
//            FileName = uploadResult.Value.FileName,
//            FilePath = uploadResult.Value.FilePath,
//            FileUrl = uploadResult.Value.FileUrl,
//            FileSize = uploadResult.Value.FileSize,
//            ContentType = file.ContentType,
//            AttachmentType = uploadResult.Value.AttachmentType,
//            UploadedByUserId = GetCurrentUserId()
//        };

//        var result = await _mediator.Send(command, cancellationToken);

//        if (!result.IsSuccess)
//        {
//            // If database operation fails, try to delete the uploaded file
//            await _fileStorageService.DeleteFileAsync(uploadResult.Value.FilePath, "maintenance-orders", cancellationToken);
//            return BadRequest(result.Error);
//        }

//        var response = new FileUploadResponseDto
//        {
//            FileName = uploadResult.Value.FileName,
//            FileUrl = uploadResult.Value.FileUrl,
//            FilePath = uploadResult.Value.FilePath,
//            FileSize = uploadResult.Value.FileSize,
//            AttachmentType = uploadResult.Value.AttachmentType
//        };

//        return Ok(response);
//    }

//    /// <summary>
//    /// Upload multiple attachments for a maintenance order
//    /// </summary>
//    /// <param name="orderId">The maintenance order ID</param>
//    /// <param name="files">The files to upload</param>
//    /// <param name="cancellationToken">Cancellation token</param>
//    /// <returns>List of file upload results</returns>
//    [HttpPost("orders/{orderId}/attachments/bulk")]
//    [Authorize(Roles = "MaintenanceStaff,Manager,Admin")]
//    [ProducesResponseType(typeof(IEnumerable<FileUploadResponseDto>), StatusCodes.Status200OK)]
//    [ProducesResponseType(StatusCodes.Status400BadRequest)]
//    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
//    [RequestSizeLimit(100_000_000)] // 100MB limit
//    public async Task<IActionResult> UploadMultipleOrderAttachments(int orderId, List<IFormFile> files, CancellationToken cancellationToken)
//    {
//        if (files == null || files.Count == 0)
//        {
//            return BadRequest("No files provided");
//        }

//        var responses = new List<FileUploadResponseDto>();
//        var uploadedFiles = new List<string>();

//        try
//        {
//            foreach (var file in files)
//            {
//                if (file.Length == 0) continue;

//                // Upload file to storage
//                var uploadResult = await _fileStorageService.UploadFileAsync(file, "maintenance-orders", cancellationToken);

//                if (!uploadResult.IsSuccess)
//                {
//                    // Rollback: Delete previously uploaded files
//                    foreach (var uploadedFile in uploadedFiles)
//                    {
//                        await _fileStorageService.DeleteFileAsync(uploadedFile, "maintenance-orders", cancellationToken);
//                    }
//                    return BadRequest($"Error uploading {file.FileName}: {uploadResult.Error}");
//                }

//                uploadedFiles.Add(uploadResult.Value.FilePath);

//                // Create attachment record in database
//                var command = new UploadOrderAttachmentCommand
//                {
//                    MaintenanceOrderId = orderId,
//                    FileName = uploadResult.Value.FileName,
//                     = uploadResult.Value.FilePath,
//                    FileUrl = uploadResult.Value.FileUrl,
//                    FileSize = uploadResult.Value.FileSize,
//                    ContentType = file.ContentType,
//                    AttachmentType = uploadResult.Value.AttachmentType,
//                    UploadedByUserId = GetCurrentUserId()
//                };

//                var result = await _mediator.Send(command, cancellationToken);

//                if (!result.IsSuccess)
//                {
//                    // Rollback: Delete all uploaded files
//                    foreach (var uploadedFile in uploadedFiles)
//                    {
//                        await _fileStorageService.DeleteFileAsync(uploadedFile, "maintenance-orders", cancellationToken);
//                    }
//                    return BadRequest($"Error saving {file.FileName}: {result.Error}");
//                }

//                responses.Add(new FileUploadResponseDto
//                {
//                    FileName = uploadResult.Value.FileName,
//                    FileUrl = uploadResult.Value.FileUrl,
//                    FilePath = uploadResult.Value.FilePath,
//                    FileSize = uploadResult.Value.FileSize,
//                    AttachmentType = uploadResult.Value.AttachmentType
//                });
//            }

//            return Ok(responses);
//        }
//        catch (Exception ex)
//        {
//            // Rollback: Delete all uploaded files
//            foreach (var uploadedFile in uploadedFiles)
//            {
//                await _fileStorageService.DeleteFileAsync(uploadedFile, "maintenance-orders", cancellationToken);
//            }
//            return BadRequest($"Error uploading files: {ex.Message}");
//        }
//    }

//    /// <summary>
//    /// Download an attachment
//    /// </summary>
//    /// <param name="containerName">Container name (e.g., maintenance-orders)</param>
//    /// <param name="fileName">File name to download</param>
//    /// <param name="cancellationToken">Cancellation token</param>
//    /// <returns>File stream</returns>
//    [HttpGet("{containerName}/{fileName}")]
//    [AllowAnonymous] // Allow public access to uploaded files (adjust based on security requirements)
//    [ProducesResponseType(StatusCodes.Status200OK)]
//    [ProducesResponseType(StatusCodes.Status404NotFound)]
//    public async Task<IActionResult> DownloadFile(string containerName, string fileName, CancellationToken cancellationToken)
//    {
//        var result = await _fileStorageService.DownloadFileAsync(fileName, containerName, cancellationToken);

//        if (!result.IsSuccess)
//        {
//            return NotFound(result.Error);
//        }

//        // Determine content type based on file extension
//        var contentType = GetContentType(fileName);

//        return File(result.Value, contentType, fileName);
//    }

//    /// <summary>
//    /// Delete an attachment
//    /// </summary>
//    /// <param name="orderId">The maintenance order ID</param>
//    /// <param name="attachmentId">The attachment ID to delete</param>
//    /// <param name="cancellationToken">Cancellation token</param>
//    /// <returns>Success or failure result</returns>
//    [HttpDelete("orders/{orderId}/attachments/{attachmentId}")]
//    [Authorize(Roles = "MaintenanceStaff,Manager,Admin")]
//    [ProducesResponseType(StatusCodes.Status204NoContent)]
//    [ProducesResponseType(StatusCodes.Status404NotFound)]
//    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
//    public async Task<IActionResult> DeleteOrderAttachment(int orderId, int attachmentId, CancellationToken cancellationToken)
//    {
//        // This would need a delete command handler - for now, return not implemented
//        // TODO: Implement DeleteOrderAttachmentCommand
//        return NoContent();
//    }

//    private string GetContentType(string fileName)
//    {
//        var extension = Path.GetExtension(fileName).ToLowerInvariant();
//        return extension switch
//        {
//            ".jpg" or ".jpeg" => "image/jpeg",
//            ".png" => "image/png",
//            ".gif" => "image/gif",
//            ".bmp" => "image/bmp",
//            ".webp" => "image/webp",
//            ".mp4" => "video/mp4",
//            ".avi" => "video/x-msvideo",
//            ".mov" => "video/quicktime",
//            ".wmv" => "video/x-ms-wmv",
//            ".mkv" => "video/x-matroska",
//            ".mp3" => "audio/mpeg",
//            ".wav" => "audio/wav",
//            ".m4a" => "audio/mp4",
//            ".pdf" => "application/pdf",
//            ".doc" => "application/msword",
//            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
//            ".xls" => "application/vnd.ms-excel",
//            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
//            ".txt" => "text/plain",
//            ".csv" => "text/csv",
//            _ => "application/octet-stream"
//        };
//    }

//    private string GetCurrentUserId()
//    {
//        // Extract user ID from JWT claims
//        return User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
//    }
//}
