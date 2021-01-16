using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using AutoMapper;
using DWorldProject.Data.Entities;
using DWorldProject.Models;
using DWorldProject.Models.Request;
using DWorldProject.Models.Response;
using DWorldProject.Repositories.Abstract;
using DWorldProject.Services.Abstact;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DWorldProject.Services
{
    public class BlogPostService : IBlogPostService
    {
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly IBlogPostRepository _blogPostRepository;
        private readonly IAmazonDynamoDB _amazonDynamoDb;
        private readonly DynamoDBContext _context;

        public BlogPostService(IBlogPostRepository blogPostRepository, IMapper mapper,
            ILogger<BlogPostService> logger, IAmazonDynamoDB amazonDynamoDb)
        {
            _blogPostRepository = blogPostRepository;
            _mapper = mapper;
            _logger = logger;
            _amazonDynamoDb = amazonDynamoDb;
            _context = new DynamoDBContext(amazonDynamoDb);
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
                    var image = await GetAzureDataById(item.AWSImageId);
                    var model = _mapper.Map<BlogPostResponseModel>(item);
                    model.HeaderImageUrl = image[0].HeaderImageURL;
                    model.YouTubeVideoURL = image[0].YouTubeVideoURL;
                    modelList.Add(model);
                }

                serviceResult.data = modelList;
                serviceResult.resultType = ServiceResultType.Success;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception:BlogPost/Get");
                serviceResult.message = e.Message;
                serviceResult.resultType = ServiceResultType.Fail;
            }

            return serviceResult;
        }

        public async Task<ServiceResult<BlogPostResponseModel>> GetById(int id)
        {
            var serviceResult = new ServiceResult<BlogPostResponseModel>();
            try
            {
                var image = await GetAzureDataById(id);
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
                serviceResult.data = response;
                serviceResult.resultType = ServiceResultType.Success;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception:BlogPost/GetById");
                serviceResult.message = e.Message;
                serviceResult.resultType = ServiceResultType.Fail;
            }

            return serviceResult;
        }

        public async Task<Amazon.DynamoDBv2.Model.ScanResponse> GetAzureData(string tableName)
        {
            List<string> attributesToGet = new List<string>();
            var images = Task.Run(async () => await _amazonDynamoDb.ScanAsync(tableName, attributesToGet));
            return await images;
        }

        public async Task<List<Image>> GetAzureDataById(int id)
        {
            var scanConditions = new List<ScanCondition>();
            scanConditions.Add(new ScanCondition("Id", ScanOperator.Equal, id));

            var response = Task.Run(async () => await _context.ScanAsync<Image>(scanConditions, null).GetRemainingAsync());
            return await response;
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
                serviceResult.data = response;
                serviceResult.resultType = ServiceResultType.Success;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception:BlogPost/Update");
                serviceResult.message = e.Message;
                serviceResult.resultType = ServiceResultType.Fail;
            }

            return serviceResult;
        }

        public ServiceResult<BlogPostResponseModel> Add(BlogPostRequestModel blogPostModel)
        {
            var serviceResult = new ServiceResult<BlogPostResponseModel>();
            try
            {
                var entity = new BlogPost()
                {
                    Body = blogPostModel.Body,
                    CreatedDate = DateTime.Now,
                    IsActive = true,
                    IsDeleted = false,
                    Title = blogPostModel.Title,
                    TopicId = blogPostModel.TopicId,
                    SectionId = blogPostModel.SectionId
                };
                serviceResult.data = _mapper.Map<BlogPostResponseModel>(_blogPostRepository.AddWithCommit(entity));
                serviceResult.resultType = ServiceResultType.Success;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception:BlogPost/Add");
                serviceResult.message = e.Message;
                serviceResult.resultType = ServiceResultType.Fail;
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
                serviceResult.data = _mapper.Map<BlogPostResponseModel>(entity);
                serviceResult.resultType = ServiceResultType.Success;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception:BlogPost/Delete");
                serviceResult.message = e.Message;
                serviceResult.resultType = ServiceResultType.Fail;
            }

            return serviceResult;
        }


    }
}
