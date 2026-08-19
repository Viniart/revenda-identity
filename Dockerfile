FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

COPY Directory.Build.props Directory.Packages.props ./
COPY src/Revenda.Identity.Domain/*.csproj src/Revenda.Identity.Domain/
COPY src/Revenda.Identity.Application/*.csproj src/Revenda.Identity.Application/
COPY src/Revenda.Identity.Infrastructure/*.csproj src/Revenda.Identity.Infrastructure/
COPY src/Revenda.Identity.Api/*.csproj src/Revenda.Identity.Api/
RUN dotnet restore src/Revenda.Identity.Api/Revenda.Identity.Api.csproj

COPY src/ src/
RUN dotnet publish src/Revenda.Identity.Api/Revenda.Identity.Api.csproj \
    --configuration Release \
    --no-restore \
    --output /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

RUN adduser --disabled-password --gecos "" --uid 5678 revenda
USER revenda

COPY --from=build /app .

ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Revenda.Identity.Api.dll"]
