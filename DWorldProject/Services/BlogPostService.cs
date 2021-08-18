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
using Amazon.DynamoDBv2.Model;
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
        private readonly IAmazonService _amazonService;
        private readonly IConfiguration _config;

        public BlogPostService(IBlogPostRepository blogPostRepository, IMapper mapper, IConfiguration config,
            ILogger<BlogPostService> logger, IAmazonService azureService)
        {
            _blogPostRepository = blogPostRepository;
            _mapper = mapper;
            _logger = logger;
            _amazonService = azureService;
            _config = config;
        }

        public async Task<ServiceResult<List<BlogPostResponseModel>>> Get()
        {
            var serviceResult = new ServiceResult<List<BlogPostResponseModel>>();
            var modelList = new List<BlogPostResponseModel>();
            var list = _blogPostRepository.FindBy(b => b.IsActive && !b.IsDeleted);
            foreach (var item in list)
            {
                var image = await _amazonService.GetAmazonDataById(item.AWSImageId);
                var model = _mapper.Map<BlogPostResponseModel>(item);
                model.HeaderImageURL = image[0].HeaderImageURL;
                model.YouTubeVideoURL = image[0].YouTubeVideoURL;
                modelList.Add(model);
            }
            serviceResult.Data = modelList;
            serviceResult.ResultType = ServiceResultType.Success;

            return serviceResult;
        }

        public async Task<ServiceResult<BlogPostResponseModel>> GetById(int id)
        {
            var serviceResult = new ServiceResult<BlogPostResponseModel>();
            var response = new BlogPostResponseModel();
            if (id > 0)
            {
                var blogPost = _blogPostRepository.GetSingle(b => b.Id == id && b.IsActive && !b.IsDeleted);
                response = _mapper.Map<BlogPostResponseModel>(blogPost);
                var image = await _amazonService.GetAmazonDataById(blogPost.AWSImageId);
                response.HeaderImageURL = image[0].HeaderImageURL;
                response.ImageGallery = image[0].ImageGallery?.ToList();
                response.YouTubeVideoURL = blogPost.YouTubeVideoURL;
            }
            else
            {
                var image = await _amazonService.GetAmazonDataById(0);
                response.HomePageImages = image[0].HomePageImages;
            }
            serviceResult.Data = response;
            serviceResult.ResultType = ServiceResultType.Success;
            return serviceResult;
        }
        public async Task<ServiceResult<List<BlogPostResponseModel>>> GetBySection(int sectionId, string topicName)
        {
            var serviceResult = new ServiceResult<List<BlogPostResponseModel>>();
            var blogPosts = new List<BlogPost>();
            if(topicName != null)
            {
                blogPosts = _blogPostRepository.AllIncluding(b => b.Topic).Where(b => b.SectionId == sectionId && b.Topic.Name == topicName && b.IsActive && !b.IsDeleted).ToList();
            }
            else
            {
                blogPosts = _blogPostRepository.FindBy(b => b.SectionId == sectionId && b.IsActive && !b.IsDeleted).ToList();
            }
            var responseList = new List<BlogPostResponseModel>();
            var tasks = blogPosts.Select(async blogPost => {
                var response = _mapper.Map<BlogPostResponseModel>(blogPost);
                var image = await _amazonService.GetAmazonDataById(blogPost.AWSImageId);
                response.HeaderImageURL = image[0].HeaderImageURL;
                response.ImageGallery = image[0].ImageGallery?.ToList();
                response.YouTubeVideoURL = blogPost.YouTubeVideoURL;
                responseList.Add(response);
            });
            await Task.WhenAll(tasks);

            serviceResult.Data = responseList;
            serviceResult.ResultType = ServiceResultType.Success;
            return serviceResult;
        }

        public async Task<ServiceResult<BlogPostResponseModel>> Update(BlogPostRequestModel blogPostModel)
        {
            var serviceResult = new ServiceResult<BlogPostResponseModel>();
            var model = _blogPostRepository.GetSingle(b => b.Id == blogPostModel.Id && b.IsActive && !b.IsDeleted);
            model.Title = blogPostModel.Title;
            model.Body = blogPostModel.Body;
            model.SectionId = (int)blogPostModel.SectionId;
            model.TopicId = (int)blogPostModel.TopicId;
            model.UpdatedDate = DateTime.Now;
            model.YouTubeVideoURL = blogPostModel.YouTubeVideoURL;
            var image = await _amazonService.GetAmazonDataById(model.AWSImageId);

            if (blogPostModel.HeaderImageFile == null)
            {
                blogPostModel.HeaderImageURL = image[0].HeaderImageURL;
            }
            else
            {
                var headerImageUrl = await UploadImagesToS3Bucket(blogPostModel.HeaderImageFile);
                blogPostModel.HeaderImageURL = headerImageUrl.Data;
            }

            if (blogPostModel.ImageGalleryFile == null || blogPostModel.ImageGalleryFile.Count == 0)
            {
                blogPostModel.ImageGallery = image[0].ImageGallery?.ToList();
            }
            else
            {
                blogPostModel.ImageGallery = await UploadImageFilesToAWS(blogPostModel.ImageGalleryFile);
            }
            if (blogPostModel.YouTubeVideoURL == null || string.IsNullOrEmpty(blogPostModel.YouTubeVideoURL))
            {
                blogPostModel.YouTubeVideoURL = image[0].YouTubeVideoURL;
            }

            var updateResult = await _amazonService.UpdateAmazonData("Image", blogPostModel, model.AWSImageId);
            _blogPostRepository.UpdateWithCommit(model);
            var response = _mapper.Map<BlogPostResponseModel>(model);

            serviceResult.Data = response;
            serviceResult.ResultType = ServiceResultType.Success;

            return serviceResult;
        }

        public async Task<List<string>> UploadImageFilesToAWS(List<IFormFile> imageGalleryFile)
        {
            var imageGallery = new List<string>();
            imageGalleryFile.ForEach(async image =>
            {
                var imageUrl = await UploadImagesToS3Bucket(image);
                imageGallery.Add(imageUrl.Data);
            });
            await Task.Delay(5000);
            return imageGallery;
        }

        public async Task<ServiceResult<BlogPostResponseModel>> Add(BlogPostRequestModel blogPostModel)
        {
            var serviceResult = new ServiceResult<BlogPostResponseModel>();

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


            var lastAWSImage = _blogPostRepository.FindBy(b => b.IsActive && !b.IsDeleted).ToList();
            var lastAWSImageId = 0;
            if (lastAWSImage.Count > 0)
            {
                lastAWSImageId = lastAWSImage.Max(b => b.AWSImageId);
            }
            else
            {
                lastAWSImageId = 0;
            }

            var entity = new BlogPost()
            {
                Body = blogPostModel.Body,
                CreatedDate = DateTime.Now,
                IsActive = true,
                IsDeleted = false,
                Title = blogPostModel.Title,
                TopicId = (int)blogPostModel.TopicId,
                SectionId = (int)blogPostModel.SectionId,
                AWSImageId = lastAWSImageId + 1,
                YouTubeVideoURL = blogPostModel.YouTubeVideoURL
            };
            var headerImageUrl = new ServiceResult<string>();
            var imageGallery = new List<string>();
            if (blogPostModel.HeaderImageFile != null)
            {
                headerImageUrl = await UploadImagesToS3Bucket(blogPostModel.HeaderImageFile);
            }
            if (blogPostModel.ImageGalleryFile != null)
            {
                blogPostModel.ImageGalleryFile.ForEach(async image =>
                {
                    var imageUrl = await UploadImagesToS3Bucket(image);
                    imageGallery.Add(imageUrl.Data);
                });
            }

            blogPostModel.HeaderImageURL = blogPostModel.HeaderImageFile != null ? headerImageUrl.Data : "";
            blogPostModel.ImageGallery = imageGallery;

            var azureDataAddResponse = await _amazonService.AddAmazonData("Image", blogPostModel, entity.AWSImageId);
            serviceResult.Data = _mapper.Map<BlogPostResponseModel>(_blogPostRepository.AddWithCommit(entity));
            serviceResult.Data.HeaderImageURL = blogPostModel.HeaderImageURL;
            serviceResult.Data.ImageGallery = blogPostModel.ImageGallery;
            serviceResult.ResultType = ServiceResultType.Success;

            return serviceResult;
        }

        public async Task<ServiceResult<string>> UploadImagesToS3Bucket(IFormFile file)
        {
            var secretKey = _config.GetSection("AWS").GetSection("SecretKey").Value;
            var accessKey = _config.GetSection("AWS").GetSection("AccessKey").Value;
            var bucketName = _config.GetSection("AWS").GetSection("PostImagesBucket").Value;
            var serviceResult = new ServiceResult<string>();
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

            return serviceResult;
        }

        public ServiceResult<BlogPostResponseModel> Delete(int id)
        {
            var serviceResult = new ServiceResult<BlogPostResponseModel>();
            var entity = _blogPostRepository.GetSingle(b => b.Id == id && b.IsActive && !b.IsDeleted);
            entity.DeletedDate = DateTime.Now;
            entity.IsActive = false;
            entity.IsDeleted = true;
            _blogPostRepository.UpdateWithCommit(entity);
            serviceResult.Data = _mapper.Map<BlogPostResponseModel>(entity);
            serviceResult.ResultType = ServiceResultType.Success;

            return serviceResult;
        }

    }
}
