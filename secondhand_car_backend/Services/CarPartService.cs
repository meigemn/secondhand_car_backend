using Microsoft.AspNetCore.Identity.Data;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using secondhand_car_backend.Models.Dtos.CreateDtos;
using secondhand_car_backend.Models.Dtos.EntityDtos;
using secondhand_car_backend.Models.Dtos.ResponseDtos;
using secondhand_car_backend.Models.Pocos;
using secondhand_car_backend.Models.UnitsOfWork;
using secondhand_car_backend.Utils;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace secondhand_car_backend.Services
{
    public interface IUsersService
    {
        
        Task<List<CarPartDto>> GetAll();
        Task<CarPartDto> GetById(string id);
        Task<CreateEditRemoveResponseDto> Create(CreateCarPartDto createCarPartDto);
        Task<CreateEditRemoveResponseDto> CreateAdmin(CreateCarPartDto createCarPartDto);
        Task<CreateEditRemoveResponseDto> Remove(string id);
        Task<CreateEditRemoveResponseDto> Update(CarPartDto carPartDto);
    }
    public class CarPartService
    {
    }
}
