﻿using DWorldProject.Models.Request;
using DWorldProject.Models.Response;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DWorldProject.Services.Abstact
{
    public interface IBlogPostService
    {
        Task<ServiceResult<List<BlogPostResponseModel>>> Get();
        Task<ServiceResult<BlogPostResponseModel>> GetById(int id);
        ServiceResult<BlogPostResponseModel> Update(BlogPostRequestModel blogPostModel);
        ServiceResult<BlogPostResponseModel> Add(BlogPostRequestModel blogPostModel);
        ServiceResult<BlogPostResponseModel> Delete(int id);
    }
}
