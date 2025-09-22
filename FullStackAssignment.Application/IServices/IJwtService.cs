using FullStackAssignment.Application.DTOs.UserDTOs;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FullStackAssignment.Application.IServices
{
    public interface IJwtService
    {
        Task<AuthenticationResponse> RefreshToken(TokenModel token);  
    }
}
