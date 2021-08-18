using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using AutoMapper;
using DWorldProject.Models.Response;
using DWorldProject.Repositories.Abstract;
using DWorldProject.Services.Abstact;
using DWorldProject.Utils.ErrorCodes;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;
using DWorldProject.Models.Request;

namespace DWorldProject.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly IConfiguration _config;

        public UserService(IUserRepository userRepository, IMapper mapper, ILogger<UserService> logger, IConfiguration config)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _logger = logger;
            _config = config;
        }

        public ServiceResult<UserResponseModel> GetById(int id)
        {
            var serviceResult = new ServiceResult<UserResponseModel>();
            try
            {
                var userEntity = _userRepository.GetSingle(u => u.IsActive && !u.IsDeleted && u.Id == id);
                if (userEntity == null)
                {
                    serviceResult.ErrorCode = (int)BaseErrorCodes.NotFound;
                    throw new Exception("User not found!");
                }

                var userModel = _mapper.Map<UserResponseModel>(userEntity);

                serviceResult.Data = userModel;
                serviceResult.ResultType = ServiceResultType.Success;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception:User/GetById");
                serviceResult.Message = e.Message;
                serviceResult.ResultType = ServiceResultType.Fail;
            }

            return serviceResult;
        }

        public ServiceResult<UserResponseModel> GetDWUser(string contextUserId)
        {
            var serviceResult = new ServiceResult<UserResponseModel>();

            try
            {
                var userEntity = _userRepository.GetSingle(u => u.IsActive && !u.IsDeleted);
                if (userEntity == null)
                {
                    serviceResult.ErrorCode = (int)BaseErrorCodes.NotFound;
                    throw new Exception("User not found!");
                }

                var userModel = _mapper.Map<UserResponseModel>(userEntity);

                serviceResult.Data = userModel;
                serviceResult.ResultType = ServiceResultType.Success;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception:User/GetById");
                serviceResult.Message = e.Message;
                serviceResult.ResultType = ServiceResultType.Fail;
            }

            return serviceResult;

        }

        public async Task<ServiceResult<bool>> UploadProfileImageToS3(IFormFile file, int id)
        {
            var secretKey = _config.GetSection("AWS").GetSection("SecretKey").Value;
            var accessKey = _config.GetSection("AWS").GetSection("AccessKey").Value;
            var bucketName = _config.GetSection("AWS").GetSection("ProfileImageS3Bucket").Value;
            var serviceResult = new ServiceResult<bool>();
            try
            {
                using (var client = new AmazonS3Client(accessKey, secretKey, RegionEndpoint.USEast2))
                {
                    using (var newMemoryStream = new MemoryStream())
                    {
                        file.CopyTo(newMemoryStream);

                        var uploadRequest = new TransferUtilityUploadRequest
                        {
                            InputStream = newMemoryStream,
                            Key = file.FileName,
                            BucketName = bucketName,
                            CannedACL = S3CannedACL.PublicRead
                        };

                        var fileTransferUtility = new TransferUtility(client);
                        await fileTransferUtility.UploadAsync(uploadRequest);
                        var imageUrl = string.Format("https://{0}.s3.{1}.amazonaws.com/{2}", bucketName, RegionEndpoint.USEast2.SystemName, file.FileName);
                        var user = _userRepository.GetSingle(u => u.IsActive && !u.IsDeleted && u.Id == id);
                        user.ProfileImageURL = imageUrl;
                        _userRepository.UpdateWithCommit(user);
                        serviceResult.Data = true;
                        serviceResult.ResultType = ServiceResultType.Success;
                    }
                }
            }
            catch (Exception e)
            {
                serviceResult.Data = false;
                serviceResult.ResultType = ServiceResultType.Fail;
                serviceResult.Message = e.Message;
                throw new Exception("Exception@User/UploadProfileImageToS3");
            }

            return serviceResult;
        }

        public ServiceResult<string> GetProfileImage(int id)
        {
            var serviceResult = new ServiceResult<string>();
            try
            {
                var userProfileImage = _userRepository.GetSingle(u => u.IsActive && !u.IsDeleted && u.Id == id)
                    .ProfileImageURL;
                serviceResult.Data = userProfileImage;
                serviceResult.ResultType = ServiceResultType.Success;
            }
            catch (Exception e)
            {
                serviceResult.ResultType = ServiceResultType.Fail;
                serviceResult.Message = e.Message;
                throw new Exception("Exception@User/GetProfileImage");
            }

            return serviceResult;
        }

        public ServiceResult<UserResponseModel> UpdateUserInfo(UserRequestModel model, int userId)
        {
            var serviceResult = new ServiceResult<UserResponseModel>();
            try
            {
                var user = _userRepository.GetSingle(u => u.IsActive && !u.IsDeleted && u.Id == userId);
                user.Name = model.Name;
                user.Surname = model.Surname;
                user.UserName = model.UserName;
                user.UpdatedDate = DateTime.Now;

                _userRepository.UpdateWithCommit(user);
                serviceResult.Data = _mapper.Map<UserResponseModel>(user);
                serviceResult.ResultType = ServiceResultType.Success;
            }
            catch (Exception e)
            {
                serviceResult.ResultType = ServiceResultType.Fail;
                serviceResult.Message = e.Message;
                throw new Exception("Exception@User/UpdateUserInfo");
            }

            return serviceResult;
        }
    }
}
