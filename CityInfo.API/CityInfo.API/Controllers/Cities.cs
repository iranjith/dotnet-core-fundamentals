using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [ApiController]

    public class Cities : ControllerBase
    {
        public Cities() { }

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
