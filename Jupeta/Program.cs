using Jupeta.Models.DBModels;
using Jupeta.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using Serilog;
using System.Text;

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


//add jwt authentication services to program
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(jwt =>
{
    var key = Encoding.ASCII.GetBytes(builder.Configuration["JwtConfig:Secret"]!);
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

});

// Add services to the container.
builder.Services.AddScoped<IMongoDBservices, MongoDBservices>();

// setup serilog
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddHttpContextAccessor();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.UseAuthorization();
app.UseAuthentication();


app.MapControllers();

app.Run();
