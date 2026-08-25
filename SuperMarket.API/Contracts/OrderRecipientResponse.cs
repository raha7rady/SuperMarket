namespace SuperMarket.API.Contracts;

public sealed class OrderRecipientResponse
{
    public string FullName { get; init; } = string.Empty;

    public string Phone { get; init; } = string.Empty;

    public string Province { get; init; } = string.Empty;

    public string City { get; init; } = string.Empty;

    public string AddressLine { get; init; } = string.Empty;

    public string PostalCode { get; init; } = string.Empty;

    public string? Plaque { get; init; }

    public string? Unit { get; init; }

    public string? DeliveryNote { get; init; }
}
