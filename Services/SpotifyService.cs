using NowTuneTG.Models;

namespace NowTuneTG.Services;

public class SpotifyService
{
    private bool _toggle;

    public Task<NowPlaying?> GetNowPlayingAsync()
    {
        _toggle = !_toggle;

        NowPlaying result = _toggle
            ? new NowPlaying
            {
                TrackName = "After Hours",
                ArtistName = "The Weeknd",
                IsPlaying = true
            }
            : new NowPlaying
            {
                TrackName = "Blinding Lights",
                ArtistName = "The Weeknd",
                IsPlaying = true
            };

        return Task.FromResult<NowPlaying?>(result);
    }
}