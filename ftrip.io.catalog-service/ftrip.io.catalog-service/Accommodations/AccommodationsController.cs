﻿using ftrip.io.catalog_service.Accommodations.UseCases.CreateAccommodation;
using ftrip.io.catalog_service.Accommodations.UseCases.DeleteAccommodation;
using ftrip.io.catalog_service.Accommodations.UseCases.ReadById;
using ftrip.io.catalog_service.Accommodations.UseCases.UpdateAccommodation;
using ftrip.io.framework.Contexts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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
        private readonly CurrentUserContext _currentUserContext;

        public AccommodationsController(IMediator mediator, CurrentUserContext currentUserContext)
        {
            _mediator = mediator;
            _currentUserContext = currentUserContext;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ReadById(Guid id, [FromQuery] bool s, CancellationToken ct = default)
        {
            return Ok(await _mediator.Send(new ReadByIdQuery { Id = id, Simple = s }, ct));
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateAccommodationRequest request, CancellationToken ct = default)
        {
            request.HostId = Guid.Parse(_currentUserContext.Id);
            return Ok(await _mediator.Send(request, ct));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, UpdateAccommodationRequest request, CancellationToken ct = default)
        {
            request.Id = id;
            return Ok(await _mediator.Send(request, ct));
        }

        [HttpPut("{id}/amenities")]
        public async Task<IActionResult> Update([FromRoute] Guid id, UpdateAccommodationAmenitiesRequest request, CancellationToken ct = default)
        {
            request.Id = id;
            return Ok(await _mediator.Send(request, ct));
        }

        [HttpPut("{id}/availabilities")]
        public async Task<IActionResult> Update([FromRoute] Guid id, UpdateAccommodationAvailabilitiesRequest request, CancellationToken ct = default)
        {
            request.Id = id;
            return Ok(await _mediator.Send(request, ct));
        }

        [HttpPut("{id}/location")]
        public async Task<IActionResult> Update([FromRoute] Guid id, UpdateAccommodationLocationRequest request, CancellationToken ct = default)
        {
            request.Id = id;
            return Ok(await _mediator.Send(request, ct));
        }

        [HttpPut("{id}/pricing")]
        public async Task<IActionResult> Update([FromRoute] Guid id, UpdateAccommodationPricingRequest request, CancellationToken ct = default)
        {
            request.Id = id;
            return Ok(await _mediator.Send(request, ct));
        }

        [Authorize]
        [HttpGet("can-modify/{id}")]
        public async Task<IActionResult> CanModify(Guid id, CancellationToken ct = default)
        {
            var accommodation = await _mediator.Send(new ReadByIdQuery { Id = id, Simple = true }, ct);
            if (accommodation.HostId.ToString() != _currentUserContext.Id) return Forbid();
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct = default)
        {
            return Ok(await _mediator.Send(new DeleteAccommodationRequest { Id = id }, ct));
        }
    }
}

