using System.Collections.Concurrent;
using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.SignalR;
using PromoCodes.Clients;
using PromoCodes.Models;
using ILogger = PromoCodes.Utils.ILogger;
namespace PromoCodes.Hubs;

public class DiscountHub : Hub<IDiscountClient>
{
    private static readonly ConcurrentDictionary<string, Discount> DiscountCodes = new();
    private static readonly SemaphoreSlim Lock = new(1, 1);
    private const string StorageFile = "discount_codes.json";

    private readonly ILogger _logger;
    private readonly IValidator<GenerateRequest> _generateRequestValidator;
    private readonly IValidator<UseCodeRequest> _useCodeRequestValidator;
    
    public DiscountHub(ILogger logger, IValidator<GenerateRequest> generateRequestValidator, IValidator<UseCodeRequest> useCodeRequestValidator)
    {
        _logger = logger;
        _generateRequestValidator = generateRequestValidator;
        _useCodeRequestValidator = useCodeRequestValidator;
    }
    
    public override async Task OnConnectedAsync()
    {
        _logger.Log($"User {Context.ConnectionId} connected.");
        await LoadDiscountCodes();
        await base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.Log($"User {Context.ConnectionId} disconnected.");
        return base.OnDisconnectedAsync(exception);
    }
    
    public Task TextMe(string message)
    {
        _logger.Log($"TextMe: {message}");
        return Task.CompletedTask;
    }
    
    public async Task GenerateCodes(GenerateRequest request)
    {
        var validationResult = await _generateRequestValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            _logger.LogErrors(nameof(GenerateCodes), validationResult.Errors);
            await ProduceCodeGenerationResponse(false);
            return;
        }
       
        var newCodes = new List<string>();

        var rng = new Random();
        while (newCodes.Count < request.Count)
        {
            var code = GenerateRandomCode(request.Length, rng);
            var discount = new Discount()
            {
                Code = code
            };
            
            if (DiscountCodes.TryAdd(code, discount))
            {
                newCodes.Add(code);
            }
        }
        
        await SaveDiscountCodes();
        await ProduceCodeGenerationResponse(true);
    }

    public async Task UseCode(UseCodeRequest request)
    {
        var validationResult = await _useCodeRequestValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            _logger.LogErrors(nameof(UseCode), validationResult.Errors);
            await ProduceUseCodeResponse((byte) UseCodeResponseType.InvalidRequest);
            return;
        }
        
        if (DiscountCodes.TryRemove(request.Code, out _))
        {
            await SaveDiscountCodes();
            await ProduceUseCodeResponse((byte)UseCodeResponseType.Success);
        }
        else
        {
            await ProduceUseCodeResponse((byte)UseCodeResponseType.CodeNotFound);
        }
    }
    
    private static async Task LoadDiscountCodes()
    {
        var codes = Enumerable.Empty<Discount>();
        await Lock.WaitAsync();
        try
        {
            if (File.Exists(StorageFile))
            {
                var fileContent = await File.ReadAllTextAsync(StorageFile);

                if (!string.IsNullOrEmpty(fileContent))
                {
                    codes = JsonSerializer.Deserialize<IEnumerable<Discount>>(fileContent);
                }
                foreach (var code in codes)
                {
                    DiscountCodes[code.Code] = code;
                }
            }
            else
            {
                File.Create(StorageFile);
            }
        }
        finally
        {
            Lock.Release();
        }
    }
    
    private static string GenerateRandomCode(int length, Random rng)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Range(0, length).Select(_ => chars[rng.Next(chars.Length)]).ToArray());
    }
    
    private static async Task SaveDiscountCodes()
    {
        await Lock.WaitAsync();
        try
        {
            await File.WriteAllTextAsync(StorageFile, JsonSerializer.Serialize(DiscountCodes.Values));
        }
        finally
        {
            Lock.Release();
        }
    }

    private async Task ProduceCodeGenerationResponse(bool result)
    {
        await Clients.Client(Context.ConnectionId).CodesGenerated(new GenerateResponse{Result = result});
    }
    
    private async Task ProduceUseCodeResponse(byte result)
    {
        await Clients.Client(Context.ConnectionId).CodeUsed(new UseCodeResponse{Result = result});
    }
}