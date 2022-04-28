using System;

namespace BLL.Tools
{
    public class ErrorRequestException : ApplicationException
    {
        public int ErrorCode { get; }

        public ErrorRequestException(int errorCode, string message = null) : base(message)
        {
            ErrorCode = errorCode;
        }

        public static ErrorRequestException NotFoundException(string message = null)
        {
            return new ErrorRequestException(404, message ?? "Resource not found.");
        }
        
        public static ErrorRequestException NotAuthorizedException(string message = null)
        {
            return new ErrorRequestException(401, message ?? "Not authorized.");
        }
        
        public static ErrorRequestException AccessForbiddenException(string message = null)
        {
            return new ErrorRequestException(403, message ?? "Access forbidden.");
        }
        
        public static ErrorRequestException InternalErrorException(string message = null)
        {
            return new ErrorRequestException(500, message ?? "Internal error.");
        }
    }
}