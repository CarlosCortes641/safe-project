FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish src/SafeProject.Web/SafeProject.Web.csproj \
    -c Release \
    -r linux-x64 \
    --self-contained false \
    -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Render Free = 512 MB. Keep workstation GC, small heap, few threads.
ENV ASPNETCORE_ENVIRONMENT=Production \
    DOTNET_EnableDiagnostics=0 \
    DOTNET_GCServer=0 \
    DOTNET_GCHeapHardLimit=201326592 \
    DOTNET_ThreadPool_ForceMinWorkerThreads=1 \
    DOTNET_ThreadPool_ForceMaxWorkerThreads=4

EXPOSE 8080
ENTRYPOINT ["/bin/sh", "-c", "dotnet SafeProject.Web.dll --urls http://0.0.0.0:${PORT:-8080}"]
