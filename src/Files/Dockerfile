FROM mcr.microsoft.com/dotnet/core/aspnet:2.2 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build
WORKDIR /src
COPY ["Files/Files.Api.csproj", "Files/"]

RUN dotnet restore "Files/Files.Api.csproj"
COPY . .
WORKDIR "/src/Files"
RUN dotnet build "Files.Api.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Files.Api.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
RUN mkdir /var/tmp/files
ENTRYPOINT ["dotnet", "Jpp.Files.Api.dll"]