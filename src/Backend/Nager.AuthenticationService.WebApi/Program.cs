using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Nager.AuthenticationService.Abstraction;
using Nager.AuthenticationService.Abstraction.Models;
using Nager.AuthenticationService.Abstraction.Services;
using Nager.AuthenticationService.MssqlRepository;
using Nager.AuthenticationService.WebApi;
using Nager.AuthenticationService.WebApi.Helpers;
using Nager.AuthenticationService.WebApi.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var initialUser1EmailAddress = builder.Configuration["Authentication:InitialUser:EmailAddress"];
var initialUser1Password = builder.Configuration["Authentication:InitialUser:Password"];

if (string.IsNullOrEmpty(initialUser1EmailAddress) || string.IsNullOrEmpty(initialUser1Password))
{
    throw new Exception("InitialUser config missing");
}

var initialUsers = new UserInfoWithPassword[]
{
    new UserInfoWithPassword
    {
        EmailAddress = initialUser1EmailAddress,
        Password = initialUser1Password,
        Roles = ["administrator"]
    }
};

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
    var signingKey = builder.Configuration["Authentication:Tokens:SigningKey"] ?? throw new NullReferenceException("Missing configuration for SigningKey");

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

    configuration.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme. 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      Example: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    configuration.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            Array.Empty<string>()
        }
    });

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
    await InitialUserHelper.CreateUsersAsync(initialUsers, userManagementService);
}

app.UseForwardedHeaders();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(configuration =>
    {
        configuration.RouteTemplate = "auth/swagger/{documentname}/swagger.json";
        //configuration.PreSerializeFilters.Add((swagger, httpReq) =>
        //{
        //    if (httpReq.Headers.ContainsKey("X-Forwarded-Host"))
        //    {
        //        var proto = httpReq.Headers["X-Forwarded-Proto"];
        //        var host = httpReq.Headers["X-Forwarded-Host"];

        //        var basePath = "auth";
        //        var serverUrl = $"{proto}://{host}/{basePath}";
        //        swagger.Servers = [new OpenApiServer { Url = serverUrl }];
        //    }
        //});
    });

    app.UseSwaggerUI(configuration =>
    {
        configuration.RoutePrefix = "auth/swagger";
        configuration.EnableTryItOutByDefault();
        configuration.DisplayRequestDuration();
        configuration.SwaggerEndpoint("authentication/swagger.json", "Authentication");
        configuration.SwaggerEndpoint("usermanagement/swagger.json", "UserManagement");
        configuration.SwaggerEndpoint("useraccount/swagger.json", "UserAccount");
    });
}

//app.UsePathBase("/auth");

app.UseRouting();

app.UseDefaultFiles(new DefaultFilesOptions
{
    RequestPath = "/auth"
});
app.UseStaticFiles(new StaticFileOptions
{
    RequestPath = "/auth"
});

app.UseAuthentication();
app.UseAuthorization();

app.MapFallbackToFile("index.html");
app.MapControllers();

app.Run();
