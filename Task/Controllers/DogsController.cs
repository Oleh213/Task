using System;
using System.Net.NetworkInformation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Task.BusinessLogic;
using Task.Context;
using Task.DBContext;
using Task.DTO;
using Task.Interfaces;
using Task.Models;

namespace Task.Controllers
{
    [ApiController]
    public class DogsController : ControllerBase
    {
        private readonly IDogsActionsBL _dogsActionsBL;

        private readonly ILoggerBL _loggerBL;

        public DogsController(IDogsActionsBL dogsActionsBL, ILoggerBL loggerBL)
		{
            _dogsActionsBL = dogsActionsBL;
            _loggerBL = loggerBL;
        }

        [HttpGet("ping")]
        public IActionResult Ping()
        {
            try
            {
                var resposne = new Response<string>
                {
                    IsError = false,
                    Data = "Dogs house service. Version 1.0.1",
                };

                return Ok(resposne.Data);
            }
            catch (Exception ex)
            {
                _loggerBL.AddLog(LoggerLevel.Error, $"Message: '{ex.Message}', Source: '{ex.Source}', InnerException: '{ex.InnerException}'");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("dogs")]
        public async Task<IActionResult> GetDogs([FromQuery] GetDogsModel model)
        {
            try
            {
                var respons = await _dogsActionsBL.GetDogs(model);
                var resposne = new Response<List<DogDTO>>
                {
                    IsError = false,
                    Data = respons.ToList(),
                };

                return Ok(resposne.Data);
            }
            catch (Exception ex)
            {
                _loggerBL.AddLog(LoggerLevel.Error, $"Message: '{ex.Message}', Source: '{ex.Source}', InnerException: '{ex.InnerException}' ");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("dog")]
        public async Task<IActionResult> AddDog([FromQuery] AddDogModel model)
        {
            try
            {
                var responsDogActions = await _dogsActionsBL.AddDogAction(model);

                var result = new Response<string>();

                result.IsError = responsDogActions != AddDogStatus.Success;

                switch (responsDogActions)
                {
                    case AddDogStatus.DogInfoInCorect:
                        result.ErrorMessage = "The information about the dog is incorrect!";
                        break;
                    case AddDogStatus.DogNameExist:
                        result.ErrorMessage = "The dog's name already exists!";
                        break;
                    case AddDogStatus.Success:
                        result.Data = "The dog was successfuly added";
                        break;
                    case AddDogStatus.UnknownError:
                        result.ErrorMessage = "Sorry! We received an unknown error. Contact the developer!";
                        break;
                    default:
                        break;
                }
                return !result.IsError ? Ok(result) : NotFound(result);
            }
            catch (Exception ex)
            {
                _loggerBL.AddLog(LoggerLevel.Error, $"Message: '{ex.Message}', Source: '{ex.Source}', InnerException: '{ex.InnerException}' ");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}

