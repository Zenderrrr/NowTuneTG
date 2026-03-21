using NowTuneTG.Models;
using SpotifyAPI.Web;

namespace NowTuneTG.Services;

public class SpotifyService
{
    private readonly SpotifySettings _settings;
    private SpotifyClient? _spotifyClient;

    public SpotifyService(SpotifySettings settings)
    {
        _settings = settings;
    }

    public string GetLoginUrl()
    {
        var request = new LoginRequest(
            new Uri(_settings.RedirectUri),
            _settings.ClientId,
            LoginRequest.ResponseType.Code)
        {
            Scope = new[]
            {
                Scopes.UserReadCurrentlyPlaying,
                Scopes.UserReadPlaybackState
            }
        };

        return request.ToUri().ToString();
    }

    public async Task ExchangeCodeAsync(string code)
    {
        var tokenResponse = await new OAuthClient().RequestToken(
            new AuthorizationCodeTokenRequest(
                _settings.ClientId,
                _settings.ClientSecret,
                code,
                new Uri(_settings.RedirectUri)));

        _spotifyClient = new SpotifyClient(tokenResponse.AccessToken);
    }

    public async Task<NowPlaying?> GetNowPlayingAsync()
    {
        if (_spotifyClient == null)
            throw new InvalidOperationException("Spotify client is not authorized");

        var playback = await _spotifyClient.Player.GetCurrentPlayback();

        if (playback?.Item is not FullTrack track)
            return null;

        string artistNames = string.Join(", ", track.Artists.Select(a => a.Name));

        return new NowPlaying
        {
            TrackName = track.Name,
            ArtistName = artistNames,
            IsPlaying = playback.IsPlaying
        };
    }
}