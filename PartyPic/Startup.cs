using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Serialization;
using PartyPic.Contracts.BannedProfiles;
using PartyPic.Contracts.Categories;
using PartyPic.Contracts.Events;
using PartyPic.Contracts.Images;
using PartyPic.Contracts.Logger;
using PartyPic.Contracts.Roles;
using PartyPic.Contracts.Users;
using PartyPic.Contracts.Venues;
using PartyPic.Helpers;
using System;

namespace PartyPic
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<UserContext>(opt => opt.UseSqlServer
                (_configuration.GetConnectionString("PartyPicConnection")));

            services.AddDbContext<ImagesContext>(opt => opt.UseSqlServer
                (_configuration.GetConnectionString("PartyPicConnection")));

            services.AddDbContext<EventContext>(opt => opt.UseSqlServer
                (_configuration.GetConnectionString("PartyPicConnection")));

            services.AddDbContext<VenueContext>(opt => opt.UseSqlServer
                (_configuration.GetConnectionString("PartyPicConnection")));

            services.AddDbContext<CategoryContext>(opt => opt.UseSqlServer
                (_configuration.GetConnectionString("PartyPicConnection")));

            services.AddDbContext<RoleContext>(opt => opt.UseSqlServer
                (_configuration.GetConnectionString("PartyPicConnection")));

            services.AddDbContext<BannedProfileContext>(opt => opt.UseSqlServer
                (_configuration.GetConnectionString("PartyPicConnection")));

            services.AddControllers().AddNewtonsoftJson(s => {
                s.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            });

            services.AddSingleton<ILoggerManager, LoggerManager>();

            services.AddHttpContextAccessor();

            //services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            //services.AddSwaggerGen(c => {
            //    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Commander API", Version = "v1" });
            //});

            services.AddScoped<IUserRepository, SqlUserRepository>();
            services.AddScoped<IImagesRepository, SqlImagesRepository>();
            services.AddScoped<IEventRepository, SqlEventRepository>();
            services.AddScoped<IVenueRepository, SqlVenueRepository>();
            services.AddScoped<ICategoryRespository, SqlCategoryRepository>();
            services.AddScoped<IRoleRepository, SqlRoleRepository>();
            services.AddScoped<IBannedProfileRepository, SqlBannedProfileRepository>();

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddHealthChecks();

            services.AddCors(options =>
            {
                options.AddPolicy("CorsApi",
                    builder => builder.WithOrigins("http://local-web.partypic.com")
                .AllowAnyHeader()
                .AllowAnyMethod());
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseCors("CorsApi");

            app.UseMiddleware<JwtMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });

                endpoints.MapControllers();
            });
        }
    }
}
