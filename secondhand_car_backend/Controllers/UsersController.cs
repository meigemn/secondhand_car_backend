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
    /// <summary>
    /// Definimos endpoints
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class UsersController : BaseController
    {
        public UsersController(IServiceProvider serviceCollection) : base(serviceCollection)
        {

        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserDto createDto)
        {
            try
            {

                var response = await ServiceUsers.Create(createDto);
                if (response.Success)
                    return Ok(response);

                return BadRequest(response);

            }
            catch (Exception ex)
            {
                Logger.LogError("UsersControllers -> Create " + ex.Message + ex.StackTrace);
                return BadRequest(ex.Message);
            }

        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Remove(string id)
        {
            try
            {
                var response = await ServiceUsers.Remove(id);
                if (response.Success)
                    return Ok(response);

                return BadRequest(response);
            }
            catch (Exception ex)
            {
                Logger.LogError("UsersControllers -> Remove " + ex.Message + ex.StackTrace);
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> Update(UserDto userDto)
        {
            try
            {
                var response = await ServiceUsers.Update(userDto);
                if (response.Success)
                    return Ok(response);

                return BadRequest(response);
            }
            catch (Exception ex)
            {
                Logger.LogError("UsersControllers -> Update " + ex.Message + ex.StackTrace);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll([FromQuery] bool isPaginated = true, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var response = new List<UserDto>();
                if (isPaginated)
                    response = await ServiceUsers.GetAll();

                response = await ServiceUsers.GetAll();

                if (response.IsNullOrEmpty())
                    return Ok(response);
                return Ok(response);
            }
            catch (Exception ex)
            {
                Logger.LogError("UsersControllers -> GetAll " + ex.Message + ex.StackTrace);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                var response = await ServiceUsers.GetById(id);
                if (response != null)
                    return Ok(response);

                return Ok(new UserDto());
            }
            catch (Exception ex)
            {
                Logger.LogError("UsersControllers -> GetById " + ex.Message + ex.StackTrace);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest login)
        {
            try
            {
                var loginResult = await ServiceUsers.Login(login);

                // Verificar si el login fue exitoso
                if (!loginResult.Success)
                {
                    return Unauthorized(new { message = loginResult.Message ?? Literals.Credentials.InvalidCredentials });
                }

                return Ok(loginResult);
            }
            catch (Exception ex)
            {
                Logger.LogError("UsersControllers -> Login " + ex.Message + ex.StackTrace);
                return BadRequest(ex.Message);
            }
        }
    }
}