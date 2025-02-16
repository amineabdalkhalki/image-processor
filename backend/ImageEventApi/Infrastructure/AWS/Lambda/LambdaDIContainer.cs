using ImageEventApi.Core.Application.Services;
using ImageEventApi.Core.Domain.Interfaces;

namespace ImageEventApi.Infrastructure.AWS.Lambda
{
    public class LambdaDIContainer
    {
        public static readonly IServiceProvider ServiceProvider;

        static LambdaDIContainer()
        {
            var services = new ServiceCollection();
            services.AddMemoryCache(); // Register memory cache as a singleton
            services.AddSingleton<IImageStorageService, ImageStorageService>(); // Your storage service using the memory cache
            ServiceProvider = services.BuildServiceProvider();
        }
    }
}
