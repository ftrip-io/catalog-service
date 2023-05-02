using ftrip.io.catalog_service.Accommodations.UseCases.CreateAccommodation;
using ftrip.io.catalog_service.Accommodations.UseCases.ReadById;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.catalog_service.Accommodations
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccommodationsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AccommodationsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ReadById(Guid id, CancellationToken ct = default)
        {
            return Ok(await _mediator.Send(new ReadByIdQuery() { Id = id }, ct));
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateAccommodationRequest request, CancellationToken ct = default)
        {
            return Ok(await _mediator.Send(request, ct));
        }
    }
}

