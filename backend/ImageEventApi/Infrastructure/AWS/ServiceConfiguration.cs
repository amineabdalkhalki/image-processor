﻿using ImageEventApi;
using ImageEventApi.Core.Application.Services;
using ImageEventApi.Core.Domain.Interfaces;

public static class ServiceConfiguration
{
    public static void AddCustomServices(this IServiceCollection services)
    {
        // Add in-memory cache and storage service
        services.AddMemoryCache();
        services.AddSingleton<IImageStorageService, ImageStorageService>();
    }

    public static void AddCustomCors(this IServiceCollection services, bool isDevelopment)
    {
        if (isDevelopment)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowLocalhost", policy =>
                {
                    policy.WithOrigins("http://localhost:4200")
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });
        }
        else
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowS3Frontend", policy =>
                {
                    policy.WithOrigins("http://amzn-s3-frontend.s3-website.eu-west-3.amazonaws.com")
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });
        }
    }
}