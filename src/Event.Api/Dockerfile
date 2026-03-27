FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["Event.Api/EventService.csproj", "Event.Api/"]
COPY ["Shared/Contracts/Contracts.csproj", "Shared/Contracts/"]
COPY ["Shared/Messaging/Contracts.Messaging.csproj", "Shared/Vimachem.Messaging/"]
COPY ["Library.ServiceDefaults/Library.ServiceDefaults.csproj", "Library.ServiceDefaults/"]
RUN dotnet restore "Event.Api/EventService.csproj"

COPY Event.Api/ Event.Api/
COPY Shared/Contracts/ Shared/Contracts/
COPY Shared/Messaging/ Shared/Messaging/
COPY Library.ServiceDefaults/ Library.ServiceDefaults/

WORKDIR "/src/Event.Api"
RUN dotnet publish "EventService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "EventService.dll"]
