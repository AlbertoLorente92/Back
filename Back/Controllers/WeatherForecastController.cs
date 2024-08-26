using Microsoft.AspNetCore.Mvc;

namespace Back.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet("GetWeather", Name = "GetWeather")]
        public ActionResult GetWeather()
        {
            return Ok(
                Enumerable
                    .Range(1, 5)
                    .Select(
                        index => new WeatherForecast {
                            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                            TemperatureC = Random.Shared.Next(-20, 55),
                            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
                        }
                    )
                    .ToArray()
            );
        }

        [HttpGet("GetWeatherForDay", Name = "GetWeatherForDay")]
        public ActionResult GetWeatherForDay(int day)
        {
            if (day <= 0 || day > 30)
            {
                return BadRequest();
            }

            return Ok(new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(day)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            });
        }

        [HttpGet("DoesTheWeatherMatch", Name = "DoesTheWeatherMatch")]
        public ActionResult DoesTheWeatherMatch(WeatherForecast weatherForecast)
        {
            if (weatherForecast == null)
            {
                return BadRequest();
            }

            return Ok(new { IsCorrect = true });
        }
    }
}
