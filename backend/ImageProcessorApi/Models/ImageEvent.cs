﻿using System.ComponentModel.DataAnnotations;

namespace ImageProcessorApi.Models;

public record ImageEvent
{
    [Required(ErrorMessage = "ImageUrl is required.")]
    [Url(ErrorMessage = "ImageUrl must be a valid URL.")]
    public string ImageUrl { get; set; } = string.Empty;

    [Required(ErrorMessage = "Description is required.")]
    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
    public string Description { get; set; } = string.Empty;

    public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;
}