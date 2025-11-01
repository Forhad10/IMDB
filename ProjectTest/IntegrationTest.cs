using IMDB.Data.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace IMDB.IntegrationTests
{
    public class WebServiceTests
    {
        private const string BaseUrl = "https://localhost:7198"; 
        private const string UsersApi = BaseUrl + "/api/User";

        // ------------------------ Tests ------------------------

        [Fact]
        public async Task ApiUsers_Signup_WithValidData_Created()
        {
            var newUser = new
            {
                Username = "testuser",
                Email = "test@example.com",
                Password = "Test@123"
            };

            var (user, statusCode) = await PostData($"{UsersApi}/signup", newUser);

            Assert.Equal(HttpStatusCode.Created, statusCode);
            Assert.NotNull(user["user"]);
            var userObj = user["user"];
            Assert.Equal(newUser.Username, userObj["username"].ToString());
            Assert.Equal(newUser.Email, userObj["email"].ToString());

            // Cleanup
            await DeleteData($"{UsersApi}/{userObj["userId"]}");
        }

        [Fact]
        public async Task ApiUsers_Signup_WithExistingEmail_BadRequest()
        {
            var existingUser = new
            {
                Username = "existinguser",
                Email = "existing@example.com",
                Password = "Test@123"
            };
            var (user, statusCodes) = await PostData($"{UsersApi}/signup", existingUser);
            var userObj = user["user"];

            var newUser = new
            {
                Username = "newuser",
                Email = "existing@example.com", // Same email
                Password = "Test@123"
            };

            var (_, statusCode) = await PostData($"{UsersApi}/signup", newUser, false);

            Assert.Equal(HttpStatusCode.BadRequest, statusCode);

            // Cleanup
            await DeleteData($"{UsersApi}/{userObj["userId"]}");




        }

        [Fact]
        public async Task ApiUsers_Signin_WithValidCredentials_OkAndToken()
        {
            // Signup first
            var newUser = new
            {
                Username = "signintest",
                Email = "signin@example.com",
                Password = "Test@123"
            };
            await PostData($"{UsersApi}/signup", newUser);

            // Signin
            var credentials = new
            {
                Email = newUser.Email,
                Password = newUser.Password
            };

            var (response, statusCode) = await PostData($"{UsersApi}/signin", credentials);

            Assert.Equal(HttpStatusCode.OK, statusCode);
            Assert.NotNull(response["data"]["token"]);
            Assert.NotNull(response["data"]["user"]);

            var userObj = response["data"]["user"];
            Assert.Equal(newUser.Username, userObj["username"].ToString());
            Assert.Equal(newUser.Email, userObj["email"].ToString());

            // Cleanup
            await DeleteData($"{UsersApi}/{userObj["userId"]}");
        }


        [Fact]
        public async Task ApiUsers_Signin_WithInvalidCredentials_Unauthorized()
        {
            var credentials = new
            {
                Email = "nonexistent@example.com",
                Password = "wrongpassword"
            };

            var (_, statusCode) = await PostData($"{UsersApi}/signin", credentials, false);

            Assert.Equal(HttpStatusCode.BadRequest, statusCode);
        }

        // ------------------------ Helper Methods ------------------------

        private async Task<(JObject, HttpStatusCode)> PostData(string url, object content, bool expectSuccess = true)
        {
            using var client = new HttpClient();
            var response = await client.PostAsync(
                url,
                new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json")
            );

            var data = await response.Content.ReadAsStringAsync();

            if (expectSuccess && !response.IsSuccessStatusCode)
            {
                throw new Exception($"Request failed with status {response.StatusCode}: {data}");
            }

            JObject json = null;
            if (!string.IsNullOrWhiteSpace(data))
            {
                json = JObject.Parse(data);
            }

            return (json, response.StatusCode);
        }

        private async Task<HttpStatusCode> DeleteData(string url)
        {
            using var client = new HttpClient();
            var response = await client.DeleteAsync(url);
            return response.StatusCode;
        }
    }
}
