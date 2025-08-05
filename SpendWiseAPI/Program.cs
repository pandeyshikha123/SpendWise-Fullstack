using CloudinaryDotNet;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using SpendWiseAPI.Config;
using SpendWiseAPI.Helpers;
using SpendWiseAPI.Mappings;
using SpendWiseAPI.Repositories;
using SpendWiseAPI.Repositories.Interfaces;
using SpendWiseAPI.Services;
using SpendWiseAPI.Services.Interfaces;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Load config
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:4200","https://spend-wise-fullstack.vercel.app") 
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Controllers and Swagger
builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "SpendWiseAPI", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter 'Bearer' followed by your JWT token"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

// MongoDB config
builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDbSettings"));
builder.Services.AddSingleton<IMongoClient>(sp =>
    new MongoClient(builder.Configuration["MongoDbSettings:ConnectionString"]));
builder.Services.AddSingleton<IMongoDatabase>(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    var dbName = builder.Configuration["MongoDbSettings:DatabaseName"];
    return client.GetDatabase(dbName);
});

// JWT Auth
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
var jwtKey = Encoding.ASCII.GetBytes(jwtSettings.Secret);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(jwtKey),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

// Email settings
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<IEmailService, EmailService>();

// Cloudinary setup
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));
var cloudinaryConfig = builder.Configuration.GetSection("CloudinarySettings").Get<CloudinarySettings>();
var account = new Account(cloudinaryConfig.CloudName, cloudinaryConfig.ApiKey, cloudinaryConfig.ApiSecret);
var cloudinary = new Cloudinary(account);
builder.Services.AddSingleton(cloudinary);

// MongoDB context registration
builder.Services.AddSingleton<MongoDbContext>();


// Dependency Injection
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<IExpenseService, ExpenseService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

// AutoMapper
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

Console.WriteLine("SpendWise API is running...");
Console.WriteLine("🔗 Swagger UI available at /swagger");

app.Run();
