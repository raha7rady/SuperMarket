namespace SuperMarket.Web.Components;

public sealed class MenuItem
{
    public string Title { get; init; } = string.Empty;

    public string Controller { get; init; } = string.Empty;

    public string Action { get; init; } = string.Empty;

    public string Area { get; init; } = string.Empty;

    public string Icon { get; init; } = string.Empty;

    public bool IsVisible { get; init; } = true;
}