using AutoMapper;
using DWorldProject.Data.Entities;
using DWorldProject.Models.Request;
using DWorldProject.Models.Response;
using DWorldProject.Repositories.Abstract;
using DWorldProject.Services.Abstact;
using DWorldProject.Utils.ErrorCodes;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using DWorldProject.Models.ViewModel;
using DWorldProject.Utils.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace DWorldProject.Services
{
    public class BlogPostService : IBlogPostService
    {
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly IBlogPostRepository _blogPostRepository;
        private readonly IAzureService _azureService;
        private readonly IConfiguration _config;
        private readonly IElasticLogService _elasticLogService;

        public BlogPostService(IBlogPostRepository blogPostRepository, IMapper mapper, IConfiguration config,
            ILogger<BlogPostService> logger, IAzureService azureService, IElasticLogService elasticLogService)
        {
            _blogPostRepository = blogPostRepository;
            _mapper = mapper;
            _logger = logger;
            _azureService = azureService;
            _config = config;
            _elasticLogService = elasticLogService;
        }

        public async Task<ServiceResult<List<BlogPostResponseModel>>> Get()
        {
            var serviceResult = new ServiceResult<List<BlogPostResponseModel>>();
            var modelList = new List<BlogPostResponseModel>();
            try
            {
                var list = _blogPostRepository.FindBy(b => b.IsActive && !b.IsDeleted);
                foreach (var item in list)
                {
                    var image = await _azureService.GetAzureDataById(item.AWSImageId);
                    var model = _mapper.Map<BlogPostResponseModel>(item);
                    model.HeaderImageUrl = image[0].HeaderImageURL;
                    model.YouTubeVideoURL = image[0].YouTubeVideoURL;
                    modelList.Add(model);
                }

               _elasticLogService.CheckIndex();
               _logger.LogInformation("heyyy it loggedd!!!");
                serviceResult.Data = modelList;
                serviceResult.ResultType = ServiceResultType.Success;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception:BlogPost/Get");
                serviceResult.Message = e.Message;
                serviceResult.ResultType = ServiceResultType.Fail;
            }

            return serviceResult;
        }

        public async Task<ServiceResult<BlogPostResponseModel>> GetById(int id)
        {
            var serviceResult = new ServiceResult<BlogPostResponseModel>();
            try
            {
                var image = await _azureService.GetAzureDataById(id);
                var response = new BlogPostResponseModel();
                if (id > 0)
                {
                    var blogPost = _blogPostRepository.GetSingle(b => b.Id == id && b.IsActive && !b.IsDeleted);
                    response = _mapper.Map<BlogPostResponseModel>(blogPost);
                    response.HeaderImageUrl = image[0].HeaderImageURL;
                    response.ImageGallery = image[0].ImageGallery;
                    response.YouTubeVideoURL = image[0].YouTubeVideoURL;
                }
                else
                {
                    response.HomePageImages = image[0].HomePageImages;
                }
                serviceResult.Data = response;
                serviceResult.ResultType = ServiceResultType.Success;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception:BlogPost/GetById");
                serviceResult.Message = e.Message;
                serviceResult.ResultType = ServiceResultType.Fail;
            }

            return serviceResult;
        }

        public ServiceResult<BlogPostResponseModel> Update(BlogPostRequestModel blogPostModel)
        {
            var serviceResult = new ServiceResult<BlogPostResponseModel>();
            try
            {
                var model = _blogPostRepository.GetSingle(b => b.Id == blogPostModel.Id && b.IsActive && !b.IsDeleted);
                model.Title = blogPostModel.Title;
                model.Body = blogPostModel.Body;
                model.UpdatedDate = DateTime.Now;
                _blogPostRepository.UpdateWithCommit(model);
                var response = _mapper.Map<BlogPostResponseModel>(model);
                _elasticLogService.LogChange(OperationType.Edit, model);
                
                serviceResult.Data = response;
                serviceResult.ResultType = ServiceResultType.Success;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception:BlogPost/Update");
                serviceResult.Message = e.Message;
                serviceResult.ResultType = ServiceResultType.Fail;
            }

            return serviceResult;
        }

        public async Task<ServiceResult<BlogPostResponseModel>> Add(BlogPostRequestModel blogPostModel)
        {
            var serviceResult = new ServiceResult<BlogPostResponseModel>();
            try
            {
                if (blogPostModel.TopicId == null)
                {
                    serviceResult.ErrorCode = (int)BlogPostErrorCodes.TopicIsNull;
                    throw new Exception("Exception@BlogPost/Add");
                }

                if (blogPostModel.SectionId == null)
                {
                    serviceResult.ErrorCode = (int)BlogPostErrorCodes.SectionIsNull;
                    throw new Exception("Exception@BlogPost/Add");
                }


                var lastAWSImageId = _blogPostRepository.GetAll().Where(b => b.IsActive && !b.IsDeleted).Max(b => b.AWSImageId);

                var entity = new BlogPost()
                {
                    Body = blogPostModel.Body,
                    CreatedDate = DateTime.Now,
                    IsActive = true,
                    IsDeleted = false,
                    Title = blogPostModel.Title,
                    TopicId = (int)blogPostModel.TopicId,
                    SectionId = (int)blogPostModel.SectionId,
                    AWSImageId = lastAWSImageId++
                };
                var headerImageUrl = await UploadImagesToS3Bucket(blogPostModel.HeaderImageFile);
                var imageGallery = new List<string>();
                blogPostModel.ImageGalleryFile.ForEach(async image =>
                {
                   var imageUrl = await UploadImagesToS3Bucket(image);
                   imageGallery.Add(imageUrl.Data);
                });
                blogPostModel.HeaderImageUrl = headerImageUrl.Data;
                blogPostModel.ImageGallery = imageGallery;

                var azureDataAddResponse =_azureService.AddAzureData("Image", blogPostModel, entity.AWSImageId);
                _elasticLogService.LogChange(OperationType.Add, entity);
                serviceResult.Data = _mapper.Map<BlogPostResponseModel>(_blogPostRepository.AddWithCommit(entity));
                serviceResult.ResultType = ServiceResultType.Success;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception:BlogPost/Add");
                serviceResult.Message = e.Message;
                serviceResult.ResultType = ServiceResultType.Fail;
                serviceResult.ErrorCode ??= serviceResult.ErrorCode = (int)BlogPostErrorCodes.UnknownError;
            }

            return serviceResult;
        }

        public async Task<ServiceResult<string>> UploadImagesToS3Bucket(IFormFile file)
        {
            var secretKey = _config.GetSection("AWS").GetSection("SecretKey").Value;
            var accessKey = _config.GetSection("AWS").GetSection("AccessKey").Value;
            var bucketName = _config.GetSection("AWS").GetSection("PostImagesBucket").Value;
            var serviceResult = new ServiceResult<string>();
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
                        
                        serviceResult.Data = imageUrl;
                        serviceResult.ResultType = ServiceResultType.Success;
                    }
                }
            }
            catch (Exception e)
            {
                serviceResult.ResultType = ServiceResultType.Fail;
                serviceResult.Message = e.Message;
                throw new Exception("Exception@User/UploadImagesToS3Bucket");
            }

            return serviceResult;
        }

        public ServiceResult<BlogPostResponseModel> Delete(int id)
        {
            var serviceResult = new ServiceResult<BlogPostResponseModel>();
            try
            {
                var entity = _blogPostRepository.GetSingle(b => b.Id == id && b.IsActive && !b.IsDeleted);
                entity.DeletedDate = DateTime.Now;
                entity.IsActive = false;
                entity.IsDeleted = true;
                _blogPostRepository.UpdateWithCommit(entity);
                _elasticLogService.LogChange(OperationType.Delete, entity);
                serviceResult.Data = _mapper.Map<BlogPostResponseModel>(entity);
                serviceResult.ResultType = ServiceResultType.Success;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception:BlogPost/Delete");
                serviceResult.Message = e.Message;
                serviceResult.ResultType = ServiceResultType.Fail;
            }

            return serviceResult;
        }

    }
}
