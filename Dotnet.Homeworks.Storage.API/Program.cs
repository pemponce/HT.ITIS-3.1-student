using Dotnet.Homeworks.Storage.API.Configuration;
using Dotnet.Homeworks.Storage.API.Endpoints;
using Dotnet.Homeworks.Storage.API.Services;
using Dotnet.Homeworks.Storage.API.ServicesExtensions;

var builder = WebApplication.CreateBuilder(args);

var minioConfig = builder.Configuration.GetSection(nameof(MinioConfig)).Get<MinioConfig>()
                  ?? throw new InvalidOperationException("MinioConfig section is missing");
builder.Services.Configure<MinioConfig>(builder.Configuration.GetSection(nameof(MinioConfig)));
builder.Services.AddMinioClient(minioConfig);
builder.Services.AddSingleton<IStorageFactory, StorageFactory>();
builder.Services.AddHostedService<PendingObjectsProcessor>();

var app = builder.Build();

app.MapProductsEndpoints();

app.Run();