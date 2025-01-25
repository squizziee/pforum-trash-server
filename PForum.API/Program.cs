using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PForum.API.Data;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options =>
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder.WithOrigins("http://localhost:5173")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials()));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, 
        options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["Jwt:AccessToken:Issuer"],
                ValidAudience = builder.Configuration["Jwt:AccessToken:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:AccessToken:Key"]!))
            };

            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    context.Request.Cookies.TryGetValue("accessToken", out var accessToken);
                    if (!string.IsNullOrEmpty(accessToken))
                        context.Token = accessToken;
                    return Task.CompletedTask;
                }
            };

        }
);

builder.Services.AddControllers();

builder.Services.AddDbContext<DatabaseContext>(options => 
    options.UseSqlite(connectionString: builder.Configuration.GetConnectionString("SqliteConnectionString"))
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseCors("AllowSpecificOrigin");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { };