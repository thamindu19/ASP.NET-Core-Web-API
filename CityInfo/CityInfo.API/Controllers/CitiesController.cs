﻿// using Microsoft.AspNetCore.Mvc;

// namespace CityInfo.API.Controllers
// {
//     [ApiController]
//     [Route("api/cities")]
//     public class CitiesController : ControllerBase
//     {
//         [HttpGet]
//         public JsonResult GetCities()
//         {
//             return new JsonResult(_citiesDataStore.Cities);
//         }

//         [HttpGet("{id}")]
//         public JsonResult GetCity(int id)
//         {
//             return new JsonResult(_citiesDataStore.Cities.FirstOrDefault(c => c.Id == id));
//         }
//     }
// }


// using CityInfo.API.Models;
// using Microsoft.AspNetCore.Mvc;

// namespace CityInfo.API.Controllers
// {
//     [ApiController]
//     [Route("api/cities")]
//     public class CitiesController : ControllerBase
//     {
//         private readonly CitiesDataStore _citiesDataStore;

//         public CitiesController(CitiesDataStore citiesDataStore)
//         {
//             _citiesDataStore = citiesDataStore ?? throw new ArgumentNullException(nameof(citiesDataStore));
//         }

//         [HttpGet]
//         public ActionResult<IEnumerable<CityDto>> GetCities()
//         {
//             return Ok(_citiesDataStore.Cities);
//         }

//         [HttpGet("{id}")]
//         public ActionResult<CityDto> GetCity(int id)
//         {
//             var cityToReturn = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == id);

//             if (cityToReturn == null)
//             {
//                 return NotFound();
//             }

//             return Ok(cityToReturn);
//         }
//     }
// }


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

        public CitiesController(ICityInfoRepository cityInfoRepository,
            IMapper mapper)
        {
            _cityInfoRepository = cityInfoRepository ??
                throw new ArgumentNullException(nameof(cityInfoRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CityWithoutPointsOfInterestDto>>> GetCities()
        {
            var cityEntities = await _cityInfoRepository.GetCitiesAsync();
            return Ok(_mapper.Map<IEnumerable<CityWithoutPointsOfInterestDto>>(cityEntities));

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCity(
            int id, bool includePointsOfInterest = false)
        {
            var city = await _cityInfoRepository.GetCityAsync(id, includePointsOfInterest);
            if (city == null)
            {
                return NotFound();
            }

            if (includePointsOfInterest)
            {
                return Ok(_mapper.Map<CityDto>(city));
            }

            return Ok(_mapper.Map<CityWithoutPointsOfInterestDto>(city));
        }
    }
}
