using Dotnet.Homeworks.Mailing.API.Configuration;
using Dotnet.Homeworks.Mailing.API.Services;
using Dotnet.Homeworks.Mailing.API.ServicesExtensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<EmailConfig>(builder.Configuration.GetSection("EmailConfig"));

builder.Services.AddScoped<IMailingService, MailingService>();

var rabbitConfig = builder.Configuration.GetSection("RabbitMqConfig").Get<RabbitMqConfig>()
                   ?? throw new InvalidOperationException(
                       "Секция RabbitMqConfig не найдена (RabbitMqConfig__Hostname, __Username, __Password)");
builder.Services.AddMasstransitRabbitMq(rabbitConfig);

var app = builder.Build();

app.Run();