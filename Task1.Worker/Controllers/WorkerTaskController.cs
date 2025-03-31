using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using CrackHash.Models;
using Task1.Worker.Services;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;

[ApiController]
[Route("api/hash")]
public class WorkerTaskController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly string _managerUrl;

    public WorkerTaskController(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _httpClient.Timeout = TimeSpan.FromSeconds(10);

        _managerUrl = configuration["AppSettings:ManagerUrl"];
    }

   private async Task SendResultsToManager(string requestId, List<string> results, int partCount)
{
    try
    {
        await Task.Delay(10000); 

        var updateRequest = new TaskUpdateRequest
        {
            RequestId = requestId,
            Results = results,
            PartCount = partCount
        };

        var jsonContent = JsonConvert.SerializeObject(updateRequest);

        var content = new StringContent(jsonContent, Encoding.UTF8);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        var response = await _httpClient.PatchAsync(_managerUrl, content);

        if (!response.IsSuccessStatusCode)
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            Console.Error.WriteLine($"Ошибка при отправке результатов менеджеру: {response.StatusCode}, {responseBody}");
        }
    }
    catch (TaskCanceledException)
    {
        Console.Error.WriteLine("Таймаут при отправке результатов менеджеру.");
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"Ошибка при отправке результатов менеджеру: {ex.Message}");
    }
}

    [HttpPost("task")]
    public async Task<IActionResult> ProcessTask([FromBody] CrackHashManagerRequest request)
    {
        var results = BruteForceService.FindHash(request.Hash, request.MaxLength, request.PartNumber, request.PartCount);

        Console.WriteLine($"Найдено {results.Count} совпадений: {string.Join(", ", results)}");

        await SendResultsToManager(request.RequestId, results, request.PartCount);
        return Ok(new { request.RequestId, results });
    }
}

public class TaskUpdateRequest
{
    public required string RequestId { get; set; }
    public required List<string> Results { get; set; }
    public required int PartCount { get; set; }
}