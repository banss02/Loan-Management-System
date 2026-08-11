using System.Security.Claims;
using LoanAPI.Services;

namespace LoanAPI.Helper
{
    public class AccessControlService
    {
        private readonly LoanService _loanService;

        public AccessControlService(LoanService loanService)
        {
            _loanService = loanService;
        }

        public static bool IsAdmin(ClaimsPrincipal user) =>user.FindFirst(ClaimTypes.Role)?.Value == "Admin";

        public static int? GetMyCustomerId(ClaimsPrincipal user) =>int.TryParse(user.FindFirst("CustomerId")?.Value, out var id) ? id : null;

        public static int? GetMyUserId(ClaimsPrincipal user) =>int.TryParse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var id) ? id : null;

        public async Task<bool> CanAccessCustomer(ClaimsPrincipal user, int customerId)
        {
            if (!IsAdmin(user))
                return GetMyCustomerId(user) == customerId;

            var adminUserId = GetMyUserId(user);
            if (adminUserId == null)
                return false;

            var visibleIds = await _loanService.GetCustomerIdsAssignedToAdmin(adminUserId.Value);
            return visibleIds.Contains(customerId);
        }

        public async Task<List<int>> GetVisibleCustomerIds(ClaimsPrincipal user)
        {
            if (!IsAdmin(user))
            {
                var myCustomerId = GetMyCustomerId(user);
                return myCustomerId.HasValue ? new List<int> { myCustomerId.Value } : new List<int>();
            }

            var adminUserId = GetMyUserId(user);
            if (adminUserId == null)
                return new List<int>();

            return await _loanService.GetCustomerIdsAssignedToAdmin(adminUserId.Value);
        }
    }
}