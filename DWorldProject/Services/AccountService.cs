using AutoMapper;
using DWorldProject.Data.Entities;
using DWorldProject.Data.Entities.Account;
using DWorldProject.Helpers;
using DWorldProject.Models.Request;
using DWorldProject.Models.Response;
using DWorldProject.Repositories.Abstract;
using DWorldProject.Services.Abstact;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DWorldProject.Utils.ErrorCodes;
using Microsoft.Extensions.Configuration;

namespace DWorldProject.Services
{
    public class AccountService : IAccountService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;

        public AccountService(IUserRepository userRepository,
            IMapper mapper, ILogger<BlogPostService> logger, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _logger = logger;
            _configuration = configuration;
        }

        public ServiceResult<UserResponseModel> Register(UserRequestModel model)
        {
            var serviceResult = new ServiceResult<UserResponseModel>();
            try
            {
                var usernameExist = _userRepository.GetSingle(u => u.IsActive && !u.IsDeleted && u.UserName == model.UserName);
                if (usernameExist != null)
                {
                    serviceResult.ErrorCode = (int)UserErrorCodes.AlreadyExists;
                    throw new Exception("Username exist");
                }

                var emailExist = _userRepository.GetSingle(u => u.Email == model.Email && u.IsActive && !u.IsDeleted);
                if (emailExist != null)
                {
                    serviceResult.ErrorCode = (int)UserErrorCodes.AlreadyExists;
                    throw new Exception("Email exist");
                }


                string salt = HashCalculator.GenerateSalt();
                string hashedPassword = HashCalculator.HashPasswordWithSalt(model.Password, salt);

                var userEntity = new User()
                {
                    Email = model.Email,
                    Password = hashedPassword,
                    PasswordSalt = salt,
                    Name = model.Name,
                    Surname = model.Surname,
                    UserName = model.UserName,
                    RoleId = 1
                };

                 _userRepository.AddWithCommit(userEntity);

                 serviceResult.Data = _mapper.Map<UserResponseModel>(userEntity);
                 serviceResult.ResultType = ServiceResultType.Success;
                 _logger.LogInformation($"Successful Register. Register Model: {JsonSerializer.Serialize<User>(userEntity)}");
            }
            catch (Exception ex)
            {
                serviceResult.ResultType = ServiceResultType.Fail;
                serviceResult.Message = ex.Message;
            }

            return serviceResult;
        }

        public ServiceResult<LoginModel> Login(UserRequestModel model)
        {
            var serviceResult = new ServiceResult<LoginModel>();
            try
            {
                var user = _userRepository.AllIncludingAsQueryable(u => u.Role).FirstOrDefault(u => u.IsActive && !u.IsDeleted && (u.UserName == model.UserName || u.Email == model.UserName));

                if (user == null)
                {
                    serviceResult.ErrorCode = (int)UserErrorCodes.IncorrectUsername;
                    throw new Exception("Username is incorrect");
                }

                var hashedPassword = HashCalculator.HashPasswordWithSalt(model.Password, user.PasswordSalt);

                if (hashedPassword != user.Password)
                {
                    serviceResult.ErrorCode = (int)UserErrorCodes.WrongPassword;
                    throw new Exception("Password is wrong");
                }

                var userViewModel = _mapper.Map<UserResponseModel>(user);
                var jwtToken = new JwtSecurityTokenHandler().WriteToken(CreateJwt(user));

                serviceResult.Data = new LoginModel()
                {
                    JwtToken = jwtToken,
                    User = userViewModel
                };
                serviceResult.ResultType = ServiceResultType.Success;
                _logger.LogInformation($"Successful Login. Login Model: {JsonSerializer.Serialize(model)}");

            }
            catch (Exception e)
            {
                serviceResult.ResultType = ServiceResultType.Fail;
                serviceResult.Message = e.Message;
                serviceResult.ErrorCode = serviceResult.ErrorCode ?? (int)BaseErrorCodes.BadRequest;
                _logger.LogError($"Unsuccessful Login. Login Model: {JsonSerializer.Serialize(model)}, ErrorCode: {serviceResult.ErrorCode}, ErrorMessage: {e.Message}");

            }

            return serviceResult;
        }

        //public async Task<ServiceResult<bool>> Logout()
        //{
        //    var serviceResult = new ServiceResult<bool>();
        //    try
        //    {
        //        await _signInManager.SignOutAsync();

        //        serviceResult.Data = true;
        //        serviceResult.ResultType = ServiceResultType.Success;
        //    }
        //    catch (Exception ex)
        //    {
        //        serviceResult.Data = false;
        //        serviceResult.ResultType = ServiceResultType.Fail;
        //        serviceResult.Message = ex.Message;
        //    }

        //    return serviceResult;
        //}

        private JwtSecurityToken CreateJwt(User user)
        {
            var claims = new[]
            {
                new Claim("UserId", user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Id.ToString()),
                new Claim("ApplicationName", "DWorldApp"),
                new Claim (ClaimTypes.Role, user.Role.Name)
            };

            var loginKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["TokenSecretKey"]));

            var token = new JwtSecurityToken(
                issuer: "DWorld",
                audience: "DWorld",
                expires: DateTime.UtcNow.AddYears(1),
                claims: claims,
                signingCredentials: new SigningCredentials(loginKey, SecurityAlgorithms.HmacSha256)
            );
            return token;

        }
    }
}
