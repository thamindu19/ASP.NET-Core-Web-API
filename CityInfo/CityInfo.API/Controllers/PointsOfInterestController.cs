// using CityInfo.API.Models;
// using CityInfo.API.Services;
// using Microsoft.AspNetCore.Mvc;

// namespace CityInfo.API.Controllers
// {
//     [Route("api/cities/{cityId}/pointsofinterest")]
//     [ApiController]
//     public class PointsOfInterestController : ControllerBase
//     {
//         private readonly ILogger<PointsOfInterestController> _logger;
//         private readonly IMailService _mailService;
//         private readonly CitiesDataStore _citiesDataStore;

//         public PointsOfInterestController(ILogger<PointsOfInterestController> logger, IMailService mailService, CitiesDataStore citiesDataStore)
//         {
//             _logger = logger ?? throw new ArgumentNullException(nameof(logger));
//             _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
//             _citiesDataStore = citiesDataStore ?? throw new ArgumentNullException(nameof(citiesDataStore));
//         }

//         [HttpGet]
//         public ActionResult<IEnumerable<PointOfInterestDto>> GetPointsOfInterest(int cityId)
//         {
//             try
//             {
//                 // throw new Exception("Exception sample.");

//                 var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);

//                 if (city == null)
//                 {
//                     _logger.LogInformation($"City with id {cityId} wasn't found when accessing points of interest.");
//                     return NotFound();
//                 }

//                 return Ok(city.PointsOfInterest);
//             }
//             catch (Exception ex)
//             {
//                 _logger.LogCritical(
//                                    $"Exception while getting points of interest for city with id {cityId}.",
//                                    ex);
//                 return StatusCode(500, "A problem happened while handling your request.");
//             }
//         }

//         [HttpGet("{pointofinterestid}", Name = "GetPointOfInterest")]
//         public ActionResult<PointOfInterestDto> GetPointOfInterest(int cityId, int pointOfInterestId)
//         {
//             var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);

//             if (city == null)
//             {
//                 return NotFound();
//             }

//             var pointOfInterest = city.PointsOfInterest.FirstOrDefault(p => p.Id == pointOfInterestId);

//             if (pointOfInterest == null)
//             {
//                 return NotFound();
//             }

//             return Ok(pointOfInterest);
//         }

//         [HttpPost]
//         public ActionResult<PointOfInterestDto> CreatePointOfInterest(
//            int cityId,
//            PointOfInterestForCreationDto pointOfInterest)
//         {
//             // if (!ModelState.IsValid)
//             // {
//             //     return BadRequest();
//             // }

//             var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);

//             if (city == null)
//             {
//                 return NotFound();
//             }

//             // demo purposes - to be improved
//             var maxPointOfInterestId = _citiesDataStore.Cities.SelectMany(
//                              c => c.PointsOfInterest).Max(p => p.Id);

//             var finalPointOfInterest = new PointOfInterestDto()
//             {
//                 Id = ++maxPointOfInterestId,
//                 Name = pointOfInterest.Name,
//                 Description = pointOfInterest.Description
//             };

//             city.PointsOfInterest.Add(finalPointOfInterest);

//             return CreatedAtRoute("GetPointOfInterest",
//                  new
//                  {
//                      cityId = cityId,
//                      pointOfInterestId = finalPointOfInterest.Id
//                  },
//                  finalPointOfInterest);
//         }

//         // [HttpPatch("{pointofinterestid}")]
//         // public ActionResult PartiallyUpdatePointOfInterest(
//         //     int cityId, int pointOfInterestId,
//         //     JsonPatchDocument<PointOfInterestForUpdateDto> patchDocument)
//         // {
//         //     var city = _citiesDataStore.Cities
//         //         .FirstOrDefault(c => c.Id == cityId);
//         //     if (city == null)
//         //     {
//         //         return NotFound();
//         //     }

//         //     var pointOfInterestFromStore = city.PointsOfInterest
//         //         .FirstOrDefault(c => c.Id == pointOfInterestId);
//         //     if (pointOfInterestFromStore == null)
//         //     {
//         //         return NotFound();
//         //     }

//         //     var pointOfInterestToPatch =
//         //            new PointOfInterestForUpdateDto()
//         //            {
//         //                Name = pointOfInterestFromStore.Name,
//         //                Description = pointOfInterestFromStore.Description
//         //            };

//         //     patchDocument.ApplyTo(pointOfInterestToPatch, ModelState);

//         //     if (!ModelState.IsValid)
//         //     {
//         //         return BadRequest(ModelState);
//         //     }

//         //     if (!TryValidateModel(pointOfInterestToPatch))
//         //     {
//         //         return BadRequest(ModelState);
//         //     }

//         //     pointOfInterestFromStore.Name = pointOfInterestToPatch.Name;
//         //     pointOfInterestFromStore.Description = pointOfInterestToPatch.Description;

//         //     return NoContent();
//         // }

//         [HttpDelete("{pointOfInterestId}")]
//         public ActionResult DeletePointOfInterest(int cityId, int pointOfInterestId)
//         {
//             var city = _citiesDataStore.Cities
//                 .FirstOrDefault(c => c.Id == cityId);
//             if (city == null)
//             {
//                 return NotFound();
//             }

//             var pointOfInterestFromStore = city.PointsOfInterest
//                 .FirstOrDefault(c => c.Id == pointOfInterestId);
//             if (pointOfInterestFromStore == null)
//             {
//                 return NotFound();
//             }

//             city.PointsOfInterest.Remove(pointOfInterestFromStore);
//             _mailService.Send(
//                 "Point of interest deleted.",
//                 $"Point of interest {pointOfInterestFromStore.Name} with id {pointOfInterestFromStore.Id} was deleted.");
//             return NoContent();
//         }
//     }
// }

using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [Route("api/cities/{cityId}/pointsofinterest")]
    [Authorize("MustBeFromAntwerp")]
    [ApiController]
    public class PointsOfInterestController : ControllerBase
    {
        private readonly ILogger<PointsOfInterestController> _logger;
        private readonly IMailService _mailService;
        private readonly ICityInfoRepository _cityInfoRepository;
        private readonly IMapper _mapper;

        public PointsOfInterestController(ILogger<PointsOfInterestController> logger,
            IMailService mailService,
            ICityInfoRepository cityInfoRepository,
            IMapper mapper)
        {
            _logger = logger ??
                throw new ArgumentNullException(nameof(logger));
            _mailService = mailService ??
                throw new ArgumentNullException(nameof(mailService));
            _cityInfoRepository = cityInfoRepository ??
                throw new ArgumentNullException(nameof(cityInfoRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PointOfInterestDto>>> GetPointsOfInterest(
            int cityId)
        {
            var cityName = User.Claims.FirstOrDefault(c => c.Type == "city")?.Value;

            if (!await _cityInfoRepository.CityNameMatchesCityId(cityName, cityId))
            {
                return Forbid();
            }

            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                _logger.LogInformation(
                    $"City with id {cityId} wasn't found when accessing points of interest.");
                return NotFound();
            }

            var pointsOfInterestForCity = await _cityInfoRepository
                .GetPointsOfInterestForCityAsync(cityId);

            return Ok(_mapper.Map<IEnumerable<PointOfInterestDto>>(pointsOfInterestForCity));
        }

        [HttpGet("{pointofinterestid}", Name = "GetPointOfInterest")]
        public async Task<ActionResult<PointOfInterestDto>> GetPointOfInterest(
            int cityId, int pointOfInterestId)
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                return NotFound();
            }

            var pointOfInterest = await _cityInfoRepository
                .GetPointOfInterestForCityAsync(cityId, pointOfInterestId);

            if (pointOfInterest == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<PointOfInterestDto>(pointOfInterest));
        }

        [HttpPost]
        public async Task<ActionResult<PointOfInterestDto>> CreatePointOfInterest(
           int cityId,
           PointOfInterestForCreationDto pointOfInterest)
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                return NotFound();
            }

            var finalPointOfInterest = _mapper.Map<Entities.PointOfInterest>(pointOfInterest);

            await _cityInfoRepository.AddPointOfInterestForCityAsync(
                cityId, finalPointOfInterest);

            await _cityInfoRepository.SaveChangesAsync();

            var createdPointOfInterestToReturn =
                _mapper.Map<Models.PointOfInterestDto>(finalPointOfInterest);

            return CreatedAtRoute("GetPointOfInterest",
                 new
                 {
                     cityId = cityId,
                     pointOfInterestId = createdPointOfInterestToReturn.Id
                 },
                 createdPointOfInterestToReturn);
        }

        [HttpPut("{pointofinterestid}")]
        public async Task<ActionResult> UpdatePointOfInterest(int cityId, int pointOfInterestId,
            PointOfInterestForUpdateDto pointOfInterest)
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                return NotFound();
            }

            var pointOfInterestEntity = await _cityInfoRepository
                .GetPointOfInterestForCityAsync(cityId, pointOfInterestId);
            if (pointOfInterestEntity == null)
            {
                return NotFound();
            }

            _mapper.Map(pointOfInterest, pointOfInterestEntity);

            await _cityInfoRepository.SaveChangesAsync();

            return NoContent();
        }


        [HttpPatch("{pointofinterestid}")]
        public async Task<ActionResult> PartiallyUpdatePointOfInterest(
            int cityId, int pointOfInterestId,
            JsonPatchDocument<PointOfInterestForUpdateDto> patchDocument)
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                return NotFound();
            }

            var pointOfInterestEntity = await _cityInfoRepository
                .GetPointOfInterestForCityAsync(cityId, pointOfInterestId);
            if (pointOfInterestEntity == null)
            {
                return NotFound();
            }

            var pointOfInterestToPatch = _mapper.Map<PointOfInterestForUpdateDto>(
                pointOfInterestEntity);

            // patchDocument.ApplyTo(pointOfInterestToPatch, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!TryValidateModel(pointOfInterestToPatch))
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(pointOfInterestToPatch, pointOfInterestEntity);
            await _cityInfoRepository.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{pointOfInterestId}")]
        public async Task<ActionResult> DeletePointOfInterest(
            int cityId, int pointOfInterestId)
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                return NotFound();
            }

            var pointOfInterestEntity = await _cityInfoRepository
                .GetPointOfInterestForCityAsync(cityId, pointOfInterestId);
            if (pointOfInterestEntity == null)
            {
                return NotFound();
            }

            // _cityInfoRepository.DeletePointOfInterest(pointOfInterestEntity);
            await _cityInfoRepository.SaveChangesAsync();

            _mailService.Send(
                "Point of interest deleted.",
                $"Point of interest {pointOfInterestEntity.Name} with id {pointOfInterestEntity.Id} was deleted.");

            return NoContent();
        }

    }
}
