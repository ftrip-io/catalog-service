using ftrip.io.catalog_service.contracts.Accommodations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace ftrip.io.catalog_service.Accommodations
{
    public interface IBookingServiceClient
    {
        Task<IEnumerable<AccommodationOccupancy>> GetOccupancies(Guid accommodationId, DateTime periodFrom, DateTime periodTo);
        Task<IEnumerable<Guid>> CheckAvailability(List<Guid> accommodationIds, DateTime periodFrom, DateTime periodTo);
    }

    public class BookingServiceClient : IBookingServiceClient
    {
        private readonly HttpClient _httpClient;

        public BookingServiceClient(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("booking");
        }

        public async Task<IEnumerable<AccommodationOccupancy>> GetOccupancies(Guid accommodationId, DateTime periodFrom, DateTime periodTo)
        {
            var responseStr = await _httpClient.GetStringAsync(
                $"api/accommodation-occupancy?" +
                $"AccommodationId={accommodationId}&PeriodFrom={periodFrom:yyyy-MM-dd}&PeriodTo={periodTo:yyyy-MM-dd}"
            );

            return JsonSerializer.Deserialize<IEnumerable<AccommodationOccupancy>>(responseStr, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
        }

        public async Task<IEnumerable<Guid>> CheckAvailability(List<Guid> accommodationIds, DateTime periodFrom, DateTime periodTo)
        {
            var responseStr = await _httpClient.GetStringAsync(
            $"api/accommodation-occupancy/availability?" +
                $"{string.Join("&", accommodationIds.Select(aId => "AccommodationIds=" + aId))}&PeriodFrom={periodFrom:yyyy-MM-dd}&PeriodTo={periodTo:yyyy-MM-dd}"
            );

            return JsonSerializer.Deserialize<IEnumerable<Guid>>(responseStr, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
        }
    }
}
