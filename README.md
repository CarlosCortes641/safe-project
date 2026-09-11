# Safe Project Solution

Website comercial desktop-first para Safe HVAC Solution LLC.
Stack inicial: ASP.NET Core 8 MVC + Razor + CSS + JavaScript.

Por agilidad, **no hay base de datos**. Catálogo, precios, capacidad de agenda y textos EN/ES viven hardcodeados en `CatalogStore` y las vistas.

## Correr

Requisito: .NET 8 SDK.

```bash
cd /Users/mac/Projects/safe-project
dotnet run --project src/SafeProject.Web --urls http://localhost:5118
```

Luego abre `http://localhost:5118`.

## Demo 1 (sandbox, sin BD)

1. Catálogo bilingüe en `CatalogStore` + `/catalog.json`
2. Selección de producto persistida en un proyecto in-memory (`/project/SPS-…`)
3. Tres rutas de pago **sandbox** (cash / Synchrony / Safe 24). No hay cobro real.
4. Agenda de prueba con capacidad 6; el día lleno rechaza la 7ª unidad
5. Solicitud Realtor en `/realtor` (pago al cierre = solo revisión)
6. Tablero interno en `/ops` con estados y auditoría

**No activado:** pagos reales, Safe 24 productivo, ni pago al cierre.

## Publicar (Azure App Service)

1. Crea un **Web App** en Azure: runtime **.NET 8**, publicar **Código**.
2. En el App Service → **Deployment Center** → GitHub → repo `safe-project`, branch `main`.
3. Opcional: Application setting `ASPNETCORE_ENVIRONMENT=Production`.
4. Health check (opcional): `/health`.

## Todavía no

- Entity Framework / Identity
- Webhooks de un procesador real
- Almacenamiento privado de archivos
