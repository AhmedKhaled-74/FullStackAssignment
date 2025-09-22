using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace FullStackAssignment.IntegrationTests
{

    using FluentAssertions;
    using global::FullStackAssignment.Application.DTOs.UserDTOs;
    using System.Net;
    using System.Net.Http.Json;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Xunit;

    namespace FullStackAssignment.Tests.Integration
    {
        public class AccountControllerIntegrationTest : IClassFixture<WebAppFactory>
        {
            private readonly HttpClient _client;
            private const string BaseUrl = "/api/v1/account";

            public AccountControllerIntegrationTest(WebAppFactory factory)
            {
                _client = factory.CreateClient();
            }

            [Fact]
            public async Task Register_ShouldReturnOk_WhenValidUser()
            {
                var registerDto = new
                {
                    UserName = "testuser",
                    Email = "testuser@example.com",
                    Password = "StrongPass123@",
                    ConfirmPassword = "StrongPass123@"
                };

                var response = await _client.PostAsJsonAsync($"{BaseUrl}/register", registerDto);

                response.StatusCode.Should().Be(HttpStatusCode.OK);

                var json = await response.Content.ReadFromJsonAsync<JsonElement>();
                json.Should().NotBeNull();
                json.GetProperty("token").GetString().Should().NotBeNull();
                json.GetProperty("refreshToken").GetString().Should().NotBeNull();

            }

            [Fact]
            public async Task Login_ShouldReturnOk_WhenValidCredentials()
            {
                var registerDto = new
                {
                    UserName = "loginuser",
                    Email = "loginuser@example.com",
                    Password = "@Aa33333366",
                    ConfirmPassword = "@Aa33333366"
                };
                await _client.PostAsJsonAsync($"{BaseUrl}/register", registerDto);

                var loginDto = new
                {
                    UsernameOrEmail = "loginuser@example.com",
                    Password = "@Aa33333366"
                };

                var response = await _client.PostAsJsonAsync($"{BaseUrl}/login", loginDto);

                response.StatusCode.Should().Be(HttpStatusCode.OK);

                var json = await response.Content.ReadFromJsonAsync<JsonElement>();
                json.GetProperty("token").GetString().Should().NotBeNull();
                json.GetProperty("refreshToken").GetString().Should().NotBeNull();
            }

            [Fact]
            public async Task RefreshToken_ShouldReturnOk_WhenValidRefreshToken()
            {
                var registerDto = new
                {
                    UserName = "refreshuseragain",
                    Email = "refresha@example.com",
                    Password = "@Aa33333366",
                    ConfirmPassword = "@Aa33333366"
                };
                var registerResponse = await _client.PostAsJsonAsync($"{BaseUrl}/register", registerDto);
                registerResponse.StatusCode.Should().Be(HttpStatusCode.OK);

                var registerResult = await registerResponse.Content.ReadFromJsonAsync<JsonElement>();
                registerResult.GetProperty("token").GetString().Should().NotBeNull();
                registerResult.GetProperty("refreshToken").GetString().Should().NotBeNull();

                var tokenModel = new TokenModel
                {
                    Token = registerResult.GetProperty("token").GetString(),
                    RefreshToken = registerResult.GetProperty("refreshToken").GetString()
                };


                var response = await _client.PostAsJsonAsync($"{BaseUrl}/refresh-token", tokenModel);
                response.StatusCode.Should().Be(HttpStatusCode.OK);

                var json = await response.Content.ReadFromJsonAsync<JsonElement>();
                json.GetProperty("token").GetString().Should().NotBeNull();
                json.GetProperty("refreshToken").GetString().Should().NotBeNull();
            }

            [Fact]
            public async Task CheckEmailExists_ShouldReturnOk()
            {
                var email = "checkme@example.com";
                await _client.PostAsJsonAsync($"{BaseUrl}/register", new
                {
                    UserName = "refreshuser",
                    Email = email,
                    Password = "@Aa33333366",
                    ConfirmPassword = "@Aa33333366"
                });

                var response = await _client.GetAsync($"{BaseUrl}/register/check-email?email={email}");

                response.StatusCode.Should().Be(HttpStatusCode.OK);

                var exists = await response.Content.ReadFromJsonAsync<bool>();
                exists.Should().BeTrue();
            }

            [Fact]
            public async Task CheckUserNameExists_ShouldReturnOk()
            {
                var username = "checkusername";
                await _client.PostAsJsonAsync($"{BaseUrl}/register", new
                {
                    UserName = username,
                    Email = "refresh@example.com",
                    Password = "@Aa33333366",
                    ConfirmPassword = "@Aa33333366"
                });

                var response = await _client.GetAsync($"{BaseUrl}/register/check-username?username={username}");

                response.StatusCode.Should().Be(HttpStatusCode.OK);

                var exists = await response.Content.ReadFromJsonAsync<bool>();
                exists.Should().BeTrue();
            }
        }
    }

}
