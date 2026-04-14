using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using secondhand_car_backend.Models.Dtos.CreateDtos;
using secondhand_car_backend.Models.Dtos.EntityDtos;
using secondhand_car_backend.Models.Dtos.ResponseDtos;
using secondhand_car_backend.Models.UnitsOfWork;
using secondhand_car_backend.Models.Pocos;
using secondhand_car_backend.Utils;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace secondhand_car_backend.Services
{
    public interface IUsersService
    {
        Task<LoginResponseDto> Login(LoginRequest login);
        Task<List<UserDto>> GetAll();
        Task<UserDto> GetById(string id);
        Task<CreateEditRemoveResponseDto> Create(CreateUserDto createUserDto);
        Task<CreateEditRemoveResponseDto> CreateAdmin(CreateUserDto createUserDto);
        Task<CreateEditRemoveResponseDto> Remove(string id);
        Task<CreateEditRemoveResponseDto> Update(UserDto userDto);
    }

    public sealed class UsersService : BaseService, IUsersService
    {
        #region Miembros privados

        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IOptionsMonitor<AppSettings> _appSettings;

        #endregion

        #region Constructores

        public UsersService(
            MeigemnUnitOfWork meigemnUnitOfWork,
            ILogger<UsersService> logger, // Se inyecta el logger específico
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IOptionsMonitor<AppSettings> appSettings)
            : base(meigemnUnitOfWork, logger) // Se pasan al BaseService
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _appSettings = appSettings;
        }

        #endregion

        #region Implementación de IUsersService

        public async Task<LoginResponseDto> Login(LoginRequest login)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(login.Email);

                if (user == null)
                {
                    return new LoginResponseDto { Success = false, Message = Literals.UserMessages.UserNotFound };
                }

                var passwordValid = await _userManager.CheckPasswordAsync(user, login.Password);

                if (!passwordValid)
                {
                    return new LoginResponseDto { Success = false, Message = Literals.UserMessages.InvalidPassword };
                }

                var userRoles = await _userManager.GetRolesAsync(user);

                
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                    new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                };

                foreach (var role in userRoles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

                var key = Encoding.ASCII.GetBytes(_appSettings.CurrentValue.Secret);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.AddDays(1),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);

                return new LoginResponseDto
                {
                    Success = true,
                    UserId = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Token = tokenHandler.WriteToken(token),
                    Roles = userRoles.ToList()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en Login");
                throw;
            }
        }

        public async Task<List<UserDto>> GetAll()
        {
            try
            {
                // Usamos la UnitOfWork que ya tiene el repositorio de IdentityUser
                return await _unitOfWork.Users.GetAll()
                    .Select(u => new UserDto
                    {
                        Id = u.Id,
                        Email = u.Email,
                        UserName = u.UserName
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en GetAll");
                throw;
            }
        }

        public async Task<UserDto> GetById(string id)
        {
            try
            {
                var user = await _unitOfWork.Users.GetAll(u => u.Id == id)
                    .Select(u => new UserDto
                    {
                        Id = u.Id,
                        Email = u.Email,
                        UserName = u.UserName
                    })
                    .FirstOrDefaultAsync();

                return user ?? new UserDto();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en GetById");
                throw;
            }
        }

        public async Task<CreateEditRemoveResponseDto> Create(CreateUserDto createUserDto)
        {
            var response = new CreateEditRemoveResponseDto();
            try
            {
                var user = new IdentityUser { UserName = createUserDto.UserName, Email = createUserDto.Email };
                var result = await _userManager.CreateAsync(user, createUserDto.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "User");
                    await _unitOfWork.Complete();
                    response.Success = true;
                }
                else
                {
                    response.Success = false;
                    response.Errors.AddRange(result.Errors.Select(e => e.Description));
                }
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en Create");
                throw;
            }
        }

        public async Task<CreateEditRemoveResponseDto> Remove(string id)
        {
            var response = new CreateEditRemoveResponseDto();
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    response.Success = false;
                    // CAMBIO: Consistencia en los mensajes de error
                    response.Errors.Add(Literals.UserMessages.UserNotFound);
                    return response;
                }

                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    // IMPORTANTE: Al ser un cambio que afecta a la base de datos,
                    // confirmamos la transacción con el UnitOfWork.
                    await _unitOfWork.Complete();
                    response.Success = true;
                }
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en Remove");
                throw;
            }
        }

        public async Task<CreateEditRemoveResponseDto> Remove(string id)
        {
            var response = new CreateEditRemoveResponseDto();
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    response.Success = false;
                    response.Errors.Add("Usuario no encontrado.");
                    return response;
                }

                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    await _unitOfWork.Complete();
                    response.Success = true;
                }
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en Remove");
                throw;
            }
        }

        public async Task<CreateEditRemoveResponseDto> Update(UserDto userDto)
        {
            var response = new CreateEditRemoveResponseDto();
            try
            {
                var user = await _userManager.FindByIdAsync(userDto.Id);
                if (user == null)
                {
                    response.Errors.Add("Usuario no encontrado.");
                    return response;
                }

                user.Email = userDto.Email;
                user.UserName = userDto.UserName;

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    await _unitOfWork.Complete();
                    response.Success = true;
                }
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en Update");
                throw;
            }
        }
        #endregion

        #region Excepciones y Utils
        public class UserApiException : Exception { }

        private static string Hash(string text)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(text));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes) builder.Append(b.ToString("x2"));
                return builder.ToString();
            }
        }
        #endregion
    }
}