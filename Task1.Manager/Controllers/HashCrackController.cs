using Microsoft.AspNetCore.Mvc;
using CrackHash.Models;
using System.Xml.Serialization;
using System.Text;
using System.Collections.Concurrent;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

[ApiController]
[Route("api/hash")]
public class HashCrackController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _workerUrl;
    private readonly List<string> _workerUrls; 
    private readonly string _managerUrl;

    private readonly ConcurrentQueue<(string RequestId, CrackHashRequest Task)> _taskQueue = new();

    private const int MaxQueueSize = 100;

    public HashCrackController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;

        _workerUrl = configuration["AppSettings:WorkerUrl"];
        _workerUrls = configuration.GetSection("AppSettings:WorkerUrls").Get<List<string>>();
        _managerUrl = configuration["AppSettings:ManagerUrl"];

        if (string.IsNullOrEmpty(_workerUrl) && (_workerUrls == null || _workerUrls.Count == 0))
        {
            throw new InvalidOperationException("Неверный WorkerUrl или WorkerUrls в конфигурации.");
        }
        Task.Run(ProcessTasksAsync);
    }

    [HttpPost("crack")]
    public IActionResult CrackHash([FromBody] CrackHashRequest clientRequest)
    {
        try
        {
            string requestId = Guid.NewGuid().ToString();

            if (_taskQueue.Count >= MaxQueueSize)
            {
                return StatusCode(429, "Too many requests. Please try again later.");
            }
            _taskStates.TryAdd(requestId, new TaskState { Status = TaskStatus.IN_PROGRESS });

            _taskQueue.Enqueue((requestId, clientRequest));

            return Ok(new { requestId });
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Ошибка при добавлении задачи в очередь: {ex.Message}");
            return StatusCode(500, "Internal Server Error");
        }
    }

    private async Task ProcessTasksAsync()
    {
        while (true)
        {
            try
            {
                if (_taskQueue.TryDequeue(out var queueItem))
                {
                    string requestId = queueItem.RequestId;
                    CrackHashRequest task = queueItem.Task;

                    await ProcessTaskAsync(requestId, task);
                }
                else
                {
                    await Task.Delay(100);
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Ошибка при обработке задачи: {ex.Message}");
            }
        }
    }

    private async Task ProcessTaskAsync(string requestId, CrackHashRequest task)
    {
        int partCount = 33; 

        for (int partNumber = 0; partNumber < partCount; partNumber++)
        {
            try
            {
                var workerRequest = new CrackHashManagerRequest
                {
                    RequestId = requestId,
                    Hash = task.Hash,
                    MaxLength = task.MaxLength,
                    PartNumber = partNumber,
                    PartCount = partCount
                };

                var xmlRequest = SerializeToXml(workerRequest);

                var content = new StringContent(xmlRequest, Encoding.UTF8, "application/xml");

                var httpClient = _httpClientFactory.CreateClient();

                string workerUrl = _workerUrls?[partNumber % _workerUrls.Count] ?? _workerUrl;

                Console.WriteLine($"Отправка части {partNumber} на воркер: {workerUrl}");

                _ = httpClient.PostAsync(workerUrl, content);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Ошибка при отправке части задачи {partNumber}: {ex.Message}");
            }
        }
    }

    private string SerializeToXml(object obj)
    {
        var xmlSerializer = new XmlSerializer(obj.GetType());
        using var stringWriter = new Utf8StringWriter();
        xmlSerializer.Serialize(stringWriter, obj);
        return stringWriter.ToString();
    }

    [HttpGet("status")]
    public IActionResult GetStatus([FromQuery] string requestId)
    {
        if (!_taskStates.TryGetValue(requestId, out var taskState))
        {
            return NotFound("Задача не найдена.");
        }

        double progress = (double)taskState.CompletedParts / 33 * 100;

        return Ok(new
        {
            status = taskState.Status.ToString(),
            progress = Math.Round(progress, 2), 
            data = taskState.Results
        });
    }

    [HttpPatch("update-status")]
    public IActionResult UpdateTaskStatus([FromBody] TaskUpdateRequest updateRequest)
    {
        if (!_taskStates.TryGetValue(updateRequest.RequestId, out var taskState))
        {
            return NotFound("Задача не найдена.");
        }

        try
        {
            lock (taskState) 
            {
                taskState.Results.AddRange(updateRequest.Results);

                taskState.CompletedParts++;

                if (taskState.CompletedParts >= updateRequest.PartCount)
                {
                    taskState.Status = TaskStatus.READY; 
                }
                else
                {
                    taskState.Status = TaskStatus.PARTIAL_READY; 
                }
            }
        }
        catch (Exception)
        {
            lock (taskState)
            {
                taskState.Status = TaskStatus.ERROR; 
            }
        }

        return Ok();
    }

    private static readonly ConcurrentDictionary<string, TaskState> _taskStates = new();

    public class TaskState
    {
        public TaskStatus Status { get; set; } = TaskStatus.IN_PROGRESS;
        public List<string> Results { get; set; } = new();
        public int CompletedParts { get; set; } = 0;
    }

    public class TaskUpdateRequest
    {
        public required string RequestId { get; set; }
        public required List<string> Results { get; set; }
        public required int PartCount { get; set; }
    }

        public class CrackHashRequest
    {
        public required string Hash { get; set; }
        public required int MaxLength { get; set; }
    }
}

