using AutoMapper;
using DWorldProject.Data.Entities;
using DWorldProject.Data.Entities.Account;
using DWorldProject.Helpers;
using DWorldProject.Repositories.Abstract;
using DWorldProject.Services.Abstact;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
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

        public AccountService(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IUserRepository userRepository,
            IMapper mapper, ILogger<BlogPostService> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userRepository = userRepository;
            _mapper = mapper;
            _logger = logger;
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
                    UserName = model.UserName
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

                serviceResult.data = model;
                serviceResult.resultType = ServiceResultType.Success;
            }
            catch (Exception ex)
            {
                serviceResult.resultType = ServiceResultType.Fail;
                serviceResult.message = ex.Message;
            }

            return serviceResult;
        }

        public async Task<ServiceResult<LoginModel>> Login(LoginModel model)
        {
            var serviceResult = new ServiceResult<LoginModel>();
            try
            {
                var result = new SignInResult();

                if (!string.IsNullOrEmpty(model.Email))
                {
                    result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                }
                else
                {
                    result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, false);
                }

                if (!result.Succeeded)
                {
                    throw new Exception();
                }
                else
                {
                    var userEntity =_userRepository.GetSingle(u => u.IsActive && !u.IsDeleted && u.UserName == model.UserName);
                    var loginModel = new LoginModel()
                    {
                        Email = userEntity.Email,
                        Id = userEntity.Id,
                        UserName = userEntity.UserName
                    };
                    serviceResult.data = loginModel;
                    serviceResult.resultType = ServiceResultType.Success;
                }

            }
            catch (Exception ex)
            {
                serviceResult.resultType = ServiceResultType.Fail;
                serviceResult.message = ex.Message;
            }

            return serviceResult;
        }

        public async Task<ServiceResult<bool>> Logout()
        {
            var serviceResult = new ServiceResult<bool>();
            try
            {
                await _signInManager.SignOutAsync();

                serviceResult.data = true;
            }
            catch (Exception ex)
            {
                serviceResult.data = false;
                serviceResult.resultType = ServiceResultType.Fail;
                serviceResult.message = ex.Message;
            }

            return serviceResult;
        }

    }
}
