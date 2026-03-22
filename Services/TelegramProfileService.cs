using TL;
using WTelegram;

namespace NowTuneTG.Services;

public class TelegramProfileService
{
    private readonly int _apiId;
    private readonly string _apiHash;

    private Client? _client;

    public TelegramProfileService(int apiId, string apiHash)
    {
        _apiId = apiId;
        _apiHash = apiHash;
    }

    private string? Config(string what)
    {
        return what switch
        {
            "api_id" => _apiId.ToString(),
            "api_hash" => _apiHash,

            "phone_number" => Prompt("Enter phone number: "),
            "verification_code" => Prompt("Enter Telegram code: "),
            "password" => Prompt("Enter 2FA password: "),

            _ => null
        };
    }

    private static string Prompt(string text)
    {
        Console.Write(text);
        return Console.ReadLine() ?? "";
    }

    public async Task InitAsync()
    {
        _client = new Client(Config);

        var user = await _client.LoginUserIfNeeded();
        Console.WriteLine($"Telegram login successful: {user}");
    }

    public async Task UpdateBioAsync(string text)
    {
        if (_client == null)
            throw new Exception("Telegram client not initialized");

        await _client.Account_UpdateProfile(about: text);
        Console.WriteLine($"[TG BIO UPDATED] {text}");
    }
}