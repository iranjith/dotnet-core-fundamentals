using AutoMapper;
using CityInfo.API.Entities;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [Route("api/cities/{cityId}/pointsofinterest")]
    [Authorize(Policy = "MustBeFromBangalore")]
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
            var cityName=  User.Claims.FirstOrDefault(c=>c.Type == "city")?.Value;

            if(!await _cityInfoRepo.CityNameMatchesCityId(cityName, cityId))
            {
                return Forbid();
            }


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

        [HttpPost]
        public async Task<ActionResult<PointOfInterestDto>> CreatePointOfInterest(int cityId, PointOfInterestForCreationDto pointOfInterestForCreation)
        {
            if (!await _cityInfoRepo.CityExistsAsync(cityId))
            {
                _logger.LogInformation($"City with id {cityId} wasn't found when accessing point of interest.");
                return NotFound();
            }


            var finalPointOfInterest = _mapper.Map<PointOfInterest>(pointOfInterestForCreation);

            await _cityInfoRepo.AddPointOfInterestForCityAsync(cityId, finalPointOfInterest);

            await _cityInfoRepo.SaveChangesAsync();

            var createdPointOfInterestToReturn = _mapper.Map<PointOfInterestDto>(finalPointOfInterest);


            return CreatedAtRoute("GetPointOfInterest",
                new
                {
                    cityId = cityId,
                    pointOfInterestId = createdPointOfInterestToReturn.Id,
                }, createdPointOfInterestToReturn);
        }

        [HttpPut("{pointOfInterestId}")]
        public async Task<ActionResult> UpdatePointOfInterest(int cityId, int pointOfInterestId, PointOfInterestForUpdateDto updatedPointOfInterest)
        {
            if (!await _cityInfoRepo.CityExistsAsync(cityId))
            {
                _logger.LogInformation($"City with id {cityId} wasn't found when accessing point of interest.");
                return NotFound();
            }


            //find pointOfInterest
            var pointOfInterestEntity =await _cityInfoRepo.GetPointOfInterestForCityAsync(cityId, pointOfInterestId);
            if (pointOfInterestEntity == null)
            {
                return NotFound();
            }

            //useful for update values for an entity class values
            _mapper.Map(updatedPointOfInterest, pointOfInterestEntity);

            await _cityInfoRepo.SaveChangesAsync(); 
            
            return NoContent();
        }

        [HttpPatch("{pointOfInterestId}")]
        public async Task<ActionResult> PartiallyUpdatePointOfInterest(int cityId, int pointOfInterestId, JsonPatchDocument<PointOfInterestForUpdateDto> patchDocument)
        {

            if (!await _cityInfoRepo.CityExistsAsync(cityId))
            {
                _logger.LogInformation($"City with id {cityId} wasn't found when accessing point of interest.");
                return NotFound();
            }


            //find pointOfInterest
            var pointOfInterestEntity = await _cityInfoRepo
                .GetPointOfInterestForCityAsync(cityId, pointOfInterestId);
            if (pointOfInterestEntity == null)
            {
                return NotFound();
            }

            var pointOfInterestPatch = _mapper.Map<PointOfInterestForUpdateDto>(pointOfInterestEntity);

            patchDocument.ApplyTo(pointOfInterestPatch, ModelState);
            
            if ( !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!TryValidateModel(pointOfInterestPatch))
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(pointOfInterestPatch, pointOfInterestEntity);

            await _cityInfoRepo.SaveChangesAsync();

            return NoContent();

        }


        [HttpDelete("{pointOfInterestId}")]
        public async Task<ActionResult> DeletePointOfInterest(int cityId, int pointOfInterestId)
        {
            if (!await _cityInfoRepo.CityExistsAsync(cityId))
            {
                _logger.LogInformation($"City with id {cityId} wasn't found when accessing point of interest.");
                return NotFound();
            }



            //find pointOfInterest

            var pointOfInterestEntity = await _cityInfoRepo
                 .GetPointOfInterestForCityAsync(cityId, pointOfInterestId);
            if (pointOfInterestEntity == null)
            {
                return NotFound();
            }

            _cityInfoRepo.DeletePointOfInterest(pointOfInterestEntity);
            await _cityInfoRepo.SaveChangesAsync();

            _mailService.Send("Point of interest deleted", $"point of interest {pointOfInterestEntity.Name} with id {pointOfInterestEntity.Id}");

            return NoContent();
        }
    }
}
