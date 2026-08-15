using Axlon.Services.Contracts.Models.Files;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Axlon.Services.Files.Controllers;

public sealed class FileTransferRequest
{
    [Required]
    public required IFormFile File { get; init; }

    [Required]
    public string Visibility { get; init; } = FileVisibilities.Tenant;
}
