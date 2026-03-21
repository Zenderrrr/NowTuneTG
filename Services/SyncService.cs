using NowTuneTG.Models;

namespace NowTuneTG.Services;

public class SyncService
{
    private readonly SpotifyService _spotifyService;
    private readonly BioFormatter _bioFormatter;
    private readonly TelegramProfileService _telegramProfileService;

    public SyncService(
        SpotifyService spotifyService,
        BioFormatter bioFormatter,
        TelegramProfileService telegramProfileService)
    {
        _spotifyService = spotifyService;
        _bioFormatter = bioFormatter;
        _telegramProfileService = telegramProfileService;
    }

    public async Task RunAsync()
    {
        string? lastBio = null;

        while (true)
        {
            try
            {
                NowPlaying? nowPlaying = await _spotifyService.GetNowPlayingAsync();
                string bio = _bioFormatter.Format(nowPlaying);

                if (bio != lastBio)
                {
                    await _telegramProfileService.UpdateBioAsync(bio);
                    lastBio = bio;
                }
                else
                {
                    Console.WriteLine("[NO CHANGE]");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] {ex.Message}");
            }

            await Task.Delay(TimeSpan.FromSeconds(20));
        }
    }
}