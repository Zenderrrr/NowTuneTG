using NowTuneTG.Services;

Console.WriteLine("NowTuneTG started");

var spotifyService = new SpotifyService();
var bioFormatter = new BioFormatter();
var telegramProfileService = new TelegramProfileService();
var syncService = new SyncService(spotifyService, bioFormatter, telegramProfileService);

await syncService.RunAsync();