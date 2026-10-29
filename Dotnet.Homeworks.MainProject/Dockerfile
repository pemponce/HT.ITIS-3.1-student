FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY . .
RUN dotnet restore Dotnet.Homeworks.MainProject/Dotnet.Homeworks.MainProject.csproj
RUN dotnet publish Dotnet.Homeworks.MainProject/Dotnet.Homeworks.MainProject.csproj \
    -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 80
ENTRYPOINT ["dotnet", "Dotnet.Homeworks.MainProject.dll"]