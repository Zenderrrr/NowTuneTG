using Microsoft.Extensions.Configuration;
using NowTuneTG.Models;
using NowTuneTG.Services;

Console.WriteLine("NowTuneTG started");

IConfiguration config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

SpotifySettings spotifySettings = config
    .GetSection("Spotify")
    .Get<SpotifySettings>() ?? throw new Exception("Spotify settings not found");

TelegramSettings telegramSettings = config
    .GetSection("Telegram")
    .Get<TelegramSettings>() ?? throw new Exception("Telegram settings not found");

var tokenStore = new SpotifyTokenStore();
var spotifyService = new SpotifyService(spotifySettings, tokenStore);
var bioFormatter = new BioFormatter();

var telegramProfileService = new TelegramProfileService(
    telegramSettings.ApiId,
    telegramSettings.ApiHash
);

await telegramProfileService.InitAsync();

var syncService = new SyncService(
    spotifyService,
    bioFormatter,
    telegramProfileService
);

bool restored = await spotifyService.TryRestoreSessionAsync();

if (!restored)
{
    string loginUrl = spotifyService.GetLoginUrl();

    Console.WriteLine("Open this URL in your browser:");
    Console.WriteLine(loginUrl);
    Console.WriteLine();
    Console.Write("Paste the ?code= value here: ");

    string? code = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(code))
        throw new Exception("Code was empty");

    await spotifyService.ExchangeCodeAsync(code);
    Console.WriteLine("Spotify connected and tokens saved.");
}
else
{
    Console.WriteLine("Spotify session restored from file.");
}

await telegramProfileService.UpdateBioAsync("test bio from c#");

Console.WriteLine("Starting sync loop...");
await syncService.RunAsync();