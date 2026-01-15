FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/HRManagement.BlazorUI/HRManagement.BlazorUI.csproj", "HRManagement.BlazorUI/"]
COPY ["src/HRManagement.Application/HRManagement.Application.csproj", "HRManagement.Application/"]
COPY ["src/HRManagement.Domain/HRManagement.Domain.csproj", "HRManagement.Domain/"]
COPY ["src/HRManagement.Infrastructure/HRManagement.Infrastructure.csproj", "HRManagement.Infrastructure/"]
RUN dotnet restore "HRManagement.BlazorUI/HRManagement.BlazorUI.csproj"
COPY src/ .
WORKDIR "/src/HRManagement.BlazorUI"
RUN dotnet build "HRManagement.BlazorUI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HRManagement.BlazorUI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HRManagement.BlazorUI.dll"]
