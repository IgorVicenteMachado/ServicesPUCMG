using ms_user.Data;
using ms_user.Services;
using ms_user.Interfaces;

namespace ms_user.Extensions
{
    public static class AppExtensions
    {
        public static void ConfigureServices(this WebApplicationBuilder builder)
        {
            //builder.Services.AddScoped<TokenService>();
            //builder.Services.AddTransient<EmailService>();
            builder.Services.AddScoped<IUserPersist, UserPersist>();
            builder.Services.AddScoped<IAccountService, AccountService>();
            builder.Services.AddScoped<ITokenService, TokenService>();
        }

        //public static void LoadConfiguration(this WebApplicationBuilder builder)
        //{
        //    Configuration.JwtKey = builder.Configuration.GetValue<string>("JwtKey");
        //    Configuration.SqlConnection = builder.Configuration.GetConnectionString("SqlConnection");
        //    Configuration.SENDGRID_API_KEY = builder.Configuration.GetValue<string>("SENDGRID_API_KEY");
        //}

        //public static void ConfigureAuthentication(this WebApplicationBuilder builder)
        //{
        //    var key = Encoding.ASCII.GetBytes(Configuration.JwtKey);
        //    builder.Services.AddAuthentication(x =>
        //    {
        //        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        //        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        //    }).AddJwtBearer(x =>
        //    {
        //        x.TokenValidationParameters = new TokenValidationParameters
        //        {
        //            ValidateIssuerSigningKey = true,
        //            IssuerSigningKey = new SymmetricSecurityKey(key),
        //            ValidateIssuer = false,
        //            ValidateAudience = false
        //        };
        //    });
        //}
        //public static void ConfigureMvc(this WebApplicationBuilder builder)
        //{
        //    builder.Services.AddMemoryCache();
        //    builder.Services.AddControllers().ConfigureApiBehaviorOptions(opts => opts.SuppressModelStateInvalidFilter = true)
        //        .AddJsonOptions(x =>
        //        {
        //            x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        //            x.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
        //        });

        //    #region compressão de arquivos para redução no tráfego de rede
        //    builder.Services.AddResponseCompression(options =>
        //    {
        //            // options.Providers.Add<BrotliCompressionProvider>();
        //        options.Providers.Add<GzipCompressionProvider>();
        //            // options.Providers.Add<CustomCompressionProvider>();
        //    });
        //    builder.Services.Configure<GzipCompressionProviderOptions>(options =>
        //    {
        //        options.Level = CompressionLevel.Optimal;
        //    });
        //    #endregion
        //}


    }

}
