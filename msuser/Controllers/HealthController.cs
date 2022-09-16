using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace msuser.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {

        [HttpGet("healthcheck")]
        public IActionResult healthcheck() => Ok($"SqlConnection: {AppSettingsConfig.SqlConnection} TokenKey: {AppSettingsConfig.TokenKey}");
    }
}
