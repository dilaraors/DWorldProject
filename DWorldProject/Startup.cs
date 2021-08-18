using System;
using System.Reflection;
using System.Text;
using Amazon.DynamoDBv2;
using Amazon.S3;
using AutoMapper;
using DWorldProject.Entities;
using DWorldProject.Models.IyziPay;
using DWorldProject.Repositories;
using DWorldProject.Repositories.Abstract;
using DWorldProject.Services;
using DWorldProject.Services.Abstact;
using DWorldProject.Utils;
using DWorldProject.Utils.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Sinks.Elasticsearch;

namespace DWorldProject
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Elasticsearch(ConfigureElasticSink(Configuration, environment))
                .Enrich.WithProperty("Environment", environment)
                .ReadFrom.Configuration(Configuration)
                .CreateLogger();

            services.AddControllers();
            services.AddAutoMapper(typeof(Startup));
            var origins = new string[] {
                "http://localhost:50400",
                "https://localhost:44325",
                "http://localhost:4200",
                "http://localhost:5000",
             };

            services.AddAuthentication(options => {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options => {
                    options.SaveToken = true;
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidIssuer = "DWorld",
                        ValidAudience = "DWorld",
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetValue<string>("TokenSecretKey")))
                    };
                });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Api", Version = "v1" });
            });

            services.AddCors(option =>
            {
                option.AddPolicy("EnableCors", builder =>
                {
                    builder.WithOrigins(origins).AllowAnyHeader().AllowAnyMethod().AllowCredentials().Build();
                });

            });
            services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));
            //services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();

            var applicationSettings = Configuration.GetSection("IyziPayOptions");
            services.Configure<AppSettings>(applicationSettings);

            var appSettingsIyzipayOptions = applicationSettings.Get<AppSettings>();

            //DynamoDB Connection
            services.AddAWSService<IAmazonDynamoDB>(Configuration.GetAWSOptions("Dynamodb"));
            services.AddDefaultAWSOptions(Configuration.GetAWSOptions());
            services.AddAWSService<IAmazonDynamoDB>();
            services.AddAWSService<IAmazonS3>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IAmazonService, AmazonService>();
            services.AddScoped<ICheckoutFormSample, CheckoutFormSample>();
            services.AddScoped<IBlogPostService, BlogPostService>();
            services.AddScoped<IBlogPostRepository, BlogPostRepository>();
            services.AddScoped<IUserBlogPostRepository, UserBlogPostRepository>();
            services.AddScoped<IUserBlogPostService, UserBlogPostService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<ITopicRepository, TopicRepository>();
            services.AddScoped<ITopicService, TopicService>();
            services.AddScoped<ISectionRepository, SectionRepository>();
            services.AddScoped<ISectionService, SectionService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            loggerFactory.AddSerilog();

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
            });

            app.UseMiddleware<ExceptionMiddleware>();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("EnableCors");
            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseCors("EnableCors");
        }

        private static ElasticsearchSinkOptions ConfigureElasticSink(IConfiguration configuration, string environment)
        {
            return new ElasticsearchSinkOptions(new Uri(configuration["Elasticsearch:URL"]))
            {
                AutoRegisterTemplate = true,
                IndexFormat =
                    $"{Assembly.GetExecutingAssembly().GetName().Name.ToLower().Replace(".", "-")}-{environment?.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM}"
            };
        }
    }
}
