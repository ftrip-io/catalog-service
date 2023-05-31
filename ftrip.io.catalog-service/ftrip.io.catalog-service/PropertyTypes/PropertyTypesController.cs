using ftrip.io.catalog_service.PropertyTypes.UseCases.ReadAll;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.catalog_service.PropertyTypes
{
    [Route("api/[controller]")]
    [ApiController]
    public class PropertyTypesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PropertyTypesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct = default)
        {
            return Ok(await _mediator.Send(new ReadAllQuery(), ct));
        }
    }
}
