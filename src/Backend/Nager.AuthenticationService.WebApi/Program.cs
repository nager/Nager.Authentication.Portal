using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Nager.Authentication.Abstraction.Models;
using Nager.Authentication.Abstraction.Services;
using Nager.Authentication.Abstraction.Validators;
using Nager.Authentication.Helpers;
using Nager.Authentication.Services;
using Nager.AuthenticationService.MssqlRepository;
using Nager.AuthenticationService.WebApi;
using System.Text;

var users = new UserInfoWithPassword[]
{
    new UserInfoWithPassword
    {
        EmailAddress = "admin@domain.com",
        Password = "password",
        Roles = new [] { "administrator" }
    }
};

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMemoryCache();

builder.Services.AddDbContextPool<DatabaseContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("Default");
    options.UseSqlServer(connectionString);
});
builder.Services.AddScoped<IUserRepository, MssqlUserRepository>();
builder.Services.AddSingleton<MigrationHelper>();

builder.Services.AddScoped<IUserAccountService, UserAccountService>();
builder.Services.AddScoped<IUserAuthenticationService, UserAuthenticationService>();
builder.Services.AddScoped<IUserManagementService, UserManagementService>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(configuration =>
{
    var issuer = builder.Configuration["Authentication:Tokens:Issuer"];
    var audience = builder.Configuration["Authentication:Tokens:Audience"];
    var signingKey = builder.Configuration["Authentication:Tokens:SigningKey"];

    //configuration.RequireHttpsMetadata = false;
    configuration.SaveToken = true;
    configuration.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
        ValidateLifetime = true,
        ValidateAudience = true,
        ValidateIssuer = true
    };
});
builder.Services.AddAuthorization();

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(configuration =>
{
    #region Provide the extended endpoint description from the xml comments

    //var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    //var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    //configuration.IncludeXmlComments(xmlPath);

    foreach (var filePath in Directory.GetFiles(AppContext.BaseDirectory, "*.xml", SearchOption.TopDirectoryOnly))
    {
        configuration.IncludeXmlComments(filePath);
    }

    #endregion

    configuration.SwaggerDoc("authentication", new OpenApiInfo
    {
        Title = "Authentication Documentation",
        Description = "Authentication",
        Contact = null,
        Version = "v1"
    });

    configuration.SwaggerDoc("usermanagement", new OpenApiInfo
    {
        Title = "UserManagement Documentation",
        Description = "UserManagement",
        Contact = null,
        Version = "v1"
    });

    configuration.SwaggerDoc("useraccount", new OpenApiInfo
    {
        Title = "UserAccount Documentation",
        Description = "UserAccount",
        Contact = null,
        Version = "v1"
    });
});

var app = builder.Build();

var migrationHelper = app.Services.GetService<MigrationHelper>();
if (migrationHelper != null)
{
    if (!await migrationHelper.UpdateDatabaseAsync())
    {
        return;
    }
}

using (var serviceScope = app.Services.CreateScope())
{
    var services = serviceScope.ServiceProvider;

    var userManagementService = services.GetRequiredService<IUserManagementService>();
    await InitialUserHelper.CreateUsersAsync(users, userManagementService);
}

app.UseForwardedHeaders();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(configuration =>
    {
        configuration.PreSerializeFilters.Add((swagger, httpReq) =>
        {
            if (httpReq.Headers.ContainsKey("X-Forwarded-Host"))
            {
                //The httpReq.PathBase and httpReq.Headers["X-Forwarded-Prefix"] is what we need to get the base path.
                //For some reason, they returning as null/blank. Perhaps this has something to do with how the proxy is configured which we don't have control.
                //For the time being, the base path is manually set here that corresponds to the APIM API Url Prefix.
                //In this case we set it to 'sample-app'. 

               var basePath = "sample-app";
               var serverUrl = $"{httpReq.Scheme}://{httpReq.Headers["X-Forwarded-Host"]}/{basePath}";
               swagger.Servers = new List<OpenApiServer> { new OpenApiServer { Url = serverUrl } };
            }
        });
    });

    app.UseSwaggerUI(configuration =>
    {
        configuration.EnableTryItOutByDefault();
        configuration.DisplayRequestDuration();
        configuration.SwaggerEndpoint("authentication/swagger.json", "Authentication");
        configuration.SwaggerEndpoint("usermanagement/swagger.json", "UserManagement");
        configuration.SwaggerEndpoint("useraccount/swagger.json", "UserAccount");
    });
}

app.UseRouting();

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapFallbackToFile("index.html");
});

app.Run();
