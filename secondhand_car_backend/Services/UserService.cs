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
using secondhand_car_backend.Models.Dtos.CreateDtos;
using secondhand_car_backend.Models.Dtos.EntityDtos;
using secondhand_car_backend.Models.Dtos.ResponseDtos;
using secondhand_car_backend.Models.Pocos;
using secondhand_car_backend.Models.UnitsOfWork;
using secondhand_car_backend.Utils;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace secondhand_car_backend.Services
{
    public interface IUsersService
    {
        /// <summary>
        /// 	Método para el inicio de sesión del usuario en la aplicación
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
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

        public UsersService(MeigemnUnitOfWork meigemnUnitOfWork,
            ILogger<UsersService> logger,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IOptionsMonitor<AppSettings> appSettings)
            : base(meigemnUnitOfWork, logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _appSettings = appSettings;
        }

        #endregion

        #region Implementación de IUsersService (interfaz)
        public async Task<LoginResponseDto> Login(LoginRequest login)
        {
            try
            {
                // Usa FindByEmailAsync del UserManager para encontrar el usuario
                var user = await _userManager.FindByEmailAsync(login.Email);

                if (user == null)
                {
                    return new LoginResponseDto
                    {
                        Success = false,
                        Message = "Usuario no encontrado"
                    };
                }

                var passwordValid = await _userManager.CheckPasswordAsync(user, login.Password);

                if (!passwordValid)
                {
                    return new LoginResponseDto
                    {
                        Success = false,
                        Message = "Contraseña incorrecta"
                    };
                }

                // Obtener roles del usuario
                var userRoles = await _userManager.GetRolesAsync(user);

                var claims = new List<Claim>
    {
     new Claim(Literals.Claim_UserId, user.Id.ToString()),
     new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
     new Claim(Literals.Claim_Email, user.Email ?? string.Empty),
     new Claim(Literals.Claim_PhoneNumber, user.PhoneNumber ?? string.Empty),
    };

                // Agregar roles como claims
                foreach (var role in userRoles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

                var claimsIdentity = new ClaimsIdentity(claims);

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_appSettings.CurrentValue.Secret);

                var tokenDescriptor = new SecurityTokenDescriptor()
                {
                    Subject = claimsIdentity,
                    Expires = DateTime.UtcNow.AddDays(1),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                var writtenToken = tokenHandler.WriteToken(token);

                var tokenString = writtenToken ?? string.Empty; // Devolver el JWT estándar

                return new LoginResponseDto
                {
                    Success = true,
                    UserId = user.Id.ToString(),
                    UserName = user.UserName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Token = tokenString, // JWT estándar
                    Roles = userRoles.ToList()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al iniciar sesión. UsersService -> Login");
                throw;
            }
        }

        public async Task<List<UserDto>> GetAll()
        {
            try
            {
                var users = await _unitOfWork.Users.GetAll()
                 .Select(u => new UserDto
                 {
                     Id = u.Id,
                     Email = u.Email,
                     UserName = u.UserName,
                     
                 })
                 .ToListAsync();

                if (users == null || !users.Any())
                    return new List<UserDto>();

                return users;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los usuarios. UserService -> Get all ");
                throw new UserApiException();
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
                     UserName = u.UserName,
                    
                 })
                 .FirstOrDefaultAsync();

                if (user == null)
                    return new UserDto();

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el usuario. UserService -> GetById ");
                throw new UserApiException();
            }
        }

        public async Task<CreateEditRemoveResponseDto> Create(CreateUserDto createUserDto)
        {
            try
            {
                var response = new CreateEditRemoveResponseDto();
                var user = new IdentityUser()
                {
                    UserName = createUserDto.UserName,
                    Email = createUserDto.Email,
                };

                var result = await _userManager.CreateAsync(user, createUserDto.Password);

                if (result.Succeeded)
                {
                    // Por defecto, asignar rol "User"
                    await _userManager.AddToRoleAsync(user, "User");
                    await _unitOfWork.Complete();

                    response.Success = true;
                    return response;
                }
                else
                {
                    response.Success = false;
                    response.Errors.AddRange(result.Errors.Select(e => e.Description));
                    return response;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el usuario. UserService -> Create");
                throw;
            }
        }

        public async Task<CreateEditRemoveResponseDto> CreateAdmin(CreateUserDto createUserDto)
        {
            try
            {
                var response = new CreateEditRemoveResponseDto();
                var user = new IdentityUser()
                {
                    UserName = createUserDto.UserName,
                    Email = createUserDto.Email,
                };

                var result = await _userManager.CreateAsync(user, createUserDto.Password);

                if (result.Succeeded)
                {
                    // Asignar rol "Admin"
                    await _userManager.AddToRoleAsync(user, "Admin");
                    await _unitOfWork.Complete();

                    response.Success = true;
                    return response;
                }
                else
                {
                    response.Success = false;
                    response.Errors.AddRange(result.Errors.Select(e => e.Description));
                    return response;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el usuario admin. UserService -> CreateAdmin ");
                throw;
            }
        }

        public async Task<CreateEditRemoveResponseDto> Remove(string id)
        {
            try
            {
                var response = new CreateEditRemoveResponseDto();
                var user = await _unitOfWork.Users.GetAll(u => u.Id == id).FirstOrDefaultAsync();

                if (user == null)
                {
                    response.Success = false;
                    response.Errors.Add("Usuario no encontrado.");
                    return response;
                }

                var result = await _userManager.DeleteAsync(user);
                await _unitOfWork.Complete();
                if (!result.Succeeded)
                {
                    response.Success = false;
                    response.Errors.Add("Error al borrar usuario UserService -> Remove");
                    return response;
                }
                response.Success = true;
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al borrar el usuario. UserService -> Remove");
                throw;
            }
        }

        public async Task<CreateEditRemoveResponseDto> Update(UserDto userDto)
        {
            try
            {
                var response = new CreateEditRemoveResponseDto();
                var user = await _unitOfWork.Users.GetAll(u => u.Id == userDto.Id).FirstOrDefaultAsync();

                if (user == null)
                {
                    response.Success = false;
                    response.Errors.Add("Usuario no encontrado.");
                    return response;
                }

                user.Email = userDto.Email;
                user.UserName = userDto.UserName;

                _unitOfWork.Users.Update(user);
                await _unitOfWork.Complete();

                response.Success = true;
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el usuario. UserService -> Update");
                throw;
            }
        }
        #endregion



        #region Excepciones particulares del servicio

        public class UserApiException : Exception { }
        public class UserNotFoundException : Exception { }
        public class PasswordNotValidException : Exception { }
        public class UserLockedException : Exception { }
        public class UserWithoutVerificationException : Exception { }
        public class UserSessionNotValidException : Exception { }
        public class UserWithoutPermissionException : Exception { }

        #endregion


        #region Métodos privados

        /// <summary>
                /// 	Hashea un texto
                /// </summary>
                /// <param name="text">el texto a hashear</param>
                /// <returns></returns>
        private static string Hash(string text) // **CAMBIO: Se hizo privado el método Hash**
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(text));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        #endregion
    }
}