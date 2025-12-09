using System.Security.Authentication;
using System.Security.Claims;
using Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions;

public static class ClaimsPrinciplesExtensions
{
    public static async Task<AppUser> GetUserByEmail(
        this UserManager<AppUser> userManager,
        ClaimsPrincipal user
    )
    {
        var userResult = await userManager.FindByEmailAsync(user.GetEmail());

        if (userResult == null)
            throw new AuthenticationException("User not found");

        return userResult;
    }

    public static string GetEmail(this ClaimsPrincipal user)
    {
        var email = user.FindFirstValue(ClaimTypes.Email);

        if (email == null)
            throw new AuthenticationException("Email claim not found");

        return email;
    }

    public static async Task<AppUser> GetUserByEmailWithAddress(
        this UserManager<AppUser> userManager,
        ClaimsPrincipal user
    )
    {
        var userResult = await userManager
            .Users.Include(u => u.Address)
            .FirstOrDefaultAsync(u => u.Email == user.GetEmail());

        if (userResult == null)
            throw new AuthenticationException("User not found");

        return userResult;
    }
}
