using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Server.Domain;
using Server.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FluentValidation.AspNetCore;
using Server.Infrastructure.Errors;
using Server.Infrastructure.Filters;
using FluentValidation;
using Server.Hubs;
using Server.Features.Chats;
using System.Net.Mail;
using System.Net;

namespace Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddDbContext<ApplicationContext>(options => 
            //    options.UseSqlServer(Configuration.GetConnectionString("Default")));
            var defaultConnectionString = string.Empty;

            
            // Use connection string provided at runtime by Heroku.
            var connectionUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

            connectionUrl = connectionUrl.Replace("postgres://", string.Empty);
            var userPassSide = connectionUrl.Split("@")[0];
            var hostSide = connectionUrl.Split("@")[1];

            var user = userPassSide.Split(":")[0];
            var pwd = userPassSide.Split(":")[1];
            var host = hostSide.Split("/")[0];
            var database = hostSide.Split("/")[1].Split("?")[0];

            defaultConnectionString = $"Host={host};Database={database};Username={user};Password={pwd};SSL Mode=Require;Trust Server Certificate=true";
            
            
            services.AddDbContext<ApplicationContext>(options =>
               options.UseNpgsql(defaultConnectionString));

            var serviceProvider = services.BuildServiceProvider();

            try
            {
                var dbContext = serviceProvider.GetRequiredService<ApplicationContext>();
                dbContext.Database.Migrate();
            }
            catch
            {
            }


            services.AddMediatR(Assembly.GetExecutingAssembly());

            var issuer = "Issuer";
            var audience = "Audience";
            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("mysecretkey12345qwerteyurfgdfghfdfdsg"));
            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var jwtOptions = new JwtOptions()
            {
                Audience = audience,
                Issuer = issuer,
                SigningCredentials = signingCredentials
            };

            services.AddSingleton(jwtOptions);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = signingCredentials.Key,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                };
            });

            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IPasswordHasher<Person>, PasswordHasher<Person>>();
            services.AddScoped<CurrentUser>();
            services.AddScoped<ChatService>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSignalR();

            services.AddControllers(opt =>
            {
                opt.Filters.Add<ValidatorActionFilter>();
            })
            .AddFluentValidation(cfg =>
            {
                cfg.RegisterValidatorsFromAssemblyContaining<Startup>();
            }
            );

            ValidatorOptions.Global.LanguageManager.Enabled = false; // 

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            services.AddAutoMapper(typeof(Startup));

            services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please insert JWT with Bearer into field",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    BearerFormat = "JWT"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {   new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                    },
                    Array.Empty<string>()}
                });

                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Server", Version = "v1" });
                c.CustomSchemaIds(d => d.FullName);

            });

            string sender = Configuration.GetSection("Gmail")["Sender"];
            string from = Configuration.GetSection("Mail")["From"];
            string password = Configuration.GetSection("Gmail")["Password"];
            int port = Convert.ToInt32(Configuration.GetSection("Gmail")["Port"]);

            services
                .AddFluentEmail(sender, from)
                .AddMailGunSender("sandbox10a2eeb384574d7bb06e406628ff2b03.mailgun.org", "1e1d6f96aafb3280a6e0762197ef1b70-cac494aa-91241d22");

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Server"));
            

            app.UseMiddleware<ErrorHandlingMiddleware>();

            app.UseAuthentication();

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<ChatHub>("/chatHub");
            });
        }
    }
}
