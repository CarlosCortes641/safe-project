# Informe de ingeniería — Safe Project Solution

**Fecha:** 12 de agosto de 2026  
**Marca:** Safe Project Solution by Safe HVAC Solution LLC  
**Tecnología:** Powered by Home Indor Technology Inc.  
**Destinatarios:** Oscar, Carlos, Sofía (paquete de kickoff)  
**Fuentes:** prototipo publicado, Engineering Handoff ES, `LEER_PRIMERO_PAQUETE_WHATSAPP_ES.md`, implementación actual en `safe-project`

Este documento es la primera respuesta de ingeniería solicitada en el paquete de WhatsApp.

**Restricción vigente:** no activar pagos reales, Safe 24 productivo ni pago al cierre hasta que contratos, divulgaciones, privacidad, seguridad y cumplimiento estén aprobados.

---

## 1. Arquitectura técnica propuesta

### 1.1 Decisión

Se propone **ASP.NET Core 8 MVC + Razor**, desktop-first, alineado al sitio Indor. El prototipo ChatGPT Sites (vinext/React) queda como **especificación visual**, no como base de producción.

Hoy Demo 1 corre en **un solo proyecto Web**, sin base de datos, para agilidad. La evolución a capas (Application / Infrastructure) se hace cuando exista EF Core, Identity y pagos alojados.

### 1.2 Estado actual (Demo 1 — sandbox)

```
src/
└── SafeProject.Web                 ASP.NET Core 8 MVC
    ├── Controllers                 Home, Project, Realtor, Ops, Language, Catalog
    ├── Views                       Razor + _Layout (no Razor Pages)
    ├── ViewModels / Models
    ├── Catalog/CatalogStore        Precios y productos hardcodeados
    ├── Demo/DemoStore              Memoria del proceso (proyectos, agenda, Realtor)
    ├── Infrastructure/SiteLanguage Cookie safe.lang + Lang.T(EN, ES)
    ├── wwwroot                     css/js del prototipo
    └── Program.cs                  AddControllersWithViews
```

Flujo: **navegador → Controller → CatalogStore / DemoStore → ViewModel → Vista Razor**.

Idioma: cookie `safe.lang` (`en`/`es`). No hay `IStringLocalizer` ni `.resx` todavía.

Persistencia Demo 1: **en memoria**. Al reiniciar el proceso se pierden proyectos; el catálogo y el día lleno 6/6 vuelven del código.

### 1.3 Arquitectura objetivo (producción)

```
Cliente (EN/ES)
    → SafeProject.Web (MVC, Identity, CSRF, HTTPS)
        → Application (casos de uso: alcance, pago, agenda, Deal Desk)
            → Infrastructure
                ├── SQL Server + EF Core 8
                ├── Blob privado (archivos de inspección / cierre)
                ├── Checkout alojado (PCI)
                ├── Synchrony (solo enlace oficial + conciliación)
                ├── Email / SMS
                └── CRM / dispatch (cuando exista)
```

Reglas no negociables:

| Regla | Implicación |
|---|---|
| Safe no guarda tarjeta ni SSN | Solo procesador / Synchrony |
| No marcar “pagado” por redirect | Webhook firmado + conciliación |
| No cita sin pago o financiamiento en estado válido | Agenda después de autorización sandbox o real |
| Capacidad no hard-coded en producción | Config en BD; default 6; reserva atómica |
| Paridad EN/ES | UI, errores, emails, recibos, divulgaciones |
| Archivos privados | Fuera de URLs públicas, analytics y `llms.txt` |

### 1.4 Recorrido comercial a implementar

1. Water heater: producto → precio → pago preferido → inspección → alcance → pago → agenda → garantía.  
2. HVAC: preguntas → precio inicial si aplica → inspección → alcance → pago → agenda.  
3. Realtor / cierre: solicitud → evidencia → revisión → ejecución → paquete de cierre. Pago al cierre = **evaluación**, no promesa.

---

## 2. Backlog por fases y dependencias

```
Demo 1 (sandbox, sin BD)  ──completada──► 12 ago 2026
        │
        ▼
Fase 0 Fundación ──► Fase 1 Persistencia + Identity
                          │
              ┌───────────┼───────────┐
              ▼           ▼           ▼
         Fase 2 Pagos   Fase 3 Agenda  Fase 4 Deal Desk
         (sandbox→hosted) (BD + dispatch) (archivos privados)
              │           │           │
              └─────► Fase 5 Operaciones + salida a producción
                      (legal, pentest, notificaciones)
```

| Fase | Alcance | Depende de | Estado |
|---|---|---|---|
| **Demo 1** | Catálogo bilingüe, selección persistente en sesión, 3 pagos sandbox, agenda 6/día, Realtor, tablero + auditoría (solo demo) | Handoff + prototipo | **Hecha** (local `localhost:5118`) |
| **0 Fundación** | Repo GitHub oficial, ambientes, CI/CD, secretos, dominio, observabilidad | Acceso GitHub / hosting | Pendiente del negocio |
| **1 Persistencia** | SQL Server + EF Core 8, Identity, catálogo administrable, alcance versionado, `/en` `/es` | Fase 0 | Planificada |
| **2 Pagos** | Checkout alojado, handoff Synchrony, stub Safe 24, webhooks idempotentes | Fase 1 + credenciales de pago | Planificada; **prod bloqueada por legal** |
| **3 Agenda** | Capacidad en BD, hold 10 min, transacción atómica, zona `America/New_York` | Fase 2 (estado de pago) + calendario ops | Planificada |
| **4 Deal Desk** | Portal Realtor, uploads privados, pago al cierre = revisión | Fase 1 + legal title/settlement | Planificada |
| **5 Salida** | Email/SMS, conciliación, WCAG 2.2 AA, pentest, go-live | Fases 2–4 + aprobación legal | Planificada |

Ítems Demo 1 ya cubiertos en el sitio MVC:

1. Catálogo EN/ES desde `CatalogStore` + `/catalog.json`  
2. Selección de water heater conservada en proyecto `SPS-…`  
3. Cash / Synchrony / Safe 24 en sandbox (sin cobro real, sin solicitud Synchrony en vivo)  
4. Agenda de prueba: 6 cupos; séptima unidad rechazada; un día sembrado 6/6  
5. Solicitud Realtor con etapa, necesidad y nombre de archivo de prueba  
6. Tablero interno `/ops` (proyectos, capacidad, Realtor; auditoría marcada solo demo)

---

## 3. Integraciones y credenciales requeridas

El negocio debe entregar o confirmar lo siguiente. Sin estos ítems, las fases 2–5 no salen de sandbox.

| Integración | Quién la da | Qué se necesita | Cuándo bloquea |
|---|---|---|---|
| **Checkout alojado (PCI)** | Negocio | Proveedor definitivo, cuenta merchant, claves test/live, webhook secret | Fase 2 producción |
| **Synchrony** | Negocio | Enlace merchant oficial (ya hay uno en prototipo), proceso de conciliación, contacto Synchrony | Fase 2 producción |
| **Safe 24** | Legal + negocio | Contrato, divulgaciones, elegibilidad, cargos, cancelación, privacidad | **Producción** (hoy solo ilustración) |
| **Pago al cierre** | Legal + title/settlement | Acuerdo, instrucciones de closing, qué ocurre si no cierra | Deal Desk producción |
| **Calendario / dispatch** | Ops | CRM o calendario de técnicos; capacidad real | Agenda real (Fase 3) |
| **Email / SMS** | Ops | Proveedor, plantillas bilingües, números/dominio | Fase 5 |
| **Almacenamiento privado** | Ingeniería + negocio | Cuenta blob (Azure/S3), política de retención | Fase 4 |
| **Hosting / dominio** | Negocio + ingeniería | Azure (u otro), DNS, certificados | Fase 0 |
| **GitHub** | Negocio | Repo oficial (aún no entregado al equipo) | Fase 0 |
| **Analítica** | Negocio | Consentimiento + herramienta (sin datos de pago/documentos) | Fase 5 |

**Ya conocidos (públicos, no son secretos):**

- Teléfono: (980) 447-0919  
- Email: safehvacsolution@gmail.com  
- Synchrony prototipo: `https://www.synchrony.com/mmc/S6239195200?sitecode=acewel402`  
  (Demo 1 **no** abre este enlace; queda sandbox hasta aprobación)

**No deben pedirse ni guardarse:** números de tarjeta, SSN, documentos de crédito, ni secretos en el repositorio.

---

## 4. Riesgos técnicos, legales, de seguridad y disponibilidad

| Área | Riesgo | Impacto | Mitigación |
|---|---|---|---|
| Legal | Activar Safe 24 o pago al cierre sin contrato | Alto | Sandbox + textos de ilustración; go-live solo con aprobación |
| Pagos | Marcar pagado por redirect del navegador | Alto | Webhook firmado, idempotencia, conciliación |
| Pagos | Abrir Synchrony live desde la demo | Alto | Demo 1 no abre la solicitud real |
| Agenda | Overbooking (7ª unidad concurrente) | Alto | Reserva atómica; en Demo 1 ya se rechaza el cupo lleno en memoria |
| Privacidad | Fotos/PDF de inspección en URLs públicas o `llms.txt` | Alto | Bucket privado, MIME allowlist, enlaces firmados (Fase 4) |
| Datos | Demo 1 en memoria: se pierde al reiniciar | Medio | Aceptable para demo; EF Core en Fase 1 |
| Datos | Precios duplicados (UI vs JSON vs cotización) | Medio | Una sola fuente (`CatalogStore` hoy; admin + BD después) |
| Seguridad | CSRF, XSS, secretos en cliente | Alto | Antiforgery ya en POST; HTTPS/HSTS/CSP en Fase 0; secret manager |
| Disponibilidad | Un solo proceso Kestrel local | Medio | Hosting + CI en Fase 0; no usar la demo local como producción |
| Cumplimiento | Afirmar “$0 down garantizado”, “sin intereses”, licencias no evidenciadas | Alto | Textos del handoff; no publicar claims sin evidencia |
| Mantenimiento | Prototipo vinext minificado vs sitio MVC | Bajo | Producción es el MVC; prototipo solo referencia UX |

Disponibilidad Demo 1: el sitio vive mientras corre `dotnet run`. Cerrar Cursor apaga esa sesión. Volver a levantar el ambiente deja el demo operativo (catálogo y reglas); no conserva proyectos de la sesión anterior.

---

## 5. Estimación, responsables y duración por fase

Supuesto: **1–2 ingenieros** (Home Indor Technology Inc.) + respuestas del negocio en ≤ 3 días hábiles. Las duraciones son **semanas calendario de trabajo efectivo**, no fechas fijas de contrato.

| Fase | Duración | Responsable principal | Apoyo |
|---|---|---|---|
| Demo 1 sandbox | Hecha (esfuerzo de kickoff) | Ingeniería — Home Indor | Producto (textos/UX del prototipo) |
| 0 Fundación | 1–2 semanas | Ingeniería | Negocio (GitHub, dominio, hosting) |
| 1 Persistencia + Identity | 2–3 semanas | Ingeniería | Producto (catálogo administrable) |
| 2 Pagos sandbox → hosted | 2–3 semanas | Ingeniería | Negocio (merchant) + Legal (límites) |
| 3 Agenda durable | 1–2 semanas | Ingeniería | Ops (capacidad real, dispatch) |
| 4 Property Deal Desk | 2–3 semanas | Ingeniería | Legal + Realtors (flujo de cierre) |
| 5 Operaciones + go-live | 2–3 semanas | Ingeniería + Legal + Negocio | Ops (SMS/email, conciliación) |

**Total restante hacia producción (si las dependencias llegan a tiempo): 10–16 semanas** después de Demo 1.

Si checkout, Synchrony o Safe 24 se retrasan, las Fases 2–5 se quedan en sandbox; no se inventa un go-live.

Roles del paquete de kickoff (a confirmar en WhatsApp):

| Rol | Persona / entidad |
|---|---|
| Ingeniería / implementación | Home Indor Technology Inc. |
| Producto / negocio | Safe HVAC Solution LLC — Oscar, Carlos, Sofía |
| Legal / cumplimiento | Por designar (abogado + title/settlement) |
| Operaciones / dispatch | Safe HVAC Solution LLC |

---

## 6. Fecha de la primera demostración técnica

**Demo 1 ya es ejecutable** en el repositorio `safe-project` a fecha **12 de agosto de 2026**.

Cubre el mínimo pedido en el paquete:

- Catálogo bilingüe  
- Selección de producto persistente en el proyecto  
- Tres rutas de pago en sandbox  
- Agenda de prueba con capacidad 6  
- Solicitud Realtor  
- Tablero interno básico  

**Fecha propuesta de demostración formal al negocio:**  
**viernes 14 de agosto de 2026** (recorrido en pantalla compartida, ~45–60 min).

Si el grupo no puede ese día: **martes 18 de agosto de 2026**.

### Cómo correrla

Requisito: .NET 8 SDK.

```bash
cd /Users/mac/Projects/safe-project
dotnet run --project src/SafeProject.Web --urls http://localhost:5118
```

- Sitio: http://localhost:5118  
- Builder: http://localhost:5118/#builder  
- Tablero: http://localhost:5118/ops  
- Prototipo de referencia: https://safe-sales-prototype.espacio-de-t-1359.chatgpt.site  

### Guion sugerido (15 min de recorrido)

1. Builder water heater EN → ES  
2. Continuar → 7 pantallas hasta revisión de pago (sandbox)  
3. Ver el proyecto en `/ops`  
4. Agendar un día abierto y uno lleno (6/6)  
5. Solicitud Realtor (pago al cierre = solo revisión)  
6. Confirmar en voz alta: **no hay cobro real, no hay Safe 24 activo, no hay pago al cierre**

---

## Anexo — Criterios de aceptación (handoff §16, recorte Demo 1)

| Criterio | Demo 1 |
|---|---|
| Water heater y HVAC conservan producto, precio y pago | Sí |
| Tres rutas con textos distintos | Sí (sandbox) |
| Capacidad 6; día lleno bloqueado | Sí (memoria) |
| Realtor con etapa y necesidad | Sí |
| Pago al cierre no es garantía | Sí (solo revisión) |
| EN/ES equivalentes en pantallas demo | Sí (helper `Lang.T`) |
| Webhooks, PCI, WCAG 2.2 AA, pentest | No — Fases 2 y 5 |

---

*Documento preparado como primera entrega de ingeniería. Pendiente confirmar fecha de demo y responsables nominados en el grupo de WhatsApp.*
