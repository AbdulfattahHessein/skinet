using System.ComponentModel.DataAnnotations;
using Core.Entities;

namespace API.DTOs;

public class AddressDto
{
    [Required]
    public required string Line1 { get; set; }

    public string? Line2 { get; set; }

    [Required]
    public required string City { get; set; }

    [Required]
    public required string State { get; set; }

    [Required]
    public required string PostalCode { get; set; }

    [Required]
    public required string Country { get; set; }

    public static implicit operator Address(AddressDto addressDto) =>
        new()
        {
            Line1 = addressDto.Line1,
            Line2 = addressDto.Line2,
            City = addressDto.City,
            State = addressDto.State,
            PostalCode = addressDto.PostalCode,
            Country = addressDto.Country,
        };

    public static implicit operator AddressDto(Address address) =>
        new()
        {
            Line1 = address.Line1,
            Line2 = address.Line2,
            City = address.City,
            State = address.State,
            PostalCode = address.PostalCode,
            Country = address.Country,
        };

    public static void Update(Address address, AddressDto addressDto)
    {
        address.Line1 = addressDto.Line1;
        address.Line2 = addressDto.Line2;
        address.City = addressDto.City;
        address.State = addressDto.State;
        address.PostalCode = addressDto.PostalCode;
        address.Country = addressDto.Country;
    }
}
