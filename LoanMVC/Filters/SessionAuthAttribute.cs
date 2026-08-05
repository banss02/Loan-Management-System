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
                if (context.HttpContext.Request.Cookies.ContainsKey("SessionExpiredElsewhere"))
                {
                    context.HttpContext.Response.Cookies.Delete("SessionExpiredElsewhere");
                    context.Result = new RedirectToActionResult("Login", "Account", new { reason = "expired" });
                }
                else
                {
                    context.Result = new RedirectToActionResult("Login", "Account", null);
                }

                return;
            }

            if (!string.IsNullOrEmpty(Roles))
            {
                var userRole = session.GetString("Role") ?? "";
                var allowedRoles = Roles.Split(',', StringSplitOptions.TrimEntries);

                if (!allowedRoles.Contains(userRole, StringComparer.OrdinalIgnoreCase))
                {
                    context.Result = new ForbidResult();
                }
            }
        }
    }
}