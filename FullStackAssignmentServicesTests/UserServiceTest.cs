using AutoFixture;
using FluentAssertions;
using Moq;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;
using FullStackAssignment.Application.DTOs.UserDTOs;
using FullStackAssignment.Application.IReposetories;
using FullStackAssignment.Application.IServices;
using FullStackAssignment.Application.Services;
using FullStackAssignment.Domain.Entites;
using FullStackAssignment.Application.Mappers;

namespace FullStackAssignment.Tests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly Mock<IJwtServiceReposetory> _jwtRepoMock;
        private readonly UserService _userService;
        private readonly IFixture _fixture;

        public UserServiceTests()
        {
            _userRepoMock = new Mock<IUserRepository>();
            _jwtRepoMock = new Mock<IJwtServiceReposetory>();
            _userService = new UserService(_userRepoMock.Object, _jwtRepoMock.Object);

            _fixture = new Fixture();
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        #region RegisterAsync
        [Fact]
        public async Task RegisterAsync_NullRegisterDto_ShouldThrowArgumentNullException()
        {
            RegisterDTO? dto = null;

            Func<Task> act = async () => await _userService.RegisterAsync(dto);

            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task RegisterAsync_UserNameExists_ShouldThrowArgumentException()
        {
            var dto = _fixture.Build<RegisterDTO>().Create();
            var user = _fixture.Build<User>().With(u => u.UserName, dto.UserName).Create();

            _userRepoMock.Setup(r => r.FindUserByUserNameAsync(dto.UserName!))
                .ReturnsAsync(user);

            Func<Task> act = async () => await _userService.RegisterAsync(dto);

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*UserName*");
        }

        [Fact]
        public async Task RegisterAsync_EmailExists_ShouldThrowArgumentException()
        {
            var dto = _fixture.Build<RegisterDTO>().Create();
            var user = _fixture.Build<User>().With(u => u.EmailAddress, dto.Email).Create();

            _userRepoMock.Setup(r => r.FindUserByUserNameAsync(dto.UserName!))
                .ReturnsAsync((User?)null);
            _userRepoMock.Setup(r => r.FindUserByEmailAsync(dto.Email!))
                .ReturnsAsync(user);

            Func<Task> act = async () => await _userService.RegisterAsync(dto);

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*Email*");
        }

        [Fact]
        public async Task RegisterAsync_ValidUser_ShouldReturnAuthenticationResponse()
        {
            var dto = _fixture.Build<RegisterDTO>().Create();
            _userRepoMock.Setup(r => r.FindUserByUserNameAsync(dto.UserName!))
                .ReturnsAsync((User?)null);
            _userRepoMock.Setup(r => r.FindUserByEmailAsync(dto.Email!))
                .ReturnsAsync((User?)null);

            var fakeUser = dto.ToUserEntity();
            _userRepoMock.Setup(r => r.RegisterUserAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);

            var fakeJwt = _fixture.Build<AuthenticationResponse>()
                .With(a => a.Token, "jwt-token")
                .With(a => a.RefreshToken, "refresh-token")
                .Create();

            _jwtRepoMock.Setup(j => j.CreateJwtToken(It.IsAny<User>()))
                .Returns(fakeJwt);

            var result = await _userService.RegisterAsync(dto);

            result.Token.Should().Be("jwt-token");
            result.RefreshToken.Should().Be("refresh-token");
            _userRepoMock.Verify(r => r.RegisterUserAsync(It.IsAny<User>()), Times.Once);
        }
        #endregion

        #region LoginAsync
        [Fact]
        public async Task LoginAsync_NullLoginDto_ShouldThrowArgumentNullException()
        {
            LoginDTO? dto = null;

            Func<Task> act = async () => await _userService.LoginAsync(dto);

            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task LoginAsync_InvalidUser_ShouldThrowArgumentException()
        {
            var dto = _fixture.Build<LoginDTO>().Create();

            _userRepoMock.Setup(r => r.LoginUserAsync(dto.UserNameOrEmail!, dto.Password!))
                .ReturnsAsync((User?)null);

            Func<Task> act = async () => await _userService.LoginAsync(dto);

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("UserName or Password is invalid");
        }

        [Fact]
        public async Task LoginAsync_ValidUser_ShouldReturnAuthenticationResponse()
        {
            var dto = _fixture.Build<LoginDTO>().Create();
            var fakeUser = _fixture.Build<User>()
                .With(u => u.EmailAddress, dto.UserNameOrEmail)
                .Create();

            _userRepoMock.Setup(r => r.LoginUserAsync(dto.UserNameOrEmail!, dto.Password!))
                .ReturnsAsync(fakeUser);

            var fakeJwt = _fixture.Build<AuthenticationResponse>()
                .With(a => a.Token, "jwt-login-token")
                .Create();

            _jwtRepoMock.Setup(j => j.CreateJwtToken(fakeUser))
                .Returns(fakeJwt);

            var result = await _userService.LoginAsync(dto);

            result.Token.Should().Be("jwt-login-token");
        }
        #endregion

        #region LogoutAsync
        [Fact]
        public async Task LogoutAsync_NullToken_ShouldThrowArgumentNullException()
        {
            string? token = null;

            Func<Task> act = async () => await _userService.LogoutAsync(token);

            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task LogoutAsync_InvalidPrincipal_ShouldThrowArgumentException()
        {
            var token = _fixture.Build<string>().Create();

            _jwtRepoMock.Setup(j => j.GetJwtPrincipal(token))
                .Returns((ClaimsPrincipal?)null);

            Func<Task> act = async () => await _userService.LogoutAsync(token);

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("Invalid Jwt access token");
        }

        [Fact]
        public async Task LogoutAsync_NoEmailClaim_ShouldThrowArgumentException()
        {
            var token = _fixture.Build<string>().Create();
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity());

            _jwtRepoMock.Setup(j => j.GetJwtPrincipal(token))
                .Returns(claimsPrincipal);

            Func<Task> act = async () => await _userService.LogoutAsync(token);

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("Invalid Jwt access token no email claim");
        }

        [Fact]
        public async Task LogoutAsync_UserNotFound_ShouldThrowArgumentException()
        {
            var token = _fixture.Build<string>().Create();
            var claimsPrincipal = new ClaimsPrincipal(
                new ClaimsIdentity(new[] { new Claim(ClaimTypes.Email, "test@example.com") })
            );

            _jwtRepoMock.Setup(j => j.GetJwtPrincipal(token))
                .Returns(claimsPrincipal);
            _userRepoMock.Setup(r => r.FindUserByEmailAsync("test@example.com"))
                .ReturnsAsync((User?)null);

            Func<Task> act = async () => await _userService.LogoutAsync(token);

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("User not found");
        }

        [Fact]
        public async Task LogoutAsync_Valid_ShouldClearRefreshToken()
        {
            var token = _fixture.Build<string>().Create();
            var fakeUser = _fixture.Build<User>()
                .With(u => u.EmailAddress, "test@example.com")
                .With(u => u.RefreshToken, "refresh-token")
                .Create();

            var claimsPrincipal = new ClaimsPrincipal(
                new ClaimsIdentity(new[] { new Claim(ClaimTypes.Email, fakeUser.EmailAddress) })
            );

            _jwtRepoMock.Setup(j => j.GetJwtPrincipal(token))
                .Returns(claimsPrincipal);
            _userRepoMock.Setup(r => r.FindUserByEmailAsync(fakeUser.EmailAddress))
                .ReturnsAsync(fakeUser);

            await _userService.LogoutAsync(token);

            fakeUser.RefreshToken.Should().BeNull();
            _userRepoMock.Verify(r => r.UpdateUserAsync(fakeUser), Times.Once);
        }
        #endregion

        #region FindUserByEmail
        [Fact]
        public async Task FindUserByEmail_Null_ShouldThrowArgumentNullException()
        {
            Func<Task> act = async () => await _userService.FindUserByEmail(null);

            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task FindUserByEmail_NotExists_ShouldReturnFalse()
        {
            _userRepoMock.Setup(r => r.FindUserByEmailAsync("ghost@example.com"))
                .ReturnsAsync((User?)null);

            var result = await _userService.FindUserByEmail("ghost@example.com");

            result.Should().BeFalse();
        }

        [Fact]
        public async Task FindUserByEmail_Exists_ShouldReturnTrue()
        {
            var user = _fixture.Build<User>().Create();
            _userRepoMock.Setup(r => r.FindUserByEmailAsync(user.EmailAddress))
                .ReturnsAsync(user);

            var result = await _userService.FindUserByEmail(user.EmailAddress);

            result.Should().BeTrue();
        }
        #endregion

        #region FindUserByUserName
        [Fact]
        public async Task FindUserByUserName_Null_ShouldThrowArgumentNullException()
        {
            Func<Task> act = async () => await _userService.FindUserByUserName(null);

            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task FindUserByUserName_NotExists_ShouldReturnFalse()
        {
            _userRepoMock.Setup(r => r.FindUserByUserNameAsync("ghost"))
                .ReturnsAsync((User?)null);

            var result = await _userService.FindUserByUserName("ghost");

            result.Should().BeFalse();
        }

        [Fact]
        public async Task FindUserByUserName_Exists_ShouldReturnTrue()
        {
            var user = _fixture.Build<User>().Create();
            _userRepoMock.Setup(r => r.FindUserByUserNameAsync(user.UserName))
                .ReturnsAsync(user);

            var result = await _userService.FindUserByUserName(user.UserName);

            result.Should().BeTrue();
        }
        #endregion
    }
}
