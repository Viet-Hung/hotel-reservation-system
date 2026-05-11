using HotelReservation.Domain.Common;

namespace HotelReservation.Domain.Hotels.ValueObjects;

public sealed class Address : ValueObject
{
    public string Street { get; }
    public string City { get; }
    public string Country { get; }
    public string PostalCode { get; }

    private Address(string street, string city, string country, string postalCode)
    {
        Street = street;
        City = city;
        Country = country;
        PostalCode = postalCode;
    }

    public static Address Create(string street, string city, string country, string postalCode)
    {
        if (string.IsNullOrWhiteSpace(street))
            throw new DomainException("Street is required.");

        if (street.Length < 5 || street.Length > 200)
            throw new DomainException("Street must be between 5 and 200 characters.");

        if (string.IsNullOrWhiteSpace(city))
            throw new DomainException("City is required.");

        if (city.Length < 2 || city.Length > 100)
            throw new DomainException("City must be between 2 and 100 characters.");

        if (string.IsNullOrWhiteSpace(country))
            throw new DomainException("Country is required.");

        if (country.Length < 2 || country.Length > 100)
            throw new DomainException("Country must be between 2 and 100 characters.");

        if (string.IsNullOrWhiteSpace(postalCode))
            throw new DomainException("Postal code is required.");

        if (postalCode.Length < 4 || postalCode.Length > 10)
            throw new DomainException("Postal code must be between 4 and 10 characters.");

        if (!postalCode.All(char.IsLetterOrDigit))
            throw new DomainException("Postal code must contain only letters and digits.");

        return new Address(
            street.Trim(),
            city.Trim(),
            country.Trim(),
            postalCode.Trim()
        );
    }

    public override string ToString()
    {
        return $"{Street}, {City}, {Country} {PostalCode}";
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Street;
        yield return City;
        yield return Country;
        yield return PostalCode;
    }
}