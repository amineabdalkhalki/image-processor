public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        // Add controllers and Swagger
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        // Add custom services
        services.AddCustomServices();

        // Configure CORS based on environment
        var isDevelopment = Configuration.GetValue<bool>("IsDevelopment");
        services.AddCustomCors(isDevelopment);
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // Configure Swagger
        app.UseSwagger();
        app.UseSwaggerUI();

        // Configure error handling
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseHsts();
        }

        // Configure middleware pipeline
        app.UseHttpsRedirection();
        app.UseRouting();

        // Enable CORS based on environment
        app.UseCors(env.IsDevelopment() ? "AllowLocalhost" : "AllowS3Frontend");

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}