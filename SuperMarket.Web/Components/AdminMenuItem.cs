namespace SuperMarket.Web.Components;

public sealed class AdminMenuItem
{
    public string Title { get; init; } = default!;
    public string Controller { get; init; } = default!;
    public string Action { get; init; } = default!;
    public string Icon { get; init; } = default!;
    public bool IsActive { get; set; }
    public bool IsLogout { get; init; }
}