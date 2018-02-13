using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Controllers
{
    [Route("api/cities")]
    public class CitiesController : Controller
    {
        private ICityInfoRepository _cityInfoRepository;

        public CitiesController(ICityInfoRepository cityInfoRepository)
        {
            _cityInfoRepository = cityInfoRepository;
        }

        [HttpGet()]
        public IActionResult GetCities()
        {
            var cityEntities = _cityInfoRepository.GetCities();

            //LD STEP35
            var results = Mapper.Map<IEnumerable<CityWithoutPointsOfInterestDto>>(cityEntities); 

            return Ok(results);
        }

        //LD STEP8
        [HttpGet("{id}")]
        public IActionResult GetCity(int id, bool includePointsOfInterest = false)
        {
            var city = _cityInfoRepository.GetCity(id, includePointsOfInterest);

            if (city == null)
            {
                return NotFound();
            }

            if (includePointsOfInterest)
            {
                var cityResult = Mapper.Map<CityDto>(city); 
                return Ok(cityResult);
            }

            var cityWithoutPointsOfInterestResult = Mapper.Map<CityWithoutPointsOfInterestDto>(city);
            return Ok(cityWithoutPointsOfInterestResult);
        }

        #region luca
        
        //LD the postman call: http://localhost:1029/api/cities/ld
        [HttpGet("ld")]
        public JsonResult GetCitiesLD()
        {

            //    //LD OPTION 1
            //    return new JsonResult(new List<object>() {
            //    new { id=1, Name="obj1"},
            //    new { id=1, Name="obj2"}
            //    });

            //LD OPTION 2
            return new JsonResult(CitiesDataStore.Current.Cities);
        }

        [HttpGet("ld/{id}")]
        public IActionResult GetCityLD(int id)
        {
            var cityLd = CitiesDataStore.Current.Cities.FirstOrDefault(city => city.Id == id);

            //LD STEP8 we return a friendly "NotFound"
            if (cityLd == null)
            {
                return NotFound();
            }
            return Ok(cityLd);
            
        }
        #endregion
    }
}
