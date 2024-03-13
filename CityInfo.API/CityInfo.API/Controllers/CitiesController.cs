using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [ApiController]
    [Route("api/cities")]
    public class CitiesController : ControllerBase
    {
        private readonly ICityInfoRepository _cityInfoRepository;
        private readonly IMapper _mapper;

        public CitiesController(ICityInfoRepository cityInfoRepository, IMapper mapper)
        {
            _cityInfoRepository= cityInfoRepository ?? throw new Exception(nameof(cityInfoRepository));
            _mapper = mapper ?? throw new Exception(nameof(IMapper));
        }


        [HttpGet]
        public async Task<ActionResult<List<CityDto>>> GetCities()
        {

            var cityEntites = await _cityInfoRepository.GetCitiesAsync();
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
