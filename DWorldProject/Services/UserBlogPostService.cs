﻿using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using AutoMapper;
using DWorldProject.Data.Entities;
using DWorldProject.Models.Response;
using DWorldProject.Repositories.Abstract;
using DWorldProject.Services.Abstact;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DWorldProject.Models.Request;

namespace DWorldProject.Services
{
    public class UserBlogPostService : IUserBlogPostService
    {
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly IUserBlogPostRepository _userBlogPostRepository;
        private readonly IBlogPostRepository _blogPostRepository;
        private readonly IAzureService _azureService;
        private readonly IUserService _userService;

        public UserBlogPostService(IUserBlogPostRepository userBlogPostRepository, IMapper mapper,
            ILogger<BlogPostService> logger, IAzureService azureService, IUserService userService, IBlogPostRepository blogPostRepository)
        {
            _userBlogPostRepository = userBlogPostRepository;
            _mapper = mapper;
            _logger = logger;
            _azureService = azureService;
            _userService = userService;
            _blogPostRepository = blogPostRepository;
        }

        public async Task<ServiceResult<List<UserBlogPostResponseModel>>> GetByType(int blogType, int userId)
        {
            var serviceResult = new ServiceResult<List<UserBlogPostResponseModel>>();

            var modelList = new List<UserBlogPostResponseModel>();
            try
            {
                var list = _userBlogPostRepository.AllIncludingAsQueryable(ubp => ubp.BlogPost).Where(ub => ub.IsActive && !ub.IsDeleted && ub.BlogType == blogType && ub.UserId == userId);
                foreach (var item in list)
                {
                    var image = await _azureService.GetAzureDataById(item.BlogPost.AWSImageId);
                    var model = _mapper.Map<UserBlogPostResponseModel>(item);
                    model.Id = item.BlogPostId;
                    model.HeaderImageUrl = image[0].HeaderImageURL;
                    model.YouTubeVideoURL = image[0].YouTubeVideoURL;
                    modelList.Add(model);
                }

                serviceResult.data = modelList;
                serviceResult.resultType = ServiceResultType.Success;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception:UserBlogPost/GetByType");
                serviceResult.message = e.Message;
                serviceResult.resultType = ServiceResultType.Fail;
            }

            return serviceResult;
        }

        public ServiceResult<UserBlogPostByTypeResponseModel> AddByType(UserBlogPostByTypeRequestModel request)
        {
            var serviceResult = new ServiceResult<UserBlogPostByTypeResponseModel>();
            try
            {
                var blogPost =
                    _blogPostRepository.GetSingle(b => b.IsActive && !b.IsDeleted && b.Id == request.BlogPostId);
                var userBlogPostByType = _userBlogPostRepository.GetSingle(b => b.UserId == request.UserId && b.BlogPostId == request.BlogPostId &&
                    b.BlogType == request.Type);
                if (userBlogPostByType == null)
                {
                    var userBlogPost = new UserBlogPost()
                    {
                        UserId = request.UserId,
                        BlogPostId = blogPost.Id,
                        BlogType = request.Type
                    };

                    _userBlogPostRepository.AddWithCommit(userBlogPost);
                }
                else if(!userBlogPostByType.IsActive && userBlogPostByType.IsDeleted)
                {
                    userBlogPostByType.IsActive = true;
                    userBlogPostByType.IsDeleted = false;

                    _userBlogPostRepository.UpdateWithCommit(userBlogPostByType);
                }

                var responseModel = new UserBlogPostByTypeResponseModel()
                {
                    UserId = request.UserId,
                    BlogPost = blogPost,
                    Type = request.Type
                };

                serviceResult.data = responseModel;
                serviceResult.resultType = ServiceResultType.Success;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception:UserBlogPost/GetByType");
                serviceResult.message = e.Message;
                serviceResult.resultType = ServiceResultType.Fail;
            }

            return serviceResult;
        }

        public ServiceResult<UserBlogPostByTypeResponseModel> DeleteByType(UserBlogPostByTypeRequestModel request)
        {
            var serviceResult = new ServiceResult<UserBlogPostByTypeResponseModel>();
            try
            {
                var blogPost =
                    _blogPostRepository.GetSingle(b => b.IsActive && !b.IsDeleted && b.Id == request.BlogPostId);
                var userBlogPostByType = _userBlogPostRepository.GetSingle(b =>
                    b.IsActive && !b.IsDeleted && b.UserId == request.UserId && b.BlogPostId == request.BlogPostId &&
                    b.BlogType == request.Type);
                if (userBlogPostByType != null)
                {
                    userBlogPostByType.IsActive = false;
                    userBlogPostByType.IsDeleted = true;

                    _userBlogPostRepository.UpdateWithCommit(userBlogPostByType);
                }

                var responseModel = new UserBlogPostByTypeResponseModel()
                {
                    UserId = request.UserId,
                    BlogPost = blogPost,
                    Type = request.Type
                };

                serviceResult.data = responseModel;
                serviceResult.resultType = ServiceResultType.Success;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception:UserBlogPost/GetByType");
                serviceResult.message = e.Message;
                serviceResult.resultType = ServiceResultType.Fail;
            }

            return serviceResult;
        }

        public ServiceResult<bool> GetBlogPostByTypeExistence(UserBlogPostByTypeRequestModel request)
        {
            var serviceResult = new ServiceResult<bool>();
            try
            {
                var userBlogPostByType = _userBlogPostRepository.GetSingle(b =>
                    b.IsActive && !b.IsDeleted && b.UserId == request.UserId && b.BlogPostId == request.BlogPostId &&
                    b.BlogType == request.Type);

                serviceResult.data = userBlogPostByType != null;
                serviceResult.resultType = ServiceResultType.Success;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception:UserBlogPost/GetByType");
                serviceResult.message = e.Message;
                serviceResult.resultType = ServiceResultType.Fail;
            }

            return serviceResult;
        }
    }
}
