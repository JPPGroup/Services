#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["LicenseScanner/LicenseScanner.csproj", "LicenseScanner/"]
RUN dotnet restore "LicenseScanner/LicenseScanner.csproj"
COPY . .
WORKDIR "/src/LicenseScanner"
RUN dotnet build "LicenseScanner.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LicenseScanner.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LicenseScanner.dll"]