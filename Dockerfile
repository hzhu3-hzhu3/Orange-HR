FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["src/HRManagement.BlazorUI/HRManagement.BlazorUI.csproj", "HRManagement.BlazorUI/"]
COPY ["src/HRManagement.Application/HRManagement.Application.csproj", "HRManagement.Application/"]
COPY ["src/HRManagement.Domain/HRManagement.Domain.csproj", "HRManagement.Domain/"]
COPY ["src/HRManagement.Infrastructure/HRManagement.Infrastructure.csproj", "HRManagement.Infrastructure/"]

RUN dotnet restore "HRManagement.BlazorUI/HRManagement.BlazorUI.csproj"

COPY src/ .

WORKDIR "/src/HRManagement.BlazorUI"

RUN dotnet publish "HRManagement.BlazorUI.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "HRManagement.BlazorUI.dll"]