using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using msuser.Data;
using msuser.Models;
using msuser.Services;
using System.Text;
using System.Text.Json.Serialization;

namespace msuser.Extensions
{
    public static class ProgramExtension
    {
        public static void LoadAppSettingsConfig(this WebApplicationBuilder builder)
        {
            AppSettingsConfig.TokenKey = builder.Configuration.GetValue<string>("TokenKey");
            AppSettingsConfig.SqlConnection = builder.Configuration.GetConnectionString("SqlConnection");
        }

        public static void ConfigureDependencyInjection(this WebApplicationBuilder builder)
        {
            builder.Services.AddDbContext<DataContext>(opt => opt.UseSqlServer(AppSettingsConfig.SqlConnection));
            builder.Services.AddScoped<TokenService>();
            builder.Services.AddMemoryCache();
        }

        public static void ConfigureIdentityCore(this WebApplicationBuilder builder)
        {
            builder.Services.AddIdentityCore<User>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
            })
            .AddRoles<Role>()
            .AddRoleManager<RoleManager<Role>>()
            .AddSignInManager<SignInManager<User>>()
            .AddRoleValidator<RoleValidator<Role>>()
            .AddEntityFrameworkStores<DataContext>()
            .AddDefaultTokenProviders();
        }
        public static void ConfigureAuuthenticationScheme(this WebApplicationBuilder builder)
        {
            builder.Services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AppSettingsConfig.TokenKey)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
        }

        public static void ConfigureMvc(this WebApplicationBuilder builder)
        {
            builder.Services.AddControllers().ConfigureApiBehaviorOptions(opts => opts.SuppressModelStateInvalidFilter = true)
                .AddJsonOptions(x =>
                {
                    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                    x.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
                    x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });
        }

        public static void ConfigureSwaggerGen(this WebApplicationBuilder builder)
        {
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "MsUsers", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = @"Enter 'Bearer' [space] and your token!",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
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
                            In= ParameterLocation.Header
                        },
                        new List<string> ()
                    }
                });
            });
        }
    }
}
