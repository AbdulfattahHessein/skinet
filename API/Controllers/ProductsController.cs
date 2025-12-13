using Core.Entities;
using Core.Interfaces;
using Core.Specification;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class ProductsController(IUnitOfWork unit) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts(
        [FromQuery] ProductSpecParams specParams
    )
    {
        var spec = new ProductSpecification(specParams);

        return await CreatePageResult(
            unit.Repository<Product>(),
            spec,
            specParams.PageIndex,
            specParams.PageSize
        );
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Product>> GetProductById(int id)
    {
        var product = await unit.Repository<Product>().GetByIdAsync(id);
        if (product is null)
            return NotFound();
        return product;
    }

    [HttpPost]
    public async Task<ActionResult<Product>> CreateProduct(Product product)
    {
        unit.Repository<Product>().Add(product);
        if (await unit.Complete())
        {
            return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
        }
        return BadRequest("Failed to create product");
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateProduct(int id, Product product)
    {
        if (!unit.Repository<Product>().Exists(id) || id != product.Id)
            return NotFound();
        unit.Repository<Product>().Update(product);

        if (await unit.Complete())
        {
            return NoContent();
        }
        return BadRequest("Failed to update product");
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteProduct(int id)
    {
        var product = await unit.Repository<Product>().GetByIdAsync(id);
        if (product is null)
            return NotFound();
        unit.Repository<Product>().Delete(product);
        if (await unit.Complete())
        {
            return NoContent();
        }
        return BadRequest("Failed to delete product");
    }

    [HttpGet("brands")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetBrands()
    {
        var spec = new ProductSpecification<string>(null, p => p.Brand, null, true);

        return Ok(await unit.Repository<Product>().ListAsync(spec));
    }

    [HttpGet("types")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetTypes()
    {
        var spec = new ProductSpecification<string>(null, p => p.Type, null, true);

        return Ok(await unit.Repository<Product>().ListAsync(spec));
    }
}
