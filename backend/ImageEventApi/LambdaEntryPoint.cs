using Amazon.Lambda.AspNetCoreServer;

namespace ImageEventApi
{
    public class LambdaEntryPoint : APIGatewayProxyFunction
    {
        protected override void Init(IWebHostBuilder builder)
        {
            // Use the Startup class which contains the public Configure method.
            builder.UseStartup<Startup>();
        }
    }
}