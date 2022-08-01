FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal AS base
LABEL org.opencontainers.image.source https://github.com/JPPGroup/Services
COPY Projects/openssl.cnf /etc/ssl/openssl.cnf
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0-focal AS build
WORKDIR /src
COPY ["Projects/Projects.csproj", "Projects/"]
RUN dotnet restore "Projects/Projects.csproj"
COPY . .
WORKDIR "/src/Projects"
RUN dotnet build "Projects.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Projects.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

#Force support for older TLS versions
COPY Projects/openssl.cnf /etc/ssl/openssl.cnf

ENTRYPOINT ["dotnet", "Projects.dll"]