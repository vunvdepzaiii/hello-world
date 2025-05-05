using API.Common;
using API.Services;
using API.Services.Interfaces;
using API.Services.Models;
using API.Services.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using OfficeOpenXml;
using QuestPDF.Infrastructure;
using System.Configuration;
using System.Text;

namespace API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // EPPlus free
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            //var builder = WebApplication.CreateBuilder(args);
            //Setting Custom Web Root Folder
            WebApplicationBuilder builder = WebApplication.CreateBuilder(new WebApplicationOptions
            {
                WebRootPath = "wwwroot"
            });

            var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins,
                                  policy =>
                                  {
                                      policy.WithOrigins("http://localhost:4200", "https://localhost:4200").AllowAnyHeader().AllowAnyMethod();
                                  });
            });

            // Add services to the container.
            builder.Services.AddTransient<IAuthenticationRepository, AuthenticationRepository>();
            builder.Services.AddTransient<IUserRepository, UserRepository>();
            builder.Services.AddTransient<IBuildingRepository, BuildingRepository>();
            builder.Services.AddTransient<IApartmentRepository, ApartmentRepository>();
            builder.Services.AddTransient<IVehicleFeeRepository, VehicleFeeRepository>();
            builder.Services.AddTransient<IWaterBillRepository, WaterBillRepository>();
            builder.Services.AddTransient<IManagementBillRepository, ManagementBillRepository>();
            builder.Services.AddTransient<IVehicleBillRepository, VehicleBillRepository>();
            builder.Services.AddTransient<IOtherBillRepository, OtherBillRepository>();
            builder.Services.AddTransient<IReportRepository, ReportRepository>();

            // controller
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddHttpClient();
            // dbcontext
            builder.Services.AddDbContext<DataContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("SQLServer")));
            // bearer token
            builder.Services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    //jwt bearer
                    var jwtConfig = builder.Configuration.GetSection("Jwt");
                    options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            ValidIssuer = jwtConfig["Issuer"],
                            ValidAudience = jwtConfig["Audience"],
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig["Key"]))
                        };
                });
            // auto mapper
            builder.Services.AddAutoMapper(typeof(MappingProfile));
            // IHttpContextAccessor
            builder.Services.AddHttpContextAccessor();
            // Set license pdf
            QuestPDF.Settings.License = LicenseType.Community;

            var app = builder.Build();
            // IHttpContextAccessor
            ServiceCommon.HttpContextAccessor = app.Services.GetRequiredService<IHttpContextAccessor>();
            // IConfiguration
            ServiceCommon.Configuration = builder.Configuration;

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseMiddleware<ExceptionMiddleware>();

            app.UseCors(MyAllowSpecificOrigins);
            app.UseHttpsRedirection();

            // Add .NET JWT authentication middleware
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.MapControllers();

            app.Run();
        }
    }
}