#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
ARG GITHUBSECRET
WORKDIR /app
EXPOSE 80
#EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Mapping/Mapping.csproj", "Mapping/"]
COPY ["SharedBlazor/SharedBlazor.csproj", "SharedBlazor/"]
RUN dotnet nuget add source https://nuget.pkg.github.com/jppgroup/index.json --name "github" --username jppgroup --store-password-in-clear-text --password $GITHUBSECRET
RUN dotnet restore "Mapping/Mapping.csproj"
COPY . .
WORKDIR "/src/Mapping"
RUN dotnet build "Mapping.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Mapping.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Mapping.dll"]