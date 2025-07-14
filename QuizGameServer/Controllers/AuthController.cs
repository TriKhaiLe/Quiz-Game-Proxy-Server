using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace QuizGameServer.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;

        public AuthController(IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _httpClientFactory = httpClientFactory;
            _config = config;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var supabaseUrl = _config["Supabase:Url"];
            var supabaseApiKey = _config["Supabase:AnonKey"];
            var client = _httpClientFactory.CreateClient();
            var url = $"{supabaseUrl}/auth/v1/token?grant_type=password";
            var body = new
            {
                email = request.Email,
                password = request.Password
            };
            var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
            client.DefaultRequestHeaders.Add("apikey", supabaseApiKey);
            var response = await client.PostAsync(url, content);
            var respContent = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, respContent);
            return Content(respContent, "application/json");
        }
    }

    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
