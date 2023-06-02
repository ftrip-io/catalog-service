using ftrip.io.catalog_service.contracts.Accommodations;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ftrip.io.catalog_service.Accommodations
{
    public interface IBookingServiceClient
    {
        Task<IEnumerable<AccommodationOccupancy>> GetOccupancies(Guid accommodationId, DateTime periodFrom, DateTime periodTo);
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
            return await _httpClient.GetFromJsonAsync<List<AccommodationOccupancy>>(
                $"api/accommodation-occupancy?" +
                $"AccommodationId={accommodationId}&PeriodFrom={periodFrom:yyyy-MM-dd}&PeriodTo={periodTo:yyyy-MM-dd}"
            );
        }

    }
}
