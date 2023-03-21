using Arduino.Core.Models;
using Geocoding.MapQuest;

namespace Arduino.Core.Services
{
    public class MapQuestGeocodeService : IGeocodeService
    {
        private readonly string apiKey;

        public MapQuestGeocodeService(string apiKey)
        {
            this.apiKey = apiKey;
        }

        public async Task<LocationResponse?> GetLocationForAddress(string address)
        {
            if (string.IsNullOrEmpty(address))
            {
                throw new ArgumentException($"'{nameof(address)}' cannot be null or empty.", nameof(address));
            }

            var geocoder = new MapQuestGeocoder(apiKey);

            var locations = await geocoder.GeocodeAsync(address);
            var latitude = locations.First().Coordinates.Latitude;
            var longitude = locations.First().Coordinates.Longitude;

            return new LocationResponse 
            {
                Latitude = latitude,
                Longitude = longitude
            };
        }
    }
}
