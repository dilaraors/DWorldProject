using DWorldProject.Data.Entities;
using DWorldProject.Utils.Enums;

namespace DWorldProject.Services.Abstact
{
    public interface IElasticLogService
    {
        void LogChange(OperationType type, BlogPost model);
        void CheckIndex();
    }
}
