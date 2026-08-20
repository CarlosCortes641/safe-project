FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish src/SafeProject.Web/SafeProject.Web.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080
# Render sets PORT at runtime; default 8080 for local Docker runs.
ENTRYPOINT ["/bin/sh", "-c", "dotnet SafeProject.Web.dll --urls http://0.0.0.0:${PORT:-8080}"]
