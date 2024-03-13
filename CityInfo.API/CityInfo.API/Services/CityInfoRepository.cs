using CityInfo.API.DbContexts;
using CityInfo.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.Services
{
    public class CityInfoRepository : ICityInfoRepository
    {
        private readonly CityInfoContext _cityInfoContext;
        public CityInfoRepository(CityInfoContext cityInfoContext) {
            _cityInfoContext = cityInfoContext ?? throw new ArgumentNullException(nameof(cityInfoContext));
        }

        async Task<IEnumerable<City>> ICityInfoRepository.GetCitiesAsync()
        {
            return await _cityInfoContext.Cities.OrderBy(c=>c.Name).ToListAsync();
        }

        async Task<City?> ICityInfoRepository.GetCityAsync(int cityId, bool includePointOfInterest)
        {
            if (includePointOfInterest)
            {
                return await _cityInfoContext.Cities.Include(c => c.PointOfInterests).Where(c=>c.Id == cityId).FirstOrDefaultAsync();
            }
            return await _cityInfoContext.Cities.Where(c => c.Id == cityId).FirstOrDefaultAsync();

        }

        async Task<IEnumerable<PointOfInterest>> ICityInfoRepository.GetPointsOfInterestForCityAsync(int cityId)
        {
            return await _cityInfoContext.PointOfInterests.ToListAsync();
        }
         
        async Task<PointOfInterest?> ICityInfoRepository.GetPointOfInterestForCityAsync(int cityId, int pointOfInterestId)
        {
            return await _cityInfoContext.PointOfInterests.Where(p => p.CityId == cityId && p.Id == pointOfInterestId).FirstOrDefaultAsync();

        }

        async Task<bool> ICityInfoRepository.CityExistsAsync(int cityId)
        {
            return await _cityInfoContext.Cities.AnyAsync(c => c.Id == cityId);
        }
    }
}
