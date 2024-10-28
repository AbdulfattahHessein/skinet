using System;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class CreateProductDto(
    string name,
    string description,
    decimal price,
    string pictureUrl,
    string type,
    string brand,
    int quantityInStock
)
{
    [Required]
    public string Name { get; set; } = name;
    public string Description { get; set; } = description;

    [Range(1, double.MaxValue, ErrorMessage = "Price must be greater than zero")]
    public decimal Price { get; set; } = price;

    [Required]
    public string PictureUrl { get; set; } = pictureUrl;

    [Required]
    public string Type { get; set; } = type;

    [Required]
    public string Brand { get; set; } = brand;

    [Range(1, int.MaxValue, ErrorMessage = "QuantityInStock must be at least 1")]
    public int QuantityInStock { get; set; } = quantityInStock;
}
