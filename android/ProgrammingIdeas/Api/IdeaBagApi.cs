using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ProgrammingIdeas.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammingIdeas.Api
{
    internal class IdeaBagApi
    {
        private HttpClient client;
        private readonly string apiKey = AppResources.ApiKey;
        private readonly string dbUrl = AppResources.DatabaseUrl;
        private string token = string.Empty;

        private readonly Dictionary<string, string> loginErrors = new Dictionary<string, string>
        {
            {"INVALID_PASSWORD", "A user with that email and password was not found." },
            {"EMAIL_NOT_FOUND", "A user with that email address was not found." },
            {"USER_DISABLED", "The user account has been disabled by an administrator." },
            {"TOO_MANY_ATTEMPTS", "Too many login attempts. Please try again in a few minutes." }
        };

        private readonly Dictionary<string, string> signupErrors = new Dictionary<string, string>
        {
            {"INVALID_PASSWORD", "A user with that email and password was not found." },
            {"EMAIL_NOT_FOUND", "A user with that email address was not found." },
            {"USER_DISABLED", "The user account has been disabled by an administrator." },
            {"TOO_MANY_ATTEMPTS", "Too many login attempts. Please try again in a few minutes." }
        };

        private static IdeaBagApi singleton;

        public static IdeaBagApi Instance
        {
            get
            {
                if (singleton == null)
                    singleton = new IdeaBagApi();

                return singleton;
            }
        }

        private IdeaBagApi()
        {
            client = new HttpClient(new TokenDelegate(new HttpClientHandler()));
            client.Timeout = TimeSpan.FromSeconds(9);
        }

        public void SetAuthToken(string token) => this.token = token;

        public async Task<ApiResponse<LoginResponseData>> LoginAsync(LoginData data)
        {
            try
            {
                var response = await client.PostAsync($"https://www.googleapis.com/identitytoolkit/v3/relyingparty/verifyPassword?key={apiKey}",
                    new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var loginResponse = JsonConvert.DeserializeObject<LoginResponseData>(json);

                    return new ApiResponse<LoginResponseData>(loginResponse, string.Empty);
                }

                return new ApiResponse<LoginResponseData>(null, GetFirebaseErrorMessage(await response.Content.ReadAsStringAsync()));
            }
            catch (Exception e)
            {
                return new ApiResponse<LoginResponseData>(null, GetExceptionError(e));
            }
        }

        public async Task<ApiResponse<LoginResponseData>> RegisterAsync(LoginData data)
        {
            try
            {
                var response = await client.PostAsync($"https://www.googleapis.com/identitytoolkit/v3/relyingparty/signupNewUser?key={apiKey}",
                    new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    var loginResponse = JsonConvert.DeserializeObject<LoginResponseData>(await response.Content.ReadAsStringAsync());
                    return new ApiResponse<LoginResponseData>(loginResponse, string.Empty);
                }

                return new ApiResponse<LoginResponseData>(null, GetFirebaseErrorMessage(await response.Content.ReadAsStringAsync(), "signup"));
            }
            catch (Exception e)
            {
                return new ApiResponse<LoginResponseData>(null, GetExceptionError(e));
            }
        }

        public async Task<bool> DeleteCommentAsync(string dataId, string commentId)
        {
            try
            {
                var response = await client.DeleteAsync($"{dbUrl}/{dataId}/comments/{commentId}.json?auth={token}");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<ApiResponse<List<IdeaComment>>> GetCommentsAsync(string dataId)
        {
            try
            {
                var response = await client.GetAsync($"{dbUrl}/{dataId}/comments.json");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    if (json != null && json != "null")
                    {
                        var result = JObject.Parse(json);
                        var comments = GenerateComments(result);

                        return new ApiResponse<List<IdeaComment>>(comments, string.Empty);
                    }

                    return new ApiResponse<List<IdeaComment>>(new List<IdeaComment>(), string.Empty);
                }

                return new ApiResponse<List<IdeaComment>>(null, "Couldn't get comments. Please retry.");
            }
            catch (Exception e)
            {
                return new ApiResponse<List<IdeaComment>>(null, GetExceptionError(e));
            }
        }

        private List<IdeaComment> GenerateComments(JObject result)
        {
            List<IdeaComment> comments = new List<IdeaComment>();

            foreach (var pair in result)
            {
                var comment = new IdeaComment
                {
                    Author = (string)pair.Value["author"],
                    Comment = (string)pair.Value["comment"],
                    Created = (long)pair.Value["created"],
                    Id = pair.Key
                };

                comments.Add(comment);
            }

            return comments;
        }

        public async Task<ApiResponse<string>> PostCommentAsync(string dataId, IdeaComment comment)
        {
            try
            {
                var response = await client.PostAsync($"{dbUrl}/{dataId}/comments.json?auth={token}",
                    new StringContent(JsonConvert.SerializeObject(comment), Encoding.UTF8, "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    var data = JObject.Parse(await response.Content.ReadAsStringAsync());
                    return new ApiResponse<string>((string)data["name"], string.Empty);
                }

                return new ApiResponse<string>(null, await response.Content.ReadAsStringAsync());
            }
            catch (Exception e)
            {
                return new ApiResponse<string>(null, GetExceptionError(e));
            }
        }

        private string GetExceptionError(Exception e)
        {
            if (e is TaskCanceledException)
                return "Couldn't load data. Your connection might be too slow";

            return e.Message;
        }

        private string GetFirebaseErrorMessage(string errorCode, string action = "login")
        {
            if (action == "login")
            {
                if (loginErrors.ContainsKey(errorCode))
                    return loginErrors[errorCode];

                return errorCode;
            }
            else if (action == "signup")
            {
                if (signupErrors.ContainsKey(errorCode))
                    return signupErrors[errorCode];

                return errorCode;
            }

            return errorCode;
        }
    }
}