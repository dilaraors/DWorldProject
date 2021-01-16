using System;

namespace DWorldProject.Services
{
    public class ServiceResult<T>
    {
        public T data { get; set; }
        public ServiceResultType resultType { get; set; }
        public String message { get; set; }
    }

    public enum ServiceResultType
    {
        Fail = 0,
        Success = 1
    }
}
