using FullStackAssignment.Application.DTOs.UserDTOs;
using FullStackAssignment.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;


namespace FullStackAssignment.Application.IReposetories
{
    public interface IJwtServiceReposetory
    {
        AuthenticationResponse CreateJwtToken(User user);
        ClaimsPrincipal? GetJwtPrincipal(string? token);
        AuthenticationResponse CreateAccessTokenOnly(User user, string existingRefreshToken, DateTime? existingRefreshExpiry);
    }
}
