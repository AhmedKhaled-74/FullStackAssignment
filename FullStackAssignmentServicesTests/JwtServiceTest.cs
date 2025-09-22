using AutoFixture;
using FluentAssertions;
using FullStackAssignment.Application.DTOs.UserDTOs;
using FullStackAssignment.Application.IReposetories;
using FullStackAssignment.Application.Services;
using FullStackAssignment.Domain.Entites;
using Moq;
using System.Security.Claims;
using Xunit;

namespace FullStackAssignment.Tests.Services
{
    public class JwtServiceTests
    {
        private readonly Mock<IJwtServiceReposetory> _jwtRepoMock;
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly JwtService _jwtService;
        private readonly IFixture _fixture;

        public JwtServiceTests()
        {
            _jwtRepoMock = new Mock<IJwtServiceReposetory>();
            _userRepoMock = new Mock<IUserRepository>();
            _jwtService = new JwtService(_jwtRepoMock.Object, _userRepoMock.Object);
            _fixture = new Fixture();
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>()
                .ToList()
                .ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [Fact]
        public async Task RefreshToken_NullToken_ShouldThrowArgumentNullException()
        {
            // Arrange
            TokenModel? token = null;

            // Act
            Func<Task> act = async () => await _jwtService.RefreshToken(token!);

            // Assert
            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task RefreshToken_InvalidPrincipal_ShouldThrowArgumentException()
        {
            // Arrange
            var token = _fixture.Create<TokenModel>();
            _jwtRepoMock.Setup(r => r.GetJwtPrincipal(token.Token)).Returns((ClaimsPrincipal?)null);

            // Act
            Func<Task> act = async () => await _jwtService.RefreshToken(token);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("Invalid Jwt access token");
        }

        [Fact]
        public async Task RefreshToken_NoEmailClaim_ShouldThrowArgumentException()
        {
            // Arrange
            var token = _fixture.Create<TokenModel>();
            var identity = new ClaimsIdentity(new List<Claim>()); // no email
            var principal = new ClaimsPrincipal(identity);
            _jwtRepoMock.Setup(r => r.GetJwtPrincipal(token.Token)).Returns(principal);

            // Act
            Func<Task> act = async () => await _jwtService.RefreshToken(token);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("Invalid Jwt access token no email claim");
        }

        [Fact]
        public async Task RefreshToken_UserNotFound_ShouldThrowArgumentException()
        {
            // Arrange
            var token = _fixture.Build<TokenModel>()
                .With(t => t.RefreshToken, "refresh123")
                .Create();

            var claims = new List<Claim> { new Claim(ClaimTypes.Email, "test@test.com") };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);

            _jwtRepoMock.Setup(r => r.GetJwtPrincipal(token.Token)).Returns(principal);
            _userRepoMock.Setup(r => r.FindUserByEmailAsync("test@test.com")).ReturnsAsync((User?)null);

            // Act
            Func<Task> act = async () => await _jwtService.RefreshToken(token);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("Invalid Jwt access refreshtoken");
        }

        [Fact]
        public async Task RefreshToken_InvalidRefreshToken_ShouldThrowArgumentException()
        {
            // Arrange
            var token = _fixture.Build<TokenModel>()
                .With(t => t.RefreshToken, "invalidToken")
                .Create();

            var claims = new List<Claim> { new Claim(ClaimTypes.Email, "test@test.com") };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);

            var user = _fixture.Build<User>()
                .With(u => u.EmailAddress, "test@test.com")
                .With(u => u.RefreshToken, "validToken")
                .With(u => u.RefreshTokenExpiration, DateTime.UtcNow.AddMinutes(5))
                .Create();

            _jwtRepoMock.Setup(r => r.GetJwtPrincipal(token.Token)).Returns(principal);
            _userRepoMock.Setup(r => r.FindUserByEmailAsync("test@test.com")).ReturnsAsync(user);

            // Act
            Func<Task> act = async () => await _jwtService.RefreshToken(token);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("Invalid Jwt access refreshtoken");
        }

        [Fact]
        public async Task RefreshToken_ExpiredRefreshToken_ShouldThrowArgumentException()
        {
            // Arrange
            var token = _fixture.Build<TokenModel>()
                .With(t => t.RefreshToken, "validToken")
                .Create();

            var claims = new List<Claim> { new Claim(ClaimTypes.Email, "test@test.com") };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);

            var user = _fixture.Build<User>()
                .With(u => u.EmailAddress, "test@test.com")
                .With(u => u.RefreshToken, "validToken")
                .With(u => u.RefreshTokenExpiration, DateTime.UtcNow.AddMinutes(-1)) // expired
                .Create();

            _jwtRepoMock.Setup(r => r.GetJwtPrincipal(token.Token)).Returns(principal);
            _userRepoMock.Setup(r => r.FindUserByEmailAsync("test@test.com")).ReturnsAsync(user);

            // Act
            Func<Task> act = async () => await _jwtService.RefreshToken(token);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("Invalid Jwt access refreshtoken");
        }

        [Fact]
        public async Task RefreshToken_ValidRequest_ShouldReturnAuthenticationResponse()
        {
            // Arrange
            var token = _fixture.Build<TokenModel>()
                .With(t => t.RefreshToken, "validToken")
                .Create();

            var claims = new List<Claim> { new Claim(ClaimTypes.Email, "test@test.com") };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);

            var user = _fixture.Build<User>()
                .With(u => u.EmailAddress, "test@test.com")
                .With(u => u.RefreshToken, "validToken")
                .With(u => u.RefreshTokenExpiration, DateTime.UtcNow.AddMinutes(10))
                .Create();

            var expectedResponse = _fixture.Create<AuthenticationResponse>();

            _jwtRepoMock.Setup(r => r.GetJwtPrincipal(token.Token)).Returns(principal);
            _userRepoMock.Setup(r => r.FindUserByEmailAsync("test@test.com")).ReturnsAsync(user);
            _jwtRepoMock.Setup(r => r.CreateAccessTokenOnly(user, user.RefreshToken!, user.RefreshTokenExpiration))
                .Returns(expectedResponse);

            // Act
            var result = await _jwtService.RefreshToken(token);

            // Assert
            result.Should().Be(expectedResponse);
        }
    }
}
