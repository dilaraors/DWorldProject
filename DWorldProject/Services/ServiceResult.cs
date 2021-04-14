using System;

namespace DWorldProject.Services
{
    public class ServiceResult<T>
    {
        public T Data { get; set; }
        public ServiceResultType ResultType { get; set; }
        public String Message { get; set; }
        public int? ErrorCode { get; set; }
    }

    public enum ServiceResultType
    {
        Fail = 0,
        Success = 1
    }
}
