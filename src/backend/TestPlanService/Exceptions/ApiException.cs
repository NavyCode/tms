using System;

namespace TestPlanService.Exceptions
{
    public class ApiException : Exception
    {
        public ApiException(int code, string message) : base(message)
        {
            HResult = code;
        }
    }
}
