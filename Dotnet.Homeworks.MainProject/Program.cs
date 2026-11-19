using Dotnet.Homeworks.Data.DatabaseContext;
using Dotnet.Homeworks.DataAccess.Repositories;
using Dotnet.Homeworks.Domain.Abstractions.Repositories;
using Dotnet.Homeworks.Features.Behaviors;
using Dotnet.Homeworks.Infrastructure.Validation.PermissionChecker.DependencyInjectionExtensions;
using Dotnet.Homeworks.Infrastructure.UnitOfWork;
using Dotnet.Homeworks.Features.Users.Abstractions;
using Dotnet.Homeworks.MainProject.Configuration;
using Dotnet.Homeworks.MainProject.Services;
using Dotnet.Homeworks.MainProject.ServicesExtensions.Masstransit;
using Dotnet.Homeworks.Mediator;
using Dotnet.Homeworks.Mediator.DependencyInjectionExtensions;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();
builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddHttpContextAccessor();

var featuresAssembly = Dotnet.Homeworks.Features.Helpers.AssemblyReference.Assembly;

// собственный медиатор (вместо MediatR)
builder.Services.AddMediator(featuresAssembly);

// PipelineBehavior'ы для операций UserManagement (порядок задаётся атрибутом [PipelineOrder])
builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(AdminPermissionBehavior<,>));
builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(AdminValidationBehavior<,>));

// валидаторы FluentValidation и проверки разрешений, которые лежат вместе с фичами
builder.Services.AddValidatorsFromAssembly(featuresAssembly);
builder.Services.AddPermissionChecks(featuresAssembly);

var rabbitConfig = builder.Configuration.GetSection("RabbitMqConfig").Get<RabbitMqConfig>()
                   ?? throw new InvalidOperationException(
                       "Секция RabbitMqConfig не найдена (RabbitMqConfig__Hostname, __Username, __Password)");
builder.Services.AddMasstransitRabbitMq(rabbitConfig);

builder.Services.AddSingleton<IRegistrationService, RegistrationService>();
builder.Services.AddSingleton<ICommunicationService, CommunicationService>();
builder.Services.AddScoped<IUserCreatedListener, UserCreatedEmailListener>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => "Hello World!");

app.MapControllers();

app.Run();