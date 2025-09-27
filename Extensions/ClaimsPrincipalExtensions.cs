using System;
using System.Security.Claims;

namespace ExpenseTracker.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        /// <summary>
        /// Tries to read the integer user id from the NameIdentifier claim.
        /// </summary>
        public static bool TryGetUserId(this ClaimsPrincipal? user, out int userId)
        {
            userId = 0;
            if (user == null)
                return false;

            var claim = user.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null || !int.TryParse(claim.Value, out userId))
                return false;

            return true;
        }

        /// <summary>
        /// Gets the user id from the NameIdentifier claim or throws if not present/invalid.
        /// </summary>
        public static int GetUserId(this ClaimsPrincipal user)
        {
            if (user.TryGetUserId(out var id))
                return id;

            throw new InvalidOperationException("User id claim is missing or invalid.");
        }
    }
}
