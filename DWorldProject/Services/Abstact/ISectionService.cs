using DWorldProject.Models.Response;
using DWorldProject.Models.ViewModel;
using System.Collections.Generic;

namespace DWorldProject.Services.Abstact
{
    public interface ISectionService
    {
        ServiceResult<List<SectionResponseModel>> Get();
        ServiceResult<bool> Add(SectionModel model);
        ServiceResult<bool> Edit(SectionModel model);
        ServiceResult<bool> Delete(int id);
    }
}
