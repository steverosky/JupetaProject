global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.EntityFrameworkCore;
global using Newtonsoft.Json;
global using System.Text;
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
    options.AddPolicy("AllowAll", p => p.AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader()));

builder.Services.AddHttpClient();

//add jwt authentication services to program
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddCookie(options =>
    {
        options.Cookie.Name = "token";
    })
.AddJwtBearer(jwt =>
{
    var key = Encoding.UTF8.GetBytes(builder.Configuration["JwtConfig:Secret"]!);
    jwt.SaveToken = true;
    jwt.RequireHttpsMetadata = false;
    jwt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JwtConfig:Audience"],
        ValidIssuer = builder.Configuration["JwtConfig:Issuer"],
        ValidateLifetime = true,

    };

    jwt.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            context.Token = context.Request.Cookies["token"];
            return Task.CompletedTask;
        }
    };

});

// Add services to the container.
builder.Services.AddScoped<IMongoDBservices, MongoDBservices>();
builder.Services.AddTransient<IFileService, FileService>();
builder.Services.AddSingleton<IAmazonS3, AmazonS3Client>();

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
    //options.Cookie.Name = "Jupeta.Session";
    options.IdleTimeout = TimeSpan.FromSeconds(2);
    //options.Cookie.HttpOnly = true;
    //options.Cookie.IsEssential = true;
});

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
app.UseCors("AllowAll");

app.UseAuthorization();
app.UseAuthentication();
app.UseSession();



app.MapControllers();

app.Run();
