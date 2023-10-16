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

var builder = WebApplication.CreateBuilder(args);

//Mongodb setup
builder.Services.Configure<MongoDBSettings>(builder.Configuration.GetSection(nameof(MongoDB)));
builder.Services.AddSingleton<IMongoDBSettings>(sp =>
    sp.GetRequiredService<IOptions<MongoDBSettings>>().Value);

builder.Services.AddSingleton<IMongoClient>(s
    => new MongoClient(builder.Configuration.GetValue<string>("MongoDB:ConnectionURL")));

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

var key = Encoding.UTF8.GetBytes(builder.Configuration["JwtConfig:Secret"]!);

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
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
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
var emailConfig = builder.Configuration
        .GetSection("EmailConfiguration")
        .Get<EmailConfiguration>();
builder.Services.AddSingleton(emailConfig);


var app = builder.Build();


app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
           Path.Combine(builder.Environment.ContentRootPath, "Uploads")),
    RequestPath = "/Resources"
});

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
