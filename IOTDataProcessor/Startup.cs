using Business;
using Business.Interfaces;
using Business.Services;
using Data.Data;
using Data.Entities;
using Data.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace IOTDataProcessor
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../"));
            configurationBuilder.AddJsonFile("Data/appsettings.json", optional: false, reloadOnChange: true);
            var configuration = configurationBuilder.Build();

            services.AddSingleton<IConfiguration>(configuration);

            services.AddDbContext<SensorSystemDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddAutoMapper(typeof(AutomapperProfile));

            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddTransient<ILightSensorTelemetry, LightSensorTelemetryService>();
            services.AddTransient<ILightSensor, LightSensorService>();
            services.AddTransient<ISensorLogger, SensorLoggerService>();
            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IIdentityService, IdentityService>();

            var tokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration["JWT:Secret"])),

                ValidateIssuer = true,
                ValidIssuer = configuration["JWT:Issuer"],

                ValidateAudience = true,
                ValidAudience = configuration["JWT:Audience"],

                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            services.AddSingleton(tokenValidationParameters);

            services.AddIdentity<AppUser, IdentityRole>()
                .AddEntityFrameworkStores<SensorSystemDbContext>()
                .AddDefaultTokenProviders()
                .AddSignInManager<SignInManager<AppUser>>();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.SaveToken = true;
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = tokenValidationParameters;
                });

            services.AddSwaggerGen();
            services.AddControllersWithViews();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment() || env.IsStaging())
            {
                app.UseDeveloperExceptionPage();

                app.UseSwagger();
                app.UseSwaggerUI();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
