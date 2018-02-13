using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CityInfo.API.Controllers
{
    [Route("api/cities")]
    public class PointsOfInterestController : Controller
    {
        //LD STEP23
        private ILogger<PointsOfInterestController> _logger; //LD we add a field to hold the instance(in this case generic)       
        //LD STEP25
        private IMailService _mailService;

        private ICityInfoRepository _cityInfoRepository;


        public PointsOfInterestController(ILogger<PointsOfInterestController> logger, //LD a type is returned by the container
            IMailService mailService,//LD STEP25
            ICityInfoRepository cityInfoRepository)
        {
            _logger = logger;
            _mailService = mailService; //LD STEP25
            _cityInfoRepository = cityInfoRepository;
        }

        [HttpGet("{cityId}/pointsofinterest")]
        public IActionResult GetPointsOfInterest(int cityId)
        {
            try
            {
                if (!_cityInfoRepository.CityExists(cityId))
                {
                    //LD STEP23 - then we log
                    _logger.LogInformation($"City with id {cityId} wasn't found when accessing points of interest.");
                    return NotFound();
                }

                var pointsOfInterestForCity = _cityInfoRepository.GetPointsOfInterestForCity(cityId);
                var pointsOfInterestForCityResults =
                                   Mapper.Map<IEnumerable<PointOfInterestDto>>(pointsOfInterestForCity);

                return Ok(pointsOfInterestForCityResults);
            }
            catch (Exception ex)
            {

                _logger.LogCritical($"Exception while getting points of interest for city with id {cityId}.", ex);
                return StatusCode(500, "A problem happened while handling your request.");
            }
        }

        [HttpGet("{cityId}/pointsofinterest/{id}", Name = "GetPointOfInterest")]
        public IActionResult GetPointOfInterest(int cityId, int id)
        {
            if (!_cityInfoRepository.CityExists(cityId))
            {
                return NotFound();
            }

            var pointOfInterest = _cityInfoRepository.GetPointOfInterestForCity(cityId, id);

            if (pointOfInterest == null)
            {
                return NotFound();
            }

            var pointOfInterestResult = Mapper.Map<PointOfInterestDto>(pointOfInterest);
            return Ok(pointOfInterestResult);             
        }


        [HttpPost("{cityId}/pointsofinterest")]
        public IActionResult CreatePointOfInterest(int cityId, [FromBody] PointOfInterestForCreationDto pointOfInterest)
        {
            if (pointOfInterest == null)
            {
                return BadRequest();
            }

            if (pointOfInterest.Description == pointOfInterest.Name)
            {
                ModelState.AddModelError("Description", "The provided description should be different from the name.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_cityInfoRepository.CityExists(cityId))
            {
                return NotFound();
            }

            var finalPointOfInterest = Mapper.Map<Entities.PointOfInterest>(pointOfInterest);

            _cityInfoRepository.AddPointOfInterestForCity(cityId, finalPointOfInterest);

            if (!_cityInfoRepository.Save())
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }

            var createdPointOfInterestToReturn = Mapper.Map<Models.PointOfInterestDto>(finalPointOfInterest);

            return CreatedAtRoute("GetPointOfInterest", new{ cityId = cityId, id = createdPointOfInterestToReturn.Id }, createdPointOfInterestToReturn);
        }


        [HttpPut("{cityId}/pointsofinterest/{id}")]
        public IActionResult UpdatePointOfInterest(int cityId, int id,
            [FromBody] PointOfInterestForUpdateDto pointOfInterest)
        {
            if (pointOfInterest == null)
            {
                return BadRequest();
            }

            if (pointOfInterest.Description == pointOfInterest.Name)
            {
                ModelState.AddModelError("Description", "The provided description should be different from the name.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_cityInfoRepository.CityExists(cityId))
            {
                return NotFound();
            }

            var pointOfInterestEntity = _cityInfoRepository.GetPointOfInterestForCity(cityId, id);
            if (pointOfInterestEntity == null)
            {
                return NotFound();
            }

            Mapper.Map(pointOfInterest, pointOfInterestEntity);

            if (!_cityInfoRepository.Save())
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }

            return NoContent();
        }


        //LD STEP20
        [HttpPatch("{cityId}/pointsofinterest/{id}")]
        public IActionResult PartiallyUpdatePointOfInterest(int cityId, int id,
            [FromBody] JsonPatchDocument<PointOfInterestForUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            if (!_cityInfoRepository.CityExists(cityId))
            {
                return NotFound();
            }

            var pointOfInterestEntity = _cityInfoRepository.GetPointOfInterestForCity(cityId, id);
            if (pointOfInterestEntity == null)
            {
                return NotFound();
            }


            var pointOfInterestToPatch = Mapper.Map<PointOfInterestForUpdateDto>(pointOfInterestEntity);

            //LD STEP21
            // so here we apply the received "patchDoc" to the record that we already have in database.
            // if an error happen we update the "ModelState"
            patchDoc.ApplyTo(pointOfInterestToPatch, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (pointOfInterestToPatch.Description == pointOfInterestToPatch.Name)
            {
                ModelState.AddModelError("Description", "The provided description should be different from the name.");
            }

            //LD STEP22
            TryValidateModel(pointOfInterestToPatch);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Mapper.Map(pointOfInterestToPatch, pointOfInterestEntity);

            if (!_cityInfoRepository.Save())
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }

            return NoContent();
        }


        [HttpDelete("{cityId}/pointsofinterest/{id}")]
        public IActionResult DeletePointOfInterest(int cityId, int id)
        {
            if (!_cityInfoRepository.CityExists(cityId))
            {
                return NotFound();
            }

            var pointOfInterestEntity = _cityInfoRepository.GetPointOfInterestForCity(cityId, id);
            if (pointOfInterestEntity == null)
            {
                return NotFound();
            }

            _cityInfoRepository.DeletePointOfInterest(pointOfInterestEntity);

            if (!_cityInfoRepository.Save())
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }

            _mailService.Send("Point of interest deleted.",
                    $"Point of interest {pointOfInterestEntity.Name} with id {pointOfInterestEntity.Id} was deleted.");
            
            return NoContent();
        }


        #region LD
       
        //LD STEP10
        [HttpGet("{cityId}/pointsofinterestLd")]
        public IActionResult GetPointsOfInterestLd(int cityId)
        {

            var cityLd = CitiesDataStore.Current.Cities.FirstOrDefault(city => city.Id == cityId);

            //LD STEP8 we return a friendly "NotFound"
            if (cityLd == null)
            {
                //LD STEP23 - then we log
                _logger.LogInformation($"City with id {cityId} wasn't found when accessing points of interest.");


                _mailService.Send("Point of interest deleted.","was deleted.");


                return NotFound();
            }
            return Ok(cityLd.PointsOfInterest);
        }

        //LD STEP12
        [HttpGet("{cityId}/pointsofinterestLd/{id}", Name = "GetPointOfInterestLd")]
        public IActionResult GetPointOfInterestLd(int cityId, int id)
        {
            var cityLd = CitiesDataStore.Current.Cities.FirstOrDefault(city => city.Id == cityId);

            //LD STEP8 we return a friendly "NotFound"
            if (cityLd == null)
            {
                return NotFound();
            }
            return Ok(cityLd.PointsOfInterest.FirstOrDefault (poi => poi.Id == id));
        }

        //LD STEP14
        [HttpPost("{cityId}/pointsofinterestLd")]
        public IActionResult CreatePointOfInterestLd(int cityId, [FromBody] PointOfInterestForCreationDto pointOfInterest)
        {
            //LD we manage the situation where the request content is null
            if (pointOfInterest == null)
            {
                return BadRequest();
            }

            //LD we manage the situation when the city doesn't exist
            var cityLd = CitiesDataStore.Current.Cities.FirstOrDefault(city => city.Id == cityId);
            if (cityLd == null)
            {
                return NotFound();
            } 

            //LD STEP17
            if (pointOfInterest.Description == pointOfInterest.Name)
            {
                ModelState.AddModelError("Description", "The provided description should be different from the name.");
            }

            //LD STEP18
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //LD get the new max for the id, because in the DTO that we call there is not "Id"
            var MaxPointOfInterestId = CitiesDataStore.Current.Cities.SelectMany(RelatedInstance => RelatedInstance.PointsOfInterest).Max(poi => poi.Id);

            var FinalPointOfInterest = new PointOfInterestDto()
            {
                Id = ++MaxPointOfInterestId,
                Name = pointOfInterest.Name,
                Description = pointOfInterest.Description 
            };

            cityLd.PointsOfInterest.Add(FinalPointOfInterest);

            //LD from here we call //LD STEP12
            // we pass also the newly created "FinalPointOfInterest" that we will have in the body
            return CreatedAtRoute("GetPointOfInterestLd",new { cityId= cityId , Id=FinalPointOfInterest.Id }, FinalPointOfInterest);
        }

        //LD STEP19
        [HttpPut("{cityId}/pointsofinterestLd/{id}")]
        public IActionResult UpdatePointOfInterestLd(int cityId, int id, [FromBody] PointOfInterestForUpdateDto pointOfInterest)
        {
            if (pointOfInterest == null)
            {
                return BadRequest(); 
            }

            if (pointOfInterest.Description == pointOfInterest.Name)
            {
                ModelState.AddModelError("Description", "The provided description should be different from the name.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            var cityLd = CitiesDataStore.Current.Cities.FirstOrDefault(city => city.Id == cityId);
            if (cityLd == null)
            {
                return NotFound();
            }

            if (cityLd==null)
            {
                return NotFound();
            }

            var pointOfInterestLd = cityLd.PointsOfInterest.FirstOrDefault(point => point.Id == id);

  if (pointOfInterestLd == null)
            {
                return NotFound();
            }

            pointOfInterestLd.Name = pointOfInterest.Name;
            pointOfInterestLd.Description = pointOfInterest.Description;

            //LD usually on update we dont return the content
            return NoContent();
        }

        #endregion
    }
}
