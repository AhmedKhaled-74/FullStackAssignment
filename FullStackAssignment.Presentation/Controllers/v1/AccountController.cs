using Asp.Versioning;
using FullStackAssignment.Application.DTOs.UserDTOs;
using FullStackAssignment.Application.IServices;
using FullStackAssignment.Application.Mappers;
using FullStackAssignment.Bootstrapper.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FullStackAssignment.Presentation.Controllers.v1
{
    /// <summary>
    /// Provides endpoints for user account management, including registration, login, 
    /// email and username runtime check
    /// </summary>
    [Authorize]
    [ApiVersion("1.0")]
    public class AccountController : CustomControllerBase
    {
        private readonly ILogger<AccountController> _logger;
        private readonly IUserService _userService;
        private readonly IJwtService _jwtService;

        public AccountController(ILogger<AccountController> logger, IUserService userService, IJwtService jwtService)
        {
            _logger = logger;
            _userService = userService;
            _jwtService = jwtService;
        }

        /// <summary>
        /// Register new user
        /// </summary>
        [AllowAnonymous]
        [HttpPost]
        [Route("register")]
        public async Task<ActionResult> PostRegister(RegisterDTO registerDTO)
        {
            try
            {
                var authentication = await _userService.RegisterAsync(registerDTO);

                if (authentication == null)
                {
                    return Problem("Registeration Failed");
                }
                return Ok(authentication);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in registration");
                return ExceptionHandel(ex);
            }
        }

        /// <summary>
        /// Login user
        /// </summary>
        [AllowAnonymous]
        [HttpPost]
        [Route("login")]
        public async Task<ActionResult> Login(LoginDTO loginDTO)
        {
            try
            {
                var authentication = await _userService.LoginAsync(loginDTO);
                if (authentication == null)
                {
                    return Problem("Login failed");
                }
                return Ok(authentication);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in login");
                return ExceptionHandel(ex);
            }
        }

        /// <summary>
        /// logout from system
        /// </summary>
        /// <returns></returns>

        [HttpPost]
        [Route("logout")]
        public async Task<ActionResult> LogOut(string? token)
        {
            try
            {
                if (token == null)
                    return Unauthorized();

                await _userService.LogoutAsync(token);
                return Unauthorized(new { message = "LogOut Done" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in logout");
                return ExceptionHandel(ex);
            }
        }

        /// <summary>
        /// Generate New Token by refreshtoken
        /// </summary>
        /// <param name="token">exsist token</param>
        /// <returns>new authentication instance</returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("refresh-token")]

        public async Task<IActionResult> RefreshToken(TokenModel token)
        {
            try
            {
                if (token == null)
                {
                    return BadRequest("Invalid Client Request");

                }
                var authentication = await _jwtService.RefreshToken(token);
                if (authentication == null)
                    return BadRequest("Invalid Jwt access token");

                return Ok(authentication);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in refresh token");
                return ExceptionHandel(ex);
            }
        }

        /// <summary> /// 
        /// Check Email Exists 
        /// /// </summary> /// 
        /// <param name="email">email</param> 
        /// /// <returns>true or false</returns> 
        [AllowAnonymous]
        [HttpGet]
        [Route("register/check-email")]

        public async Task<IActionResult> CheckEmailExists(string? email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                    return BadRequest("email required");
                var IsUser = await _userService.FindUserByEmail(email);

                return Ok(IsUser);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Check Email Exists");
                return ExceptionHandel(ex);
            }
        }

        /// <summary> /// 
        /// Check Username Exists 
        /// /// </summary> /// 
        /// <param name="username">username</param> 
        /// /// <returns>true or false</returns> 
        [AllowAnonymous]
        [HttpGet]
        [Route("register/check-username")]
        public async Task<IActionResult> CheckUserNameExists(string? username)
        {
            try
            {
                if (string.IsNullOrEmpty(username))
                    return BadRequest("username required");
                var IsUser = await _userService.FindUserByUserName(username);

                return Ok(IsUser);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Check UserName Exists");
                return ExceptionHandel(ex);
            }
        }
    }
}
