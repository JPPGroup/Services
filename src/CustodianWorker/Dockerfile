#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["CustodianWorker/CustodianWorker.csproj", "CustodianWorker/"]
RUN dotnet restore "CustodianWorker/CustodianWorker.csproj"
COPY . .
WORKDIR "/src/CustodianWorker"
RUN dotnet build "CustodianWorker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CustodianWorker.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CustodianWorker.dll"]