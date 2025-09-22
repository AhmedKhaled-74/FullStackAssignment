using Asp.Versioning;
using FullStackAssignment.Application.IReposetories;
using FullStackAssignment.Application.IServices;
using FullStackAssignment.Application.Services;
using FullStackAssignment.Infrastructure.DbContexts;
using FullStackAssignment.Infrastructure.Reposetories;
using FullStackAssignment.Presentation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;

namespace FullStackAssignment.Bootstrapper.StartUpConfig
{
    public static class AppStartUpExtentionConfig
    {
        public static IServiceCollection ConfigureServices(this IServiceCollection services, WebApplicationBuilder? builder)
        {
            ArgumentNullException.ThrowIfNull(builder);
            // Define a CORS policy
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAngularClient", policy =>
                {
                    policy.WithOrigins(builder.Configuration.GetSection("AllowOrigins").Get<string[]>()!)
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            // Add services to the container.
            services.AddControllers(opt =>
                {
                    var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                    opt.Filters.Add(new AuthorizeFilter(policy));
                })
                .AddApplicationPart(typeof(PresentationAssemply).Assembly);

            if (!builder.Environment.IsEnvironment("Test"))
            {
                builder.Services.AddSingleton<IFileRepo>(new FileRepo(builder.Environment.WebRootPath));

                services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
            }

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IJwtServiceReposetory, JwtServiceRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICategoryService, CategoryService>();

            // API Versioning
            services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            })
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            // Swagger
            var xmlPath = Path.Combine(Directory.GetCurrentDirectory(),
                builder.Configuration.GetSection("ApiDocumentation").GetValue<string>("XmlFilePath") ?? "");

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                if (File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
                }

                c.SwaggerDoc("v1", new OpenApiInfo()
                {
                    Title = "FullStack Assignment API",
                    Version = "v1"
                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme."
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
                new OpenApiSecurityScheme
                {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
                },
            Array.Empty<string>()
        }
                    });
            });

            // JWT
            services.AddAuthentication(opts =>
            {
                opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(opts =>
            {
                opts.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
                    ),
                    RoleClaimType = ClaimTypes.Role
                };
            });

            services.AddAuthorization();

            return services;
        }
    }
}
