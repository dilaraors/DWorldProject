using DWorldProject.Models.Response;
using DWorldProject.Models.ViewModel;
using System.Collections.Generic;

namespace DWorldProject.Services.Abstact
{
    public interface ITopicService
    { 
        ServiceResult<List<TopicResponseModel>> Get();
        ServiceResult<TopicResponseModel> GetById(int id);
        ServiceResult<bool> Add(TopicModel model);
        ServiceResult<bool> Edit(TopicModel model);
        ServiceResult<bool> Delete(int id);
        ServiceResult<List<TopicResponseModel>> GetBySectionId(int sectionId);
    }
}
