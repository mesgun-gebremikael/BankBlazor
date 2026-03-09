using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace BankBlazorAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BankHolidayController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public BankHolidayController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        [HttpGet("next-scottish")]
        public async Task<IActionResult> GetNextScottishHoliday()
        {
            var url = "https://www.gov.uk/bank-holidays.json";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            using var document = JsonDocument.Parse(json);

            var events = document
                .RootElement
                .GetProperty("scotland")
                .GetProperty("events");

            var today = DateTime.Today;

            var nextHoliday = events
                .EnumerateArray()
                .Select(e => new
                {
                    Title = e.GetProperty("title").GetString(),
                    Date = DateTime.Parse(e.GetProperty("date").GetString()!)
                })
                .Where(x => x.Date >= today)
                .OrderBy(x => x.Date)
                .FirstOrDefault();

            if (nextHoliday == null)
            {
                return NotFound("Ingen kommande bank holiday hittades.");
            }

            return Ok(new
            {
                holiday = nextHoliday.Title,
                date = nextHoliday.Date.ToString("yyyy-MM-dd")
            });
        }
    }
}
