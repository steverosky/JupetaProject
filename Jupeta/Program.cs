global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.EntityFrameworkCore;
global using System.Text;
using Newtonsoft.Json;
using Amazon.S3;
using Jupeta.Models.DBModels;
using Jupeta.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using Serilog;
using Swashbuckle.AspNetCore.Filters;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);


DotNetEnv.Env.Load();

var JwtAudience = DotNetEnv.Env.GetString("JwtConfig__Audience");
var dbString = DotNetEnv.Env.GetString("MongoDB__ConnectionURL");
var JwtIssuer = DotNetEnv.Env.GetString("JwtConfig__Issuer");
var secretKey = DotNetEnv.Env.GetString("JwtConfig__Secret")!;


if (string.IsNullOrEmpty(dbString) || string.IsNullOrEmpty(secretKey) || string.IsNullOrEmpty(JwtIssuer) || string.IsNullOrEmpty(JwtAudience))
{
    Console.WriteLine("Provide all values for environment variables");
    return;
}

//Mongodb setup
var mongoDBSettings = new MongoDBSettings
{
    ConnectionURI = Environment.GetEnvironmentVariable("MongoDB__ConnectionURL")!,
    DatabaseName = Environment.GetEnvironmentVariable("MongoDB__DatabaseName")!
};

builder.Services.AddSingleton<IMongoDBSettings>(sp => mongoDBSettings);


builder.Services.AddSingleton<IMongoClient>(s
    => new MongoClient(dbString));

//add CORS policy
builder.Services.AddCors(options =>
{
    //options.AddPolicy("AllowAll", p =>
    //{
    //    p.AllowAnyOrigin() // Allow requests from any origin
    //     .AllowAnyMethod()
    //     .AllowAnyHeader();
    //     .AllowCredentials(); // Allow cross-origin requests with credentials (if needed)
    //});

    options.AddPolicy("AllowJupeta", p =>
    {
        p.WithOrigins("https://jupeta.vercel.app", "http://127.0.0.1:3000", "http://127.0.0.1:5500") // Allow requests only from specific origin
         .AllowAnyMethod()
         .AllowAnyHeader()
         .AllowCredentials(); // Allow cross-origin requests with credentials (if needed)
    });
});




builder.Services.AddHttpClient();

var key = Encoding.UTF8.GetBytes(secretKey!);

//var RefreshTokenValidationParameter = new TokenValidationParameters
//{
//    ValidateIssuerSigningKey = true,
//    IssuerSigningKey = new SymmetricSecurityKey(key),
//    ValidateIssuer = true,
//    ValidateAudience = true,
//    ValidAudience = builder.Configuration["JwtConfig:Audience"],
//    ValidIssuer = builder.Configuration["JwtConfig:Issuer"],
//    ValidateLifetime = true,
//    RequireExpirationTime = false,
//    ClockSkew = TimeSpan.Zero

//};

var tokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(key),
    ValidateIssuer = true,
    ValidateAudience = true,
    ValidAudience = builder.Configuration["JwtConfig:Audience"],
    ValidIssuer = builder.Configuration["JwtConfig:Issuer"],
    ValidateLifetime = true,
    RequireExpirationTime= true,
    ClockSkew = TimeSpan.Zero

};

//add jwt authentication services to program
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options =>
    {
        options.Cookie.Name = "AccessToken";
    })
.AddJwtBearer(jwt =>
{

    jwt.SaveToken = true;
    jwt.RequireHttpsMetadata = false;
    jwt.TokenValidationParameters = tokenValidationParameters;

    jwt.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            context.Token = context.Request.Cookies["AccessToken"];
            return Task.CompletedTask;
        }
    };

})
.AddGoogle(options =>
{
    options.ClientId = Environment.GetEnvironmentVariable("GoogleSigninConfig__ClientId")!;
    options.ClientSecret = Environment.GetEnvironmentVariable("GoogleSigninConfig__ClientSecret")!;
})
.AddFacebook(options =>
{
    options.AppId = Environment.GetEnvironmentVariable("FacebookSigninConfig__AppId")!;
    options.AppSecret = Environment.GetEnvironmentVariable("FacebookSigninConfig__AppSecret")!;
});

// Add services to the container.
builder.Services.AddSingleton(tokenValidationParameters);
//builder.Services.AddSingleton(RefreshTokenValidationParameter);
builder.Services.AddScoped<IMongoDBservices, MongoDBservices>();
builder.Services.AddTransient<IFileService, FileService>();
builder.Services.AddSingleton<IAmazonS3, AmazonS3Client>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ICacheService, CacheService>();


builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());

// setup serilog
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddHttpContextAccessor();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.Name = "Jupeta.Session";
    options.IdleTimeout = TimeSpan.FromSeconds(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

//add email service
var emailConfig = new EmailConfiguration
{
    From = Environment.GetEnvironmentVariable("EmailConfiguration__From")!,
    DisplayName = Environment.GetEnvironmentVariable("EmailConfiguration__DisplayName")!,
    Password = Environment.GetEnvironmentVariable("EmailConfiguration__Password")!
};

builder.Services.AddSingleton(emailConfig);


var app = builder.Build();


app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseSerilogRequestLogging();
app.UseCors("AllowJupeta");

app.UseAuthorization();
app.UseAuthentication();
app.UseSession();



app.MapControllers();

app.Run();
