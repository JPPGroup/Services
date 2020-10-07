FROM mcr.microsoft.com/dotnet/core/runtime:2.2-bionic AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-bionic AS build
WORKDIR /src
COPY ["MappingReportGenerator/MappingReportGenerator.csproj", "MappingReportGenerator/"]
COPY ["MessageBroker/MessageBroker.csproj", "MessageBroker/"]
COPY ["Common.Mapping/Common.Mapping.csproj", "Common.Mapping/"]

RUN dotnet restore "MappingReportGenerator/MappingReportGenerator.csproj"
COPY . .
WORKDIR "/src/MappingReportGenerator"
RUN dotnet build "MappingReportGenerator.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "MappingReportGenerator.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .

RUN apt-get -qq update && apt-get install -y libfontconfig1
RUN ls

ENTRYPOINT ["dotnet", "Jpp.MappingReportGenerator.dll"]