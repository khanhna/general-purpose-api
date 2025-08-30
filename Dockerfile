#See https://docs.microsoft.com/en-us/dotnet/core/docker/build-container?tabs=windows

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /src

# Restore as distinct layers
COPY ["src/GeneralPurpose.Api/GeneralPurpose.Api.csproj", "src/GeneralPurpose.Api/"]
COPY ["src/GeneralPurpose.Application/GeneralPurpose.Application.csproj", "src/GeneralPurpose.Application/"]
COPY ["src/GeneralPurpose.Domain/GeneralPurpose.Domain.csproj", "src/GeneralPurpose.Domain/"]
COPY ["src/GeneralPurpose.Infrastructure/GeneralPurpose.Infrastructure.csproj", "src/GeneralPurpose.Infrastructure/"]
RUN dotnet restore "src/GeneralPurpose.Api/GeneralPurpose.Api.csproj"

# Copy everything
COPY . ./
# Build and publish a release
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /src/out .

EXPOSE 8080
ENTRYPOINT ["dotnet", "GeneralPurpose.Api.dll"]