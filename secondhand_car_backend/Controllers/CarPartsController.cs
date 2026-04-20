using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using secondhand_car_backend.Controllers;
using secondhand_car_backend.Models.Dtos.CreateDtos;
using secondhand_car_backend.Models.Dtos.EntityDtos;
using Serilog.Core;
using secondhand_car_backend.Utils;
namespace secondhand_car_backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CarPartsController : BaseController
    {
        public CarPartsController(IServiceProvider serviceCollection) : base(serviceCollection)
        {

        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCarPartDto createDto)
        {
            try
            {

                var response = await ServiceCarParts.Create(createDto);
                if (response.Success)
                    return Ok(response);

                return BadRequest(response);

            }
            catch (Exception ex)
            {
                Logger.LogError("CarPartsControllers -> Create " + ex.Message + ex.StackTrace);
                return BadRequest(ex.Message);
            }

        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Remove(string id)
        {
            try
            {
                var response = await ServiceCarParts.Remove(id);
                if (response.Success)
                    return Ok(response);

                return BadRequest(response);
            }
            catch (Exception ex)
            {
                Logger.LogError("CarPartsControllers -> Remove " + ex.Message + ex.StackTrace);
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> Update(CarPartDto CarPartDto)
        {
            try
            {
                var response = await ServiceCarParts.Update(CarPartDto);
                if (response.Success)
                    return Ok(response);

                return BadRequest(response);
            }
            catch (Exception ex)
            {
                Logger.LogError("CarPartsControllers -> Update " + ex.Message + ex.StackTrace);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll([FromQuery] bool isPaginated = true, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var response = new List<CarPartDto>();
                if (isPaginated)
                    response = await ServiceCarParts.GetAll();

                response = await ServiceCarParts.GetAll();

                if (response.IsNullOrEmpty())
                    return Ok(response);
                return Ok(response);
            }
            catch (Exception ex)
            {
                Logger.LogError("CarPartsControllers -> GetAll " + ex.Message + ex.StackTrace);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                var response = await ServiceCarParts.GetById(id);
                if (response != null)
                    return Ok(response);

                return Ok(new CarPartDto());
            }
            catch (Exception ex)
            {
                Logger.LogError("CarPartsControllers -> GetById " + ex.Message + ex.StackTrace);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest login)
        {
            try
            {
                var loginResult = await ServiceCarParts.Login(login);

                // Verificar si el login fue exitoso
                if (!loginResult.Success)
                {
                    return Unauthorized(new { message = loginResult.Message ?? Literals.Credentials.InvalidCredentials });
                }

                return Ok(loginResult);
            }
            catch (Exception ex)
            {
                Logger.LogError("CarPartsControllers -> Login " + ex.Message + ex.StackTrace);
                return BadRequest(ex.Message);
            }
        }
    }
}
}
