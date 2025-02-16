public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;

        Console.WriteLine(@"
           _____                          _                 _____                                       _____ _____ 
          / ____|                        | |               |_   _|                                /\   |  __ \_   _|
         | (___   ___ _ ____   _____ _ __| | ___  ___ ___    | |  _ __ ___   __ _  __ _  ___     /  \  | |__) || |  
          \___ \ / _ \ '__\ \ / / _ \ '__| |/ _ \/ __/ __|   | | | '_ ` _ \ / _` |/ _` |/ _ \   / /\ \ |  ___/ | |  
          ____) |  __/ |   \ V /  __/ |  | |  __/\__ \__ \  _| |_| | | | | | (_| | (_| |  __/  / ____ \| |    _| |_ 
         |_____/_\___|_| _  \_/ \___|_|  |_|\___||___/___/ |_____|_| |_| |_|\__,_|\__, |\___| /_/    \_\_|   |_____|
         |__   __|      | |       /\             (_)                /\             __/ |                            
            | | ___  ___| |_     /  \   _ __ ___  _ _ __   ___     /  \           |___/                             
            | |/ _ \/ __| __|   / /\ \ | '_ ` _ \| | '_ \ / _ \   / /\ \                                            
            | |  __/\__ \ |_   / ____ \| | | | | | | | | |  __/  / ____ \ _                                         
            |_|\___||___/\__| /_/    \_\_| |_| |_|_|_| |_|\___| /_/    \_(_)  
        ");
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