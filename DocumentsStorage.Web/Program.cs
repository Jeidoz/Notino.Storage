using Ardalis.Specification;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using DocumentsStorage.Core;
using DocumentsStorage.Core.Interfaces;
using DocumentsStorage.Infrastructure;
using DocumentsStorage.Infrastructure.Data;
using DocumentsStorage.Web;
using DocumentsStorage.Web.Formatters.Config;
using DocumentsStorage.Web.Services;
using MessagePack;
using MessagePack.AspNetCoreMvcFormatter;
using MessagePack.Resolvers;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

// Here you can change storage for EF Core
// Connection string
string? connectionString = builder.Configuration.GetConnectionString("Sqlite");
// Type of storage (InMemory, SQL Database, file (i.e. SQLite))
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));
// options.UseInMemory()
// options.UseSqlServer()
// ...

builder.Services.AddControllers(options =>
{
    options.RespectBrowserAcceptHeader = true;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Notino Storage API", Version = "v1"});
});

// Here you can add any output formatters
builder.Services.AddMvcCore()
    .AddMvcOptions(options =>
    {
        // JSON Serializer enabled by default
        options.OutputFormatters.Add(new XmlSerializerOutputFormatter());
        options.OutputFormatters.Add(new MessagePackOutputFormatter(ContractlessStandardResolver.Options));
    });
// Should be after adding all formatters
builder.Services.AddOutputFormattersService();

builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    containerBuilder.RegisterModule(new DefaultCoreModule());
    containerBuilder.RegisterModule(new DefaultInfrastructureModule(builder.Environment.EnvironmentName == "Development"));
});
builder.Services.RegisterMapsterConfiguration();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Seed Database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        //                    context.Database.Migrate();
        context.Database.EnsureCreated();
        SeedData.Initialize(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred seeding the DB. {exceptionMessage}", ex.Message);
    }
}

app.Run();