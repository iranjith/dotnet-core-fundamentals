using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [Route("api/cities/{cityId}/pointsofinterest")]
    [ApiController]
    public class PointsOfInterestController : ControllerBase
    {


        private readonly ILogger<PointsOfInterestController> _logger;
        private readonly IMailService _mailService;
        private readonly CitiesDataStore _citiesDataStore;

        public PointsOfInterestController(ILogger<PointsOfInterestController> logger, IMailService mailService, CitiesDataStore citiesDataStore)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
            _citiesDataStore = citiesDataStore ?? throw new ArgumentNullException(nameof(citiesDataStore));
        }

        [HttpGet]
        public ActionResult<IEnumerable<PointOfInterestDto>> GetPointsOfInterest(int cityId)
        {
            try
            {
                var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);

                if (city == null)
                {
                    _logger.LogInformation($"City with id {cityId} wasn't found when accessing point of interest.");
                    return NotFound();
                }
                return Ok(city.PointOfInterests);

            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception while getting point of interest with {cityId}", ex);
                return StatusCode(500, "A problem while handling your request"); 
            }
            
            

        }

        [HttpGet("{pointOfInterestId}", Name ="GetPointOfInterest")]
        public ActionResult<PointOfInterestDto> GetPointOfInterest(int cityId, int pointOfInterestId)
        {
            var city = _citiesDataStore.Cities.FirstOrDefault(c=>c.Id== cityId);
            
            if(city == null)
            {
                return NotFound();
            }

            var pointOfInterest= city.PointOfInterests.FirstOrDefault(c=>c.Id== pointOfInterestId);

            if(pointOfInterest==null)
            {
                return NotFound();
            }

            return Ok(pointOfInterest);
        }

        [HttpPost]
        public ActionResult<PointOfInterestDto> CreatePointOfInterest(int cityId, PointOfInterestForCreationDto pointOfInterestForCreation)
        {
            var city= _citiesDataStore.Cities.FirstOrDefault(c=>c.Id== cityId);

            if (city == null)
            {
                return NotFound();
            }

            var maxPoiId=_citiesDataStore.Cities.SelectMany(c=>c.PointOfInterests).Max(p=>p.Id);

            var finalPointOfInterest = new PointOfInterestDto()
            {
                Id = ++maxPoiId,
                Name = pointOfInterestForCreation.Name,
                Description = pointOfInterestForCreation.Description
            };

            city.PointOfInterests.Add(finalPointOfInterest);

            return CreatedAtRoute("GetPointOfInterest",
                new
                {
                    cityId = cityId,
                    pointOfInterestId = finalPointOfInterest.Id,
                }, finalPointOfInterest);
        }

        [HttpPut("{pointOfInterestId}")]
        public ActionResult UpdatePointOfInterest(int cityId, int pointOfInterestId, PointOfInterestForUpdateDto pointOfInterest)
        {
            var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);

            if (city == null)
            {
                return NotFound();
            }


            //find pointOfInterest

            var pointOfInterestfromStore= city.PointOfInterests.FirstOrDefault(c=>c.Id== pointOfInterestId);
            if (pointOfInterestfromStore == null)
            {
                return NotFound();
            }

            pointOfInterestfromStore.Name = pointOfInterest.Name;
            pointOfInterestfromStore.Description = pointOfInterest.Description;

            return NoContent();
        }

        [HttpPatch("{pointOfInterestId}")]
        public ActionResult PartiallyUpdatePointOfInterest(int cityId, int pointOfInterestId, JsonPatchDocument<PointOfInterestForUpdateDto> patchDocument)
        {

            var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }


            //find pointOfInterest

            var pointOfInterestfromStore = city.PointOfInterests.FirstOrDefault(c => c.Id == pointOfInterestId);
            if (pointOfInterestfromStore == null)
            {
                return NotFound();
            }

            var pointOfInterestPatch = new PointOfInterestForUpdateDto(){
                Name = pointOfInterestfromStore.Name,
                Description = pointOfInterestfromStore.Description
            };

            patchDocument.ApplyTo(pointOfInterestPatch, ModelState);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!TryValidateModel(pointOfInterestPatch))
            {
                return BadRequest(ModelState);
            }

            pointOfInterestfromStore.Name = pointOfInterestPatch.Name;
            pointOfInterestfromStore.Description = pointOfInterestPatch.Description;

            return NoContent();



        }


        [HttpDelete("{pointOfInterestId}")]
        public ActionResult DeletePointOfInterest(int cityId, int pointOfInterestId)
        {
            var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);

            if (city == null)
            {
                return NotFound();
            }


            //find pointOfInterest

            var pointOfInterestfromStore = city.PointOfInterests.FirstOrDefault(c => c.Id == pointOfInterestId);
            if (pointOfInterestfromStore == null)
            {
                return NotFound();
            }

            city.PointOfInterests.Remove(pointOfInterestfromStore);
            _mailService.Send("Point of interest deleted", $"point of interest {pointOfInterestfromStore.Name} with id {pointOfInterestfromStore.Id}");

            return NoContent();
        }
    }
}
