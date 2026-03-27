FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["Library.Api/Library.Api.csproj", "Library.Api/"]
COPY ["Library.Application/Library.Application.csproj", "Library.Application/"]
COPY ["Library.Domain/Library.Domain.csproj", "Library.Domain/"]
COPY ["Library.Infrastructure/Library.Infrastructure.csproj", "Library.Infrastructure/"]
COPY ["Shared/Contracts/Contracts.csproj", "Shared/Contracts/"]
COPY ["Shared/Messaging/Contracts.Messaging.csproj", "Shared/Messaging/"]
COPY ["Library.ServiceDefaults/Library.ServiceDefaults.csproj", "Library.ServiceDefaults/"]
RUN dotnet restore "Library.Api/Library.Api.csproj"

COPY Library.Api/ Library.Api/
COPY Library.Application/ Library.Application/
COPY Library.Domain/ Library.Domain/
COPY Library.Infrastructure/ Library.Infrastructure/
COPY Shared/Contracts/ Shared/Contracts/
COPY Shared/Messaging/ Shared/Messaging/
COPY Library.ServiceDefaults/ Library.ServiceDefaults/

WORKDIR "/src/Library.Api"
RUN dotnet publish "Library.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "Library.Api.dll"]
