using System;
using System.Collections.Generic;

namespace ftrip.io.catalog_service.contracts.Accommodations
{
    public class PriceInfo
    {
        public decimal TotalPrice { get; set; }
        public ICollection<string> Problems { get; set; } = new List<string>();
        public int Days { get; set; }
        public ICollection<Item> Items { get; set; } = new List<Item>();
        public Guid AccommodationId { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int Guests { get; set; }
    }

    public class Item
    {
        public DateTime Date { get; set; }
        public decimal Price { get; set; }
        public float PriceDiffPercent { get; set; }
    }
}
