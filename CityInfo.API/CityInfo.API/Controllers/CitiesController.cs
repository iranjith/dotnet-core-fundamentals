using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CityInfo.API.Controllers
{
    [ApiController]
    [Route("api/cities")]
    public class CitiesController : ControllerBase
    {
        private readonly ICityInfoRepository _cityInfoRepository;
        private readonly IMapper _mapper;
        const int maxPageSize = 20;

        public CitiesController(ICityInfoRepository cityInfoRepository, IMapper mapper)
        {
            _cityInfoRepository= cityInfoRepository ?? throw new Exception(nameof(cityInfoRepository));
            _mapper = mapper ?? throw new Exception(nameof(IMapper));
        }


        [HttpGet]
        public async Task<ActionResult<List<CityWithOutPointOfInterestDto>>> GetCities(string? name, string? searchQuery, int pageNumber=1, int pageSize=10)
        {
            if (pageSize > maxPageSize)
            {
                pageSize = maxPageSize;
            }


            var (cityEntites, paginationMetadata) = await _cityInfoRepository.GetCitiesAsync(name, searchQuery, pageNumber, pageSize);
            Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(paginationMetadata));
            return Ok(_mapper.Map<IEnumerable<CityWithOutPointOfInterestDto>>(cityEntites));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCity(int id, bool includePointOfInterest=false)
        {

            var city = await _cityInfoRepository.GetCityAsync(id,includePointOfInterest);

            if (city == null)
            {
                return NotFound();
            }
            if (includePointOfInterest)
            {
                return Ok(_mapper.Map<CityDto>(city));
            }

            return Ok(_mapper.Map<CityWithOutPointOfInterestDto>(city));

            //var cityToReturn = _citiesDataStore.Cities.FirstOrDefault(x => x.Id == id);
            //if(cityToReturn==null)
            //{
            //    return NotFound();
            //}
            //return Ok(cityToReturn);
        }


    }
}
