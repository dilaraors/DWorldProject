using DWorldProject.Models.Response;
using System.Collections.Generic;

namespace DWorldProject.Services.Abstact
{
    public interface ITopicService
    { 
        ServiceResult<List<TopicResponseModel>> Get();
        ServiceResult<TopicResponseModel> GetById(int id);
        ServiceResult<List<string>> GetSections();
    }
}
