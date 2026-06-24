FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY src/ .

RUN find . -type d \( -name bin -o -name obj \) -prune -exec rm -rf {} +

RUN dotnet restore "HRManagement.BlazorUI/HRManagement.BlazorUI.csproj"

WORKDIR "/src/HRManagement.BlazorUI"

RUN dotnet publish "HRManagement.BlazorUI.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "HRManagement.BlazorUI.dll"]