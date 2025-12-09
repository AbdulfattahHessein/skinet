using API.RequestHelpers;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BaseApiController : ControllerBase
{
    protected async Task<ActionResult> CreatePageResult<T>(
        IGenericRepository<T> repository,
        ISpecification<T> spec,
        int pageIndex,
        int pageSize
    )
        where T : BaseEntity
    {
        var totalItems = await repository.CountAsync(spec.Criteria);
        var pagedProducts = new Pagination<T>(
            pageIndex,
            pageSize,
            totalItems,
            await repository.ListAsync(spec)
        );
        return Ok(pagedProducts);
    }
}
