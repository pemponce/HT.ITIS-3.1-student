FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY . .
RUN dotnet restore Dotnet.Homeworks.Mailing.API/Dotnet.Homeworks.Mailing.API.csproj
RUN dotnet publish Dotnet.Homeworks.Mailing.API/Dotnet.Homeworks.Mailing.API.csproj \
    -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Dotnet.Homeworks.Mailing.API.dll"]
