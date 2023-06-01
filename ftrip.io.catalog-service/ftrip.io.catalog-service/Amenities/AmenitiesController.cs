using ftrip.io.catalog_service.Amenities.UseCases.ReadAll;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.catalog_service.Amenities
{
    [Route("api/[controller]")]
    [ApiController]
    public class AmenitiesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AmenitiesController(IMediator mediator)
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
