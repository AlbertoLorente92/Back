using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

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
        private readonly IConfiguration _configuration;

        public WeatherForecastController(ILogger<WeatherForecastController> logger
            , IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
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
                return BadRequest("Value of 'day' should be between 1 and 30");
            }

            return Ok(new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(day)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            });
        }

        [HttpGet("DoesTheWeatherMatch", Name = "DoesTheWeatherMatch")]
        public ActionResult DoesTheWeatherMatch(string encryptedWeather)
        {
            if (string.IsNullOrEmpty(encryptedWeather))
            {
                return BadRequest("Encrypted payload is missing.");
            }

            try
            {
                var AesSecretKey = _configuration.GetValue<string>("AesSecretKey");
                var AesIV = _configuration.GetValue<string>("AesIV");

                byte[] encryptedBytes = Convert.FromBase64String(encryptedWeather);

                string jsonString = DecryptWithAes(encryptedBytes, AesSecretKey, AesIV);

                var weatherForecast = JsonConvert.DeserializeObject<WeatherForecast>(jsonString);

                if (weatherForecast == null)
                {
                    return BadRequest("Decrypted payload is invalid.");
                }

                return Ok(new { IsCorrect = true });
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }

        private string DecryptWithAes(byte[] cipherText, string key, string iv)
        {
            using Aes aesAlg = Aes.Create();
            aesAlg.Key = Encoding.UTF8.GetBytes(key);
            aesAlg.IV = Encoding.UTF8.GetBytes(iv);

            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using MemoryStream msDecrypt = new(cipherText);
            using CryptoStream csDecrypt = new(msDecrypt, decryptor, CryptoStreamMode.Read);
            using StreamReader srDecrypt = new(csDecrypt);
            return srDecrypt.ReadToEnd();
        }
    }
}
