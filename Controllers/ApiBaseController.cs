using Microsoft.AspNetCore.Mvc;
using UWEServer.Responses;
using System.Linq.Expressions;
using System.Reflection;

namespace sl_APIs.Controller.Controller
{
    public class APIBaseController : ControllerBase
    {
        public PropertyInfo InfoOf<T>(Expression<Func<T>> ex)
        {
            return (PropertyInfo)((MemberExpression)ex.Body).Member;
        }
        protected ApiJsonResult APIException(Exception ex)
        {
            return new ApiJsonResult
            {
                code = 400,
                message = ex.Message,
                data = new { ERROR = ex.Message }
            };
        }
    }
}
