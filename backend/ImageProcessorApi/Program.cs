using ImageProcessorApi.Models;
using ImageProcessorApi.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Image API", Version = "v1" });
});

// Register MemoryCache and ImageStorage
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<IImageStorage, ImageStorage>();

var app = builder.Build();

// Configure middleware
app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Image API v1"));
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();