namespace SafeProject.Web.Infrastructure;

public static class SeoContent
{
    public sealed record CityPage(
        string Slug,
        string NameEn,
        string NameEs,
        string CountyEn,
        string CountyEs,
        string State);

    public sealed record FaqItem(string EnQ, string EsQ, string EnA, string EsA);

    public static readonly IReadOnlyList<CityPage> Cities =
    [
        new("mint-hill", "Mint Hill", "Mint Hill", "Mecklenburg County", "Condado de Mecklenburg", "NC"),
        new("matthews", "Matthews", "Matthews", "Mecklenburg County", "Condado de Mecklenburg", "NC"),
        new("huntersville", "Huntersville", "Huntersville", "Mecklenburg County", "Condado de Mecklenburg", "NC"),
        new("concord", "Concord", "Concord", "Cabarrus County", "Condado de Cabarrus", "NC"),
        new("gastonia", "Gastonia", "Gastonia", "Gaston County", "Condado de Gaston", "NC"),
        new("rock-hill", "Rock Hill", "Rock Hill", "York County", "Condado de York", "SC")
    ];

    public static readonly IReadOnlyList<FaqItem> Faqs =
    [
        new(
            "Is the displayed price final?",
            "¿El precio mostrado es final?",
            "It covers the published standard scope. Safe confirms site conditions and discloses non-standard work before authorization.",
            "Cubre el alcance estándar publicado. Safe confirma condiciones del sitio y divulga trabajo no estándar antes de autorizar."),
        new(
            "Is $0 down with Synchrony guaranteed?",
            "¿$0 inicial con Synchrony está garantizado?",
            "No. It may be available for approved applicants. Synchrony determines the promotion, APR, term, and payment.",
            "No. Puede estar disponible para solicitantes aprobados. Synchrony determina promoción, APR, plazo y pago."),
        new(
            "Is Safe 24 the same as Synchrony?",
            "¿Safe 24 es lo mismo que Synchrony?",
            "No. Safe 24 is the company eligibility path with a $100 initial payment; Synchrony is a separate third-party credit application.",
            "No. Safe 24 es la ruta de elegibilidad de la compañía con $100 inicial; Synchrony es una solicitud de crédito de terceros."),
        new(
            "Does this reserve an appointment?",
            "¿Esto reserva una cita?",
            "No. Safe first confirms scope, price, and availability.",
            "No. Safe primero confirma alcance, precio y disponibilidad."),
        new(
            "What area does Safe serve?",
            "¿Qué área cubre Safe?",
            "Charlotte, North Carolina and communities approximately one hour away, including nearby cities in North Carolina and the Rock Hill, SC area.",
            "Charlotte, North Carolina y comunidades a aproximadamente una hora, incluyendo ciudades cercanas en North Carolina y el área de Rock Hill, SC."),
        new(
            "What does a standard water heater install include?",
            "¿Qué incluye una instalación estándar de water heater?",
            "New equipment, expansion tank when applicable, removal of the old unit, standard connections, 2-year Safe labor, and 5-year manufacturer warranty on covered equipment.",
            "Equipo nuevo, tanque de expansión cuando aplique, retiro del equipo anterior, conexiones estándar, 2 años de labor Safe y 5 años de garantía del fabricante en equipo cubierto."),
        new(
            "Is ductwork included in HVAC replacement?",
            "¿El ductwork está incluido en el reemplazo HVAC?",
            "No. Published HVAC prices cover cooling and heating equipment replacement with standard labor. Ductwork is evaluated and priced separately when needed.",
            "No. Los precios HVAC publicados cubren el reemplazo de equipo de enfriamiento y calefacción con labor estándar. El ductwork se evalúa y cotiza por separado cuando se necesita."),
        new(
            "How fast can Safe install?",
            "¿Qué tan rápido puede instalar Safe?",
            "After scope and payment path are confirmed, installation is scheduled based on published capacity. Same-week options may be available depending on demand.",
            "Después de confirmar alcance y ruta de pago, la instalación se agenda según capacidad publicada. Puede haber opciones en la misma semana según demanda."),
        new(
            "Can I text a photo of my water heater?",
            "¿Puedo enviar por texto una foto de mi water heater?",
            "Yes. Text your address and a clear photo of the tank and model label. Safe helps confirm the right replacement and standard scope.",
            "Sí. Envía tu dirección y una foto clara del tanque y la etiqueta del modelo. Safe ayuda a confirmar el reemplazo correcto y el alcance estándar."),
        new(
            "Do you help Realtors before closing?",
            "¿Ayudan a Realtors antes del cierre?",
            "Yes. Safe supports inspection findings, repair planning, documentation, and coordination for Charlotte-area transactions. Payment at closing remains an eligibility evaluation only in this sandbox.",
            "Sí. Safe apoya hallazgos de inspección, plan de reparación, documentación y coordinación para transacciones en el área de Charlotte. El pago al cierre sigue siendo solo una evaluación de elegibilidad en este sandbox.")
    ];

    public static CityPage? FindCity(string slug) =>
        Cities.FirstOrDefault(c => string.Equals(c.Slug, slug, StringComparison.OrdinalIgnoreCase));
}
