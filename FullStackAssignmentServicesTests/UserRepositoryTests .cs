using AutoFixture;
using FluentAssertions;
using FullStackAssignment.Application.Mappers;
using FullStackAssignment.Domain.Entites;
using FullStackAssignment.Infrastructure.DbContexts;
using FullStackAssignment.Infrastructure.Reposetories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Xunit;

namespace FullStackAssignment.Tests.Repositories
{
    public class UserRepositoryTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly UserRepository _userRepository;
        private readonly IFixture _fixture;

        public UserRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // unique db per test
                .Options;

            _context = new AppDbContext(options);
            _userRepository = new UserRepository(_context);

            _fixture = new Fixture();
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>()
                .ToList()
                .ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
        #region RegisterUser
        [Fact]
        public async Task RegisterUserAsync_ShouldAddUserToDatabase()
        {
            // Arrange
            var user = _fixture.Build<User>()
                .With(u => u.UserName, "testuser")
                .With(u => u.EmailAddress, "test@test.com")
                .With(u => u.HashedPassword, PasswordHelper.ToHashedPassword("P@ssword123"))
                .Create();

            // Act
            await _userRepository.RegisterUserAsync(user);

            // Assert
            var dbUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == "testuser");
            dbUser.Should().NotBeNull();
            dbUser!.EmailAddress.Should().Be("test@test.com");
        }
        #endregion

        #region LoginUser
        [Fact]
        public async Task LoginUserAsync_WithValidCredentials_ShouldReturnUser()
        {
            // Arrange
            var password = "P@ssword123";
            var hashed = PasswordHelper.ToHashedPassword(password);

            var user = new User
            {
                UserName = "validuser",
                EmailAddress = "valid@test.com",
                HashedPassword = hashed
            };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _userRepository.LoginUserAsync("validuser", password);

            // Assert
            result.Should().NotBeNull();
            result!.UserName.Should().Be("validuser");
        }

        [Fact]
        public async Task LoginUserAsync_WithInvalidPassword_ShouldReturnNull()
        {
            // Arrange
            var user = new User
            {
                UserName = "wrongpass",
                EmailAddress = "wrong@test.com",
                HashedPassword = PasswordHelper.ToHashedPassword("CorrectPass123")
            };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _userRepository.LoginUserAsync("wrongpass", "WrongPass456");

            // Assert
            result.Should().BeNull();
        }
        #endregion

        #region UpdateUser
        [Fact]
        public async Task UpdateUserAsync_ShouldUpdateUser()
        {
            // Arrange
            var user = new User
            {
                UserName = "updateuser",
                EmailAddress = "old@test.com",
                HashedPassword = PasswordHelper.ToHashedPassword("Pass123")
            };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            user.EmailAddress = "new@test.com";

            // Act
            await _userRepository.UpdateUserAsync(user);

            // Assert
            var dbUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == "updateuser");
            dbUser!.EmailAddress.Should().Be("new@test.com");
        }
        #endregion

        #region FindUser
        [Fact]
        public async Task FindUserByEmailAsync_ShouldReturnCorrectUser()
        {
            // Arrange
            var user = new User
            {
                UserName = "emailuser",
                EmailAddress = "email@test.com",
                HashedPassword = PasswordHelper.ToHashedPassword("Pass123")
            };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _userRepository.FindUserByEmailAsync("EMAIL@test.com");

            // Assert
            result.Should().NotBeNull();
            result!.UserName.Should().Be("emailuser");
        }

        [Fact]
        public async Task FindUserByUserNameAsync_ShouldReturnCorrectUser()
        {
            // Arrange
            var user = new User
            {
                UserName = "finduser",
                EmailAddress = "find@test.com",
                HashedPassword = PasswordHelper.ToHashedPassword("Pass123")
            };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _userRepository.FindUserByUserNameAsync("finduser");

            // Assert
            result.Should().NotBeNull();
            result!.EmailAddress.Should().Be("find@test.com");
        }
        #endregion

    }
}
