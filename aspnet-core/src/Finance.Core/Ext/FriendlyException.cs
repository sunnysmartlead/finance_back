using Abp.UI;
using System.Net;

namespace Finance
{
    /// <summary>
    /// Friendly exception
    /// </summary>
    public class FriendlyException : UserFriendlyException
    {
        public FriendlyException(int code, string message, string details)
        : this((int)HttpStatusCode.BadRequest, code, message, details)
        {
        }

        public FriendlyException(HttpStatusCode httpCode, int code, string message, string details)
        : this((int)httpCode, code, message, details)
        {
        }

        public FriendlyException(int httpCode, int code, string message, string details)
        : base(code, message, details)
        {
            HttpCode = httpCode;
        }

        public FriendlyException(int httpCode, string message)
       : base(message)
        {
            HttpCode = httpCode;
        }
        public FriendlyException(string message) : base(message)
        {
        }
        public FriendlyException(string message, string details) : base(message, details)
        {
        }
        public virtual int HttpCode { get; set; } = 200;
    }
}
