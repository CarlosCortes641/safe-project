namespace SafeProject.Web.Demo;

public static class DemoCopy
{
    public static string PaymentName(PaymentPath path, bool es) => path switch
    {
        PaymentPath.CashSandbox => es ? "Pago completo · sandbox" : "Pay in full · sandbox",
        PaymentPath.SynchronySandbox => "Synchrony · sandbox",
        PaymentPath.Safe24Sandbox => es ? "Safe 24 · sandbox (no activado)" : "Safe 24 · sandbox (not activated)",
        _ => path.ToString()
    };

    public static string PaymentNameCustomer(PaymentPath path, bool es, bool confirmed = false) => path switch
    {
        PaymentPath.CashSandbox => confirmed
            ? (es ? "Pagado completo" : "Paid in full")
            : (es ? "Pago completo" : "Pay in full"),
        PaymentPath.SynchronySandbox => "Synchrony",
        PaymentPath.Safe24Sandbox => "Safe 24",
        _ => path.ToString()
    };

    public static string KindName(string kind, bool es) =>
        string.Equals(kind, "hvac", StringComparison.OrdinalIgnoreCase)
            ? "HVAC"
            : "Water heater";

    public static string StatusName(ProjectStatus status, bool es) => status switch
    {
        ProjectStatus.Draft => es ? "Borrador" : "Draft",
        ProjectStatus.ScopeApproved => es ? "Alcance aprobado" : "Scope approved",
        ProjectStatus.PaymentPending => es ? "Pago sandbox pendiente" : "Sandbox payment pending",
        ProjectStatus.PaymentAuthorizedSandbox => es ? "Pago sandbox autorizado" : "Sandbox payment authorized",
        ProjectStatus.Scheduled => es ? "Agendado" : "Scheduled",
        ProjectStatus.Cancelled => es ? "Cancelado" : "Cancelled",
        _ => status.ToString()
    };
}
