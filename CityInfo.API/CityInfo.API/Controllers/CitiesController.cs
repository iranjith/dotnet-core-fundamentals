using CityInfo.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [ApiController]
    [Route("api/cities")]
    public class CitiesController : ControllerBase
    {
        public CitiesController() { }

        [HttpGet]
        public ActionResult<List<CityDto>> GetCities()
        {
            return Ok(CitiesDataStore.Current.Cities);
        }

        [HttpGet("{id}")]
        public ActionResult<CityDto> GetCity(int id)
        {
            var cityToReturn = CitiesDataStore.Current.Cities.FirstOrDefault(x => x.Id == id);
            if(cityToReturn==null)
            {
                return NotFound();
            }
            return Ok(cityToReturn);
        }


    }
}
