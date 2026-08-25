using System;
using System.Collections.Generic;

namespace SuperMarket.Domain.ValueObjects
{
    public sealed class ShippingAddress : ValueObject
    {
        public string FullName { get; }

        public string Phone { get; }

        public string Province { get; }

        public string City { get; }

        public string AddressLine { get; }

        public string PostalCode { get; }

        public string? Plaque { get; }

        public string? Unit { get; }

        public string? DeliveryNote { get; }

        private ShippingAddress() { }

        private ShippingAddress(
            string fullName,
            string phone,
            string province,
            string city,
            string addressLine,
            string postalCode,
            string? plaque,
            string? unit,
            string? deliveryNote)
        {
            FullName = fullName;
            Phone = phone;
            Province = province;
            City = city;
            AddressLine = addressLine;
            PostalCode = postalCode;
            Plaque = plaque;
            Unit = unit;
            DeliveryNote = deliveryNote;
        }

        public static ShippingAddress Create(
            string fullName,
            string phone,
            string province,
            string city,
            string addressLine,
            string postalCode,
            string? plaque = null,
            string? unit = null,
            string? deliveryNote = null)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                throw new ArgumentException("Full name is required.", nameof(fullName));

            if (string.IsNullOrWhiteSpace(phone))
                throw new ArgumentException("Phone is required.", nameof(phone));

            if (string.IsNullOrWhiteSpace(province))
                throw new ArgumentException("Province is required.", nameof(province));

            if (string.IsNullOrWhiteSpace(city))
                throw new ArgumentException("City is required.", nameof(city));

            if (string.IsNullOrWhiteSpace(addressLine))
                throw new ArgumentException("Address is required.", nameof(addressLine));

            if (string.IsNullOrWhiteSpace(postalCode))
                throw new ArgumentException("Postal code is required.", nameof(postalCode));

            if (fullName.Length > 150)
                throw new ArgumentException("Full name cannot exceed 150 characters.", nameof(fullName));

            if (phone.Length > 20)
                throw new ArgumentException("Phone cannot exceed 20 characters.", nameof(phone));

            if (province.Length > 100)
                throw new ArgumentException("Province cannot exceed 100 characters.", nameof(province));

            if (city.Length > 100)
                throw new ArgumentException("City cannot exceed 100 characters.", nameof(city));

            if (addressLine.Length > 500)
                throw new ArgumentException("Address cannot exceed 500 characters.", nameof(addressLine));

            if (postalCode.Length > 20)
                throw new ArgumentException("Postal code cannot exceed 20 characters.", nameof(postalCode));

            return new ShippingAddress(
                fullName.Trim(),
                phone.Trim(),
                province.Trim(),
                city.Trim(),
                addressLine.Trim(),
                postalCode.Trim(),
                string.IsNullOrWhiteSpace(plaque) ? null : plaque.Trim(),
                string.IsNullOrWhiteSpace(unit) ? null : unit.Trim(),
                string.IsNullOrWhiteSpace(deliveryNote) ? null : deliveryNote.Trim());
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return FullName;
            yield return Phone;
            yield return Province;
            yield return City;
            yield return AddressLine;
            yield return PostalCode;
            yield return Plaque ?? string.Empty;
            yield return Unit ?? string.Empty;
            yield return DeliveryNote ?? string.Empty;
        }
    }
}
