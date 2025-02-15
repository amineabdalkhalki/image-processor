using ImageProcessorApi.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add CORS services
builder.Services.AddCors(options => {
    options.AddPolicy("AngularDev", policy => {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Register MemoryCache and ImageStorage
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<IImageStorage, ImageStorage>();

var app = builder.Build();

// Enable CORS middleware
app.UseCors("AngularDev");

// Configure middleware
app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Image API v1"));
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();