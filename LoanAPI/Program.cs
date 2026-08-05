using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using LoanAPI.Data;
using LoanAPI.Repositories;
using LoanAPI.Services;
using LoanAPI.Helper;
using LoanAPI.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<CustomerRepository>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<LoanRepository>();
builder.Services.AddScoped<LoanScheduleRepository>();
builder.Services.AddScoped<PaymentRepository>();
builder.Services.AddScoped<DocumentRepository>();

builder.Services.AddScoped<CustomerService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<LoanService>();
builder.Services.AddScoped<LoanScheduleService>();
builder.Services.AddScoped<PaymentService>();
builder.Services.AddScoped<DocumentService>();
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<AccessControlService>();

var jwtKey = builder.Configuration["Jwt:Key"]!;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };

        //enforces "one active session per account"
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                var userIdClaim = context.Principal?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var sessionIdClaim = context.Principal?.FindFirst("SessionId")?.Value;

                if (string.IsNullOrEmpty(userIdClaim) || string.IsNullOrEmpty(sessionIdClaim))
                {
                    context.Fail("Invalid token.");
                    return;
                }

                var userRepo = context.HttpContext.RequestServices.GetRequiredService<UserRepository>();
                var user = await userRepo.GetUserById(int.Parse(userIdClaim));

                if (user == null || user.SessionId != sessionIdClaim)
                {
                    context.Fail("This account was logged in from another browser or device.Logout from other device to continue.");
                }
            }
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddControllers();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage)
            .ToList();

        return new BadRequestObjectResult(new
        {
            Message = errors.First()
        });
    };
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "LoanAPI", Version = "v1" });

    options.AddSecurityDefinition("Bearer", new()
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Paste the token you get back from /api/User/login here (no 'Bearer ' prefix needed)."
    });
    options.AddSecurityRequirement(new()
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
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();

    var userRepo = scope.ServiceProvider.GetRequiredService<UserRepository>();
    if (await userRepo.GetUserByUsername("admin1") == null)
    {
        await userRepo.AddUser(new User
        {
            Username = "Admin1",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin@1"),
            Role = "Admin",
            CustomerId = null,
        });
    }

    if (await userRepo.GetUserByUsername("admin2") == null)
    {
        await userRepo.AddUser(new User
        {
            Username = "Admin2",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin@2"),
            Role = "Admin",
            CustomerId = null,
        });
    }
    if (await userRepo.GetUserByUsername("admin3") == null)
    {
        await userRepo.AddUser(new User
        {
            Username = "Admin3",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin@3"),
            Role = "Admin",
            CustomerId = null,
        });
    }
}

app.Run();