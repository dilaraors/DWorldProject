using AutoMapper;
using DWorldProject.Data.Entities;
using DWorldProject.Data.Entities.Account;
using DWorldProject.Models.Request;
using DWorldProject.Models.Response;
using DWorldProject.Models.ViewModel;

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
            CreateMap<TopicModel, Topic>();
            CreateMap<Topic, TopicModel>();
            CreateMap<Section, SectionResponseModel>();
            CreateMap<Section, SectionModel>();
            CreateMap<SectionModel, Section>();
        }
    }
}
