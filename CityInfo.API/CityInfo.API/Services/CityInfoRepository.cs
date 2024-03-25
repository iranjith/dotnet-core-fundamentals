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

        public async Task<IEnumerable<City>> GetCitiesAsync()
        {
            return await _cityInfoContext.Cities.OrderBy(c=>c.Name).ToListAsync();
        }


        public async Task<(IEnumerable<City>, PaginationMetadata)> GetCitiesAsync(string? name, string? searchQuery, int pageNumber, int pageSize)
        {
            var collection = _cityInfoContext.Cities as IQueryable<City>;

            if (!string.IsNullOrEmpty(name))
            {
                name = name.Trim();
                collection=collection.Where(c=>c.Name==name).OrderBy(c=>c.Name);
            }

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                searchQuery = searchQuery.Trim();
                collection=collection.Where(c=>c.Name.Contains(searchQuery) || (c.Description!=null && c.Description.Contains(searchQuery)));   
            }
            var totalItemCount = await collection.CountAsync();
            var paginationMetadata = new PaginationMetadata(totalItemCount, pageSize, pageNumber);

            var collectionToReturn= await collection.OrderBy(c=>c.Name).Skip(pageSize*(pageNumber-1)).Take(pageSize).ToListAsync();
            return (collectionToReturn, paginationMetadata);

        }



        public async Task<City?> GetCityAsync(int cityId, bool includePointOfInterest)
        {
            if (includePointOfInterest)
            {
                return await _cityInfoContext.Cities.Include(c => c.PointOfInterests).Where(c=>c.Id == cityId).FirstOrDefaultAsync();
            }
            return await _cityInfoContext.Cities.Where(c => c.Id == cityId).FirstOrDefaultAsync();

        }

        public async Task<IEnumerable<PointOfInterest>> GetPointsOfInterestForCityAsync(int cityId)
        {
            return await _cityInfoContext.PointOfInterests.ToListAsync();
        }
         
        public async Task<PointOfInterest?> GetPointOfInterestForCityAsync(int cityId, int pointOfInterestId)
        {
            return await _cityInfoContext.PointOfInterests.Where(p => p.CityId == cityId && p.Id == pointOfInterestId).FirstOrDefaultAsync();

        }

        public async Task<bool> CityExistsAsync(int cityId)
        {
            return await _cityInfoContext.Cities.AnyAsync(c => c.Id == cityId);
        }

        public async Task AddPointOfInterestForCityAsync(int cityId, PointOfInterest pointOfInterest)
        {
            var city = await GetCityAsync(cityId,false);
            if (city != null)
            {
                city.PointOfInterests.Add(pointOfInterest);
            }
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _cityInfoContext.SaveChangesAsync()>=0);

        }

        public void DeletePointOfInterest(PointOfInterest pointOfInterest)
        {
            _cityInfoContext.PointOfInterests.Remove(pointOfInterest);
        }

        public async Task<bool> CityNameMatchesCityId(string? cityName, int cityId)
        {
             return await _cityInfoContext.Cities.AnyAsync(c => c.Id == cityId && c.Name == cityName);
        }
    }
}
