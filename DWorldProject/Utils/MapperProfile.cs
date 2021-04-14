using AutoMapper;
using DWorldProject.Data.Entities;
using DWorldProject.Models.Request;
using DWorldProject.Models.Response;

namespace DWorldProject.Utils
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<BlogPost, BlogPostRequestModel>();
            CreateMap<BlogPost, BlogPostResponseModel>();
            CreateMap<BlogPostRequestModel, BlogPost>();
            CreateMap<BlogPostResponseModel, BlogPost>();
            CreateMap<UserResponseModel, User>();
            CreateMap<User, UserResponseModel>();
            CreateMap<UserBlogPostResponseModel, UserBlogPost>();
            CreateMap<UserBlogPost, UserBlogPostResponseModel>();
            CreateMap<Topic, TopicResponseModel>();
        }
    }
}
