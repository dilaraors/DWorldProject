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
using System.Threading.Tasks;

namespace DWorldProject.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccountService(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IUserRepository userRepository,
            IMapper mapper, ILogger<BlogPostService> logger, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userRepository = userRepository;
            _mapper = mapper;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ServiceResult<RegisterModel>> Register(RegisterModel model)
        {
            var serviceResult = new ServiceResult<RegisterModel>();
            try
            {
                var user = new IdentityUser
                {
                    UserName = model.UserName,
                    Email = model.Email,
                };

                string salt = HashCalculator.GenerateSalt();
                string hashedPassword = HashCalculator.HashPasswordWithSalt(model.Password, salt);

                var userEntity = new User()
                {
                    Email = model.Email,
                    Password = hashedPassword,
                    Name = model.Name,
                    Surname = model.Surname,
                    UserName = model.UserName,
                    RoleId = 1
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    _userRepository.AddWithCommit(userEntity);
                }

                foreach (var error in result.Errors)
                {
                    throw new Exception(error.Description);
                }

                serviceResult.Data = model;
                serviceResult.ResultType = ServiceResultType.Success;
            }
            catch (Exception ex)
            {
                serviceResult.ResultType = ServiceResultType.Fail;
                serviceResult.Message = ex.Message;
            }

            return serviceResult;
        }

        public async Task<ServiceResult<LoginModel>> Login(UserRequestModel model)
        {
            var serviceResult = new ServiceResult<LoginModel>();
            try
            {
                var result = new SignInResult();

                if (!string.IsNullOrEmpty(model.Email))
                {
                    result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, true, false);
                }
                else
                {
                    result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, true, false);
                }

                if (!result.Succeeded)
                {
                    throw new Exception("Login is unsuccessfull!");
                }
                else
                {
                    var userModel = _userRepository.FindBy(u => u.IsActive && !u.IsDeleted);
                    User umodel = new User();

                    if (!string.IsNullOrEmpty(model.UserName))
                    {
                        umodel = userModel.FirstOrDefault(u => u.UserName == model.UserName);
                    }
                    else
                    {
                        umodel = userModel.FirstOrDefault(u => u.UserName == model.Email);
                    }

                    var user = new IdentityUser()
                    {
                        Id = umodel?.Id.ToString(),
                        Email = umodel?.Email,
                        UserName = umodel?.UserName
                    };

                    var claim = new Claim("UserId", user.Id.ToString());
                    await _userManager.AddClaimAsync(user, claim);
                    await _signInManager.CreateUserPrincipalAsync(user);

                    var userEntity =_userRepository.GetSingle(u => u.IsActive && !u.IsDeleted && u.UserName == model.UserName);
                    var userViewModel = _mapper.Map<UserResponseModel>(userEntity);
                    var jwtToken = new JwtSecurityTokenHandler().WriteToken(CreateJwt(userViewModel));

                    serviceResult.Data = new LoginModel()
                    {
                        JwtToken = jwtToken,
                        User = userViewModel
                    };
                    serviceResult.ResultType = ServiceResultType.Success;
                }

            }
            catch (Exception ex)
            {
                serviceResult.ResultType = ServiceResultType.Fail;
                serviceResult.Message = ex.Message;
            }

            return serviceResult;
        }

        public async Task<ServiceResult<bool>> Logout()
        {
            var serviceResult = new ServiceResult<bool>();
            try
            {
                await _signInManager.SignOutAsync();

                serviceResult.Data = true;
                serviceResult.ResultType = ServiceResultType.Success;
            }
            catch (Exception ex)
            {
                serviceResult.Data = false;
                serviceResult.ResultType = ServiceResultType.Fail;
                serviceResult.Message = ex.Message;
            }

            return serviceResult;
        }

        private static JwtSecurityToken CreateJwt(UserResponseModel user)
        {
            var claims = new[]
            {
                new Claim("UserId", user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Id.ToString()),
                new Claim("ApplicationName", "DWorldApp")
            };

            var loginKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("MySuperSecureKey"));

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
