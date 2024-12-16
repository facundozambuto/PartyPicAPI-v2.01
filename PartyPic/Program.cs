using Microsoft.AspNetCore.Builder;
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
using PartyPic.Contracts.Payments;
using PartyPic.Contracts.Plans;
using PartyPic.Contracts.Reports;
using PartyPic.Contracts.Roles;
using PartyPic.Contracts.SessionLogs;
using PartyPic.Contracts.Subscriptions;
using PartyPic.Contracts.Users;
using PartyPic.Contracts.Venues;
using PartyPic.Helpers;
using PartyPic.ThirdParty;
using PartyPic.ThirdParty.Impl;
using System;

var builder = WebApplication.CreateBuilder(args);

// Accede a la configuración global
var configuration = builder.Configuration;

// 🔹 Configurar DbContexts (Conexiones a SQL Server)
builder.Services.AddDbContext<UserContext>(opt => opt.UseSqlServer(configuration.GetConnectionString("PartyPicConnection")));
builder.Services.AddDbContext<ImagesContext>(opt => opt.UseSqlServer(configuration.GetConnectionString("PartyPicConnection")));
builder.Services.AddDbContext<EventContext>(opt => opt.UseSqlServer(configuration.GetConnectionString("PartyPicConnection")));
builder.Services.AddDbContext<VenueContext>(opt => opt.UseSqlServer(configuration.GetConnectionString("PartyPicConnection")));
builder.Services.AddDbContext<CategoryContext>(opt => opt.UseSqlServer(configuration.GetConnectionString("PartyPicConnection")));
builder.Services.AddDbContext<RoleContext>(opt => opt.UseSqlServer(configuration.GetConnectionString("PartyPicConnection")));
builder.Services.AddDbContext<BannedProfileContext>(opt => opt.UseSqlServer(configuration.GetConnectionString("PartyPicConnection")));
builder.Services.AddDbContext<SessionLogsContext>(opt => opt.UseSqlServer(configuration.GetConnectionString("PartyPicConnection")));
builder.Services.AddDbContext<SubscriptionContext>(opt => opt.UseSqlServer(configuration.GetConnectionString("PartyPicConnection")));
builder.Services.AddDbContext<PaymentContext>(opt => opt.UseSqlServer(configuration.GetConnectionString("PartyPicConnection")));
builder.Services.AddDbContext<PlanContext>(opt => opt.UseSqlServer(configuration.GetConnectionString("PartyPicConnection")));

// 🔹 Configurar los controladores con soporte para Newtonsoft.Json
builder.Services.AddControllers()
    .AddNewtonsoftJson(s =>
        s.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver()
    );

// 🔹 Configuración de dependencias (Inyección de dependencias)
builder.Services.AddSingleton<ILoggerManager, LoggerManager>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IUserRepository, SqlUserRepository>();
builder.Services.AddScoped<IImagesRepository, SqlImagesRepository>();
builder.Services.AddScoped<IEventRepository, SqlEventRepository>();
builder.Services.AddScoped<IVenueRepository, SqlVenueRepository>();
builder.Services.AddScoped<ICategoryRepository, SqlCategoryRepository>();
builder.Services.AddScoped<IRoleRepository, SqlRoleRepository>();
builder.Services.AddScoped<IBannedProfileRepository, SqlBannedProfileRepository>();
builder.Services.AddScoped<IReportsRepository, SqlReportsRepository>();
builder.Services.AddScoped<ISessionLogsRepository, SqlSessionLogsRepository>();
builder.Services.AddScoped<ISubscriptionRepository, SqlSubscriptionRepository>();
builder.Services.AddScoped<IPaymentRepository, SqlPaymentRepository>();
builder.Services.AddScoped<IPlanRepository, SqlPlanRepository>();
builder.Services.AddScoped<IBlobStorageManager, BlobStorageManager>();
builder.Services.AddScoped<IPaymentGatewayStrategy, MercadoPagoManager>();
builder.Services.AddScoped<PaymentGatewayFactory>();
builder.Services.AddScoped<ICurrencyConverter, CurrencyConverterManager>();
builder.Services.AddScoped<IEmailSender, EmailSenderManager>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// 🔹 Configurar Health Checks
builder.Services.AddHealthChecks();

// 🔹 Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsApi", builder =>
    {
        builder.WithOrigins("http://local-web.partypic.com")
            .WithOrigins("http://www.partypic.fun")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// 🔹 Crear la aplicación (similar a la función Configure en Startup.cs)
var app = builder.Build();

// 🔹 Configurar el entorno de la aplicación
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// 🔹 Usar Middleware (JWT, CORS, Rutas, etc.)
app.UseRouting();

app.UseCors("CorsApi");

app.UseMiddleware<JwtMiddleware>();

// 🔹 Configurar los Endpoints (similar a Configure en Startup.cs)
app.MapGet("/", async context =>
{
    await context.Response.WriteAsync("Hello World!");
});

app.MapControllers();

// 🔹 Ejecutar la aplicación
app.Run();
