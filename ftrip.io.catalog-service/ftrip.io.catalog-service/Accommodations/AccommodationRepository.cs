using ftrip.io.framework.Persistence.Contracts;
using ftrip.io.framework.Persistence.Sql.Repository;
using ftrip.io.catalog_service.Accommodations.Domain;
using System;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Threading;
using ftrip.io.catalog_service.Accommodations.UseCases.UpdateAccommodation;
using System.Linq;
using System.Collections.Generic;
using ftrip.io.catalog_service.Accommodations.UseCases.SearchAccommodations;

namespace ftrip.io.catalog_service.Accommodations
{
    public interface IAccommodationRepository : IRepository<Accommodation, Guid>
    {
        Task<Accommodation> Update(UpdateAccommodationRequest accommodationUpdate, CancellationToken ct = default);
        Task<Accommodation> Update(UpdateAccommodationLocationRequest accommodationUpdate, CancellationToken ct = default);
        Task<Accommodation> Update(UpdateAccommodationAmenitiesRequest accommodationUpdate, CancellationToken ct = default);
        Task<Accommodation> Update(UpdateAccommodationAvailabilitiesRequest accommodationUpdate, CancellationToken ct = default);
        Task<Accommodation> Update(UpdateAccommodationPricingRequest accommodationUpdate, CancellationToken ct = default);
        Task<Accommodation> ReadSimple(Guid id, CancellationToken ct = default);
        Task<IEnumerable<Accommodation>> ReadByIds(Guid[] ids, CancellationToken ct = default);
        Task<IEnumerable<Accommodation>> ReadByQuery(SearchAccommodationQuery query, CancellationToken ct = default);
        Task<IEnumerable<Accommodation>> ReadByHostId(Guid hostId, CancellationToken ct = default);
        Task<IEnumerable<Accommodation>> DeleteByHostId(Guid hostId, CancellationToken ct = default);
    }

    public class AccommodationRepository : Repository<Accommodation, Guid>, IAccommodationRepository
    {
        public AccommodationRepository(DbContext context) : base(context)
        {
        }

        public override async Task<Accommodation> Read(Guid id, CancellationToken cancellationToken = default)
        {
            return await _entities
                .Include(a => a.PropertyType)
                .Include(a => a.Amenities).ThenInclude(aa => aa.Amenity)
                .Include(a => a.Location)
                .Include(a => a.Availabilities)
                .Include(a => a.PriceDiffs)
                .FirstOrDefaultAsync(e => e.Id.Equals(id), cancellationToken);
        }

        public Task<Accommodation> ReadSimple(Guid id, CancellationToken ct = default) => base.Read(id, ct);

        public async Task<Accommodation> Update(UpdateAccommodationRequest accommodationUpdate, CancellationToken ct = default)
        {
            var accommodation = await Read(accommodationUpdate.Id, ct);
            if (accommodation != null)
            {
                _context.Entry(accommodation).CurrentValues.SetValues(accommodationUpdate);
            }
            return accommodation;
        }
        public async Task<Accommodation> Update(UpdateAccommodationLocationRequest accommodationUpdate, CancellationToken ct = default)
        {
            var accommodation = await Read(accommodationUpdate.Id, ct);
            if (accommodation != null)
            {
                _context.Entry(accommodation.Location).CurrentValues.SetValues(accommodationUpdate.Location);
            }
            return accommodation;
        }
        public async Task<Accommodation> Update(UpdateAccommodationAmenitiesRequest accommodationUpdate, CancellationToken ct = default)
        {
            var accommodation = await Read(accommodationUpdate.Id, ct);
            if (accommodation == null) return null;

            foreach (var existingAccommodationAmenity in accommodation.Amenities)
            {
                if (!accommodationUpdate.Amenities.Any(aa => aa.Id == existingAccommodationAmenity.Id))
                    _context.Remove(existingAccommodationAmenity);
            }
            foreach (var accommodationAmenityUpdate in accommodationUpdate.Amenities)
            {
                var existingAccommodationAmenity = accommodation.Amenities.SingleOrDefault(aa => aa.Id == accommodationAmenityUpdate.Id);

                if (existingAccommodationAmenity != null)
                    _context.Entry(existingAccommodationAmenity).CurrentValues.SetValues(accommodationAmenityUpdate);
                else
                    _context.Add(new AccommodationAmenity
                    {
                        AmenityId = accommodationAmenityUpdate.AmenityId,
                        IsPresent = accommodationAmenityUpdate.IsPresent,
                        AccommodationId = accommodation.Id
                    });
            }
            return accommodation;
        }
        public async Task<Accommodation> Update(UpdateAccommodationAvailabilitiesRequest accommodationUpdate, CancellationToken ct = default)
        {
            var accommodation = await Read(accommodationUpdate.Id, ct);
            if (accommodation == null) return null;
            _context.Entry(accommodation).CurrentValues.SetValues(accommodationUpdate);
            foreach (var existingAvailability in accommodation.Availabilities)
            {
                if (!accommodationUpdate.Availabilities.Any(a => a.Id == existingAvailability.Id))
                    _context.Remove(existingAvailability);
            }
            foreach (var availabilityUpdate in accommodationUpdate.Availabilities)
            {
                var existingAvailability = accommodation.Availabilities.SingleOrDefault(a => a.Id == availabilityUpdate.Id);

                if (existingAvailability != null)
                    _context.Entry(existingAvailability).CurrentValues.SetValues(availabilityUpdate);
                else
                    _context.Add(new Availability
                    {
                        FromDate = availabilityUpdate.FromDate,
                        ToDate = availabilityUpdate.ToDate,
                        IsAvailable = availabilityUpdate.IsAvailable,
                        AccommodationId = accommodation.Id
                    });
            }
            return accommodation;
        }
        public async Task<Accommodation> Update(UpdateAccommodationPricingRequest accommodationUpdate, CancellationToken ct = default)
        {
            var accommodation = await Read(accommodationUpdate.Id, ct);
            if (accommodation == null) return null;
            _context.Entry(accommodation).CurrentValues.SetValues(accommodationUpdate);
            foreach (var existingPriceDiff in accommodation.PriceDiffs)
            {
                if (!accommodationUpdate.PriceDiffs.Any(pd => pd.Id == existingPriceDiff.Id))
                    _context.Remove(existingPriceDiff);
            }
            foreach (var priceDiffUpdate in accommodationUpdate.PriceDiffs)
            {
                var existingPriceDiff = accommodation.PriceDiffs.SingleOrDefault(pd => pd.Id == priceDiffUpdate.Id);

                if (existingPriceDiff != null)
                    _context.Entry(existingPriceDiff).CurrentValues.SetValues(priceDiffUpdate);
                else
                    _context.Add(new PriceDiff
                    {
                        Percentage = priceDiffUpdate.Percentage,
                        When = priceDiffUpdate.When,
                        AccommodationId = accommodation.Id
                    });
            }
            return accommodation;
        }

        public override async Task<Accommodation> Delete(Guid id, CancellationToken cancellationToken = default)
        {
            var accommodation = await Read(id, cancellationToken);
            if (accommodation == null) return accommodation;
            RemoveFromContext(accommodation);
            return accommodation;
        }

        public async Task<IEnumerable<Accommodation>> ReadByIds(Guid[] ids, CancellationToken ct = default)
        {
            return await _entities
                .Where(accommodation => ids.Contains(accommodation.Id))
                .AsNoTracking()
                .ToListAsync(ct);
        }

        public async Task<IEnumerable<Accommodation>> ReadByQuery(SearchAccommodationQuery query, CancellationToken ct = default)
        {
            return await _entities
                 .AsNoTracking()
                 .Include(a => a.Location)
                 .Include(a => a.Availabilities)
                 .Include(a => a.PriceDiffs)
                 .Where(a => a.Location.Country.ToLower().Contains(query.Location.ToLower()) ||
                             a.Location.Region.ToLower().Contains(query.Location.ToLower()) ||
                             a.Location.City.ToLower().Contains(query.Location.ToLower()))
                 .Where(a => !query.GuestNum.HasValue || (a.MinGuests <= query.GuestNum && a.MaxGuests >= query.GuestNum))
                 .ToListAsync(ct);
        }

        public async Task<IEnumerable<Accommodation>> ReadByHostId(Guid hostId, CancellationToken ct = default)
        {
            return await _entities
                .AsNoTracking()
                .Include(a => a.Location)
                .Where(a => a.HostId == hostId)
                .ToListAsync(ct);
        }

        public async Task<IEnumerable<Accommodation>> DeleteByHostId(Guid hostId, CancellationToken ct = default)
        {
            var accommodations = await _entities
                .Include(a => a.Location)
                .Include(a => a.Amenities)
                .Include(a => a.Availabilities)
                .Include(a => a.PriceDiffs)
                .Where(a => a.HostId == hostId)
                .ToListAsync(ct);
            foreach (var accommodation in accommodations) RemoveFromContext(accommodation);
            return accommodations;
        }

        private void RemoveFromContext(Accommodation accommodation)
        {
            _entities.Remove(accommodation);
            _context.Remove(accommodation.Location);
            _context.RemoveRange(accommodation.Amenities);
            _context.RemoveRange(accommodation.Availabilities);
            _context.RemoveRange(accommodation.PriceDiffs);
        }
    }
}
