using CityInfo.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [Route("api/cities/{cityId}/pointsofinterest")]
    [ApiController]
    public class PointsOfInterestController : ControllerBase
    {
        [HttpGet]
        public ActionResult<IEnumerable<PointOfInterestDto>> GetPointsOfInterest(int cityId)
        {
            var city= CitiesDataStore.Current.Cities.FirstOrDefault(c=>c.Id==cityId);

            if(city==null)
            {
                return NotFound();
            }

            return Ok(city.PointOfInterests);
        }

        [HttpGet("{pointOfInterestId}", Name ="GetPointOfInterest")]
        public ActionResult<PointOfInterestDto> GetPointOfInterest(int cityId, int pointOfInterestId)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c=>c.Id== cityId);
            
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
            var city= CitiesDataStore.Current.Cities.FirstOrDefault(c=>c.Id== cityId);

            if (city == null)
            {
                return NotFound();
            }

            var maxPoiId=CitiesDataStore.Current.Cities.SelectMany(c=>c.PointOfInterests).Max(p=>p.Id);

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
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

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

            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
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
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

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

            return NoContent();



        }
    }
}
