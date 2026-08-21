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
COPY start.sh /app/start.sh
RUN chmod +x /app/start.sh

# Render Free = 512 MB. Workstation GC + conserve memory.
# Do NOT set GCHeapHardLimit: hitting it aborts with exit 134.
ENV ASPNETCORE_ENVIRONMENT=Production \
    DOTNET_EnableDiagnostics=0 \
    DOTNET_GCServer=0 \
    DOTNET_GCConserveMemory=9 \
    DOTNET_ThreadPool_ForceMinWorkerThreads=1 \
    DOTNET_ThreadPool_ForceMaxWorkerThreads=4

EXPOSE 8080
ENTRYPOINT ["/app/start.sh"]
