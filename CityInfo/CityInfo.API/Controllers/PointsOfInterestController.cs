using CityInfo.API.Models;
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
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

            if (city == null)
            {
                return NotFound();
            }

            return Ok(city.PointsOfInterest);
        }

        [HttpGet("{pointofinterestid}", Name = "GetPointOfInterest")]
        public ActionResult<PointOfInterestDto> GetPointOfInterest(int cityId, int pointOfInterestId)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

            if (city == null)
            {
                return NotFound();
            }

            var pointOfInterest = city.PointsOfInterest.FirstOrDefault(p => p.Id == pointOfInterestId);

            if (pointOfInterest == null)
            {
                return NotFound();
            }

            return Ok(pointOfInterest);
        }

        [HttpPost]
        public ActionResult<PointOfInterestDto> CreatePointOfInterest(
           int cityId,
           PointOfInterestForCreationDto pointOfInterest)
        {
            // if (!ModelState.IsValid)
            // {
            //     return BadRequest();
            // }

            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

            if (city == null)
            {
                return NotFound();
            }

            // demo purposes - to be improved
            var maxPointOfInterestId = CitiesDataStore.Current.Cities.SelectMany(
                             c => c.PointsOfInterest).Max(p => p.Id);

            var finalPointOfInterest = new PointOfInterestDto()
            {
                Id = ++maxPointOfInterestId,
                Name = pointOfInterest.Name,
                Description = pointOfInterest.Description
            };

            city.PointsOfInterest.Add(finalPointOfInterest);

            return CreatedAtRoute("GetPointOfInterest",
                 new
                 {
                     cityId = cityId,
                     pointOfInterestId = finalPointOfInterest.Id
                 },
                 finalPointOfInterest);
        }

        // [HttpPatch("{pointofinterestid}")]
        // public ActionResult PartiallyUpdatePointOfInterest(
        //     int cityId, int pointOfInterestId,
        //     JsonPatchDocument<PointOfInterestForUpdateDto> patchDocument)
        // {
        //     var city = CitiesDataStore.Current.Cities
        //         .FirstOrDefault(c => c.Id == cityId);
        //     if (city == null)
        //     {
        //         return NotFound();
        //     }

        //     var pointOfInterestFromStore = city.PointsOfInterest
        //         .FirstOrDefault(c => c.Id == pointOfInterestId);
        //     if (pointOfInterestFromStore == null)
        //     {
        //         return NotFound();
        //     }

        //     var pointOfInterestToPatch =
        //            new PointOfInterestForUpdateDto()
        //            {
        //                Name = pointOfInterestFromStore.Name,
        //                Description = pointOfInterestFromStore.Description
        //            };

        //     patchDocument.ApplyTo(pointOfInterestToPatch, ModelState);

        //     if (!ModelState.IsValid)
        //     {
        //         return BadRequest(ModelState);
        //     }

        //     if (!TryValidateModel(pointOfInterestToPatch))
        //     {
        //         return BadRequest(ModelState);
        //     }

        //     pointOfInterestFromStore.Name = pointOfInterestToPatch.Name;
        //     pointOfInterestFromStore.Description = pointOfInterestToPatch.Description;

        //     return NoContent();
        // }

        [HttpDelete("{pointOfInterestId}")]
        public ActionResult DeletePointOfInterest(int cityId, int pointOfInterestId)
        {
            var city = CitiesDataStore.Current.Cities
                .FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }

            var pointOfInterestFromStore = city.PointsOfInterest
                .FirstOrDefault(c => c.Id == pointOfInterestId);
            if (pointOfInterestFromStore == null)
            {
                return NotFound();
            }

            city.PointsOfInterest.Remove(pointOfInterestFromStore);
            return NoContent();
        }
    }
}
