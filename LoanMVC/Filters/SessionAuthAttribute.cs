using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LoanMVC.Filters
{
    //   [SessionAuth]                 -> just requires the user to be logged in
    //   [SessionAuth(Roles = "Admin")] -> requires logged in AND Role == "Admin"

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class SessionAuthAttribute : Attribute, IAuthorizationFilter
    {
        public string Roles { get; set; } = ""; 

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var session = context.HttpContext.Session;
            var token = session.GetString("Token");

            if (string.IsNullOrEmpty(token))
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
                return;
            }

            if (!string.IsNullOrEmpty(Roles))
            {
                var userRole = session.GetString("Role") ?? "";
               if (!userRole.Equals(Roles, StringComparison.OrdinalIgnoreCase))
                {
                       context.Result = new ForbidResult();
                }
                }
            }
        }
    }

