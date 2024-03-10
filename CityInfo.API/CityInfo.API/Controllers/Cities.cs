using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [ApiController]
    [Route("api/cities")]
    public class Cities : ControllerBase
    {
        public Cities() { }

        [HttpGet]
        public JsonResult GetCities()
        {
            return new JsonResult(
                new List<object> {
                    new {id=1, name ="New York"},
                    new {id=1, name="Tokyo"}
                });
        }


    }
}
