namespace NowTuneTG.Services;

public class TelegramProfileService
{
    public Task UpdateBioAsync(string text)
    {
        Console.WriteLine($"[MOCK TG BIO UPDATE] {text}");
        return Task.CompletedTask;
    }
}