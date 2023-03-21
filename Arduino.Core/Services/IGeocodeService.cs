using Arduino.Core.Models;

namespace Arduino.Core.Services
{
    public interface IGeocodeService
    {
        Task<LocationResponse?> GetLocationForAddress(string address);
    }
}