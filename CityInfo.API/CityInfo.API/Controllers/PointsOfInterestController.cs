using AutoMapper;
using CityInfo.API.Entities;
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
        private readonly ICityInfoRepository _cityInfoRepo;
        private readonly IMapper _mapper;

        public PointsOfInterestController(ILogger<PointsOfInterestController> logger, IMailService mailService, 
                ICityInfoRepository cityRepo, IMapper mapper)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
            _cityInfoRepo= cityRepo ?? throw new ArgumentNullException(nameof(cityRepo));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PointOfInterestDto>>> GetPointsOfInterest(int cityId)
        {
            if(!await _cityInfoRepo.CityExistsAsync(cityId))
            {
                _logger.LogInformation($"City with id {cityId} wasn't found when accessing point of interest.");
                return NotFound();
            }
            var pointOfInterestForCity = await _cityInfoRepo.GetPointsOfInterestForCityAsync(cityId);

            return Ok(_mapper.Map<IEnumerable<PointOfInterestDto>>(pointOfInterestForCity));
        }

        [HttpGet("{pointOfInterestId}", Name ="GetPointOfInterest")]
        public async Task<ActionResult<PointOfInterestDto>> GetPointOfInterest(int cityId, int pointOfInterestId)
        {

            if (!await _cityInfoRepo.CityExistsAsync(cityId))
            {
                _logger.LogInformation($"City with id {cityId} wasn't found when accessing point of interest.");
                return NotFound();
            }

            var pointOfInterest=_cityInfoRepo.GetPointOfInterestForCityAsync(cityId, pointOfInterestId);
            if(pointOfInterest == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<PointOfInterestDto>(pointOfInterest));
        }

        //[HttpPost]
        //public ActionResult<PointOfInterestDto> CreatePointOfInterest(int cityId, PointOfInterestForCreationDto pointOfInterestForCreation)
        //{
        //    var city= _citiesDataStore.Cities.FirstOrDefault(c=>c.Id== cityId);

        //    if (city == null)
        //    {
        //        return NotFound();
        //    }

        //    var maxPoiId=_citiesDataStore.Cities.SelectMany(c=>c.PointOfInterests).Max(p=>p.Id);

        //    var finalPointOfInterest = new PointOfInterestDto()
        //    {
        //        Id = ++maxPoiId,
        //        Name = pointOfInterestForCreation.Name,
        //        Description = pointOfInterestForCreation.Description
        //    };

        //    city.PointOfInterests.Add(finalPointOfInterest);

        //    return CreatedAtRoute("GetPointOfInterest",
        //        new
        //        {
        //            cityId = cityId,
        //            pointOfInterestId = finalPointOfInterest.Id,
        //        }, finalPointOfInterest);
        //}

        //[HttpPut("{pointOfInterestId}")]
        //public ActionResult UpdatePointOfInterest(int cityId, int pointOfInterestId, PointOfInterestForUpdateDto pointOfInterest)
        //{
        //    var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);

        //    if (city == null)
        //    {
        //        return NotFound();
        //    }


        //    //find pointOfInterest

        //    var pointOfInterestfromStore= city.PointOfInterests.FirstOrDefault(c=>c.Id== pointOfInterestId);
        //    if (pointOfInterestfromStore == null)
        //    {
        //        return NotFound();
        //    }

        //    pointOfInterestfromStore.Name = pointOfInterest.Name;
        //    pointOfInterestfromStore.Description = pointOfInterest.Description;

        //    return NoContent();
        //}

        //[HttpPatch("{pointOfInterestId}")]
        //public ActionResult PartiallyUpdatePointOfInterest(int cityId, int pointOfInterestId, JsonPatchDocument<PointOfInterestForUpdateDto> patchDocument)
        //{

        //    var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
        //    if (city == null)
        //    {
        //        return NotFound();
        //    }


        //    //find pointOfInterest

        //    var pointOfInterestfromStore = city.PointOfInterests.FirstOrDefault(c => c.Id == pointOfInterestId);
        //    if (pointOfInterestfromStore == null)
        //    {
        //        return NotFound();
        //    }

        //    var pointOfInterestPatch = new PointOfInterestForUpdateDto(){
        //        Name = pointOfInterestfromStore.Name,
        //        Description = pointOfInterestfromStore.Description
        //    };

        //    patchDocument.ApplyTo(pointOfInterestPatch, ModelState);
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    if (!TryValidateModel(pointOfInterestPatch))
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    pointOfInterestfromStore.Name = pointOfInterestPatch.Name;
        //    pointOfInterestfromStore.Description = pointOfInterestPatch.Description;

        //    return NoContent();



        //}


        //[HttpDelete("{pointOfInterestId}")]
        //public ActionResult DeletePointOfInterest(int cityId, int pointOfInterestId)
        //{
        //    var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);

        //    if (city == null)
        //    {
        //        return NotFound();
        //    }


        //    //find pointOfInterest

        //    var pointOfInterestfromStore = city.PointOfInterests.FirstOrDefault(c => c.Id == pointOfInterestId);
        //    if (pointOfInterestfromStore == null)
        //    {
        //        return NotFound();
        //    }

        //    city.PointOfInterests.Remove(pointOfInterestfromStore);
        //    _mailService.Send("Point of interest deleted", $"point of interest {pointOfInterestfromStore.Name} with id {pointOfInterestfromStore.Id}");

        //    return NoContent();
        //}
    }
}
