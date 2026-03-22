using NowTuneTG.Models;
using SpotifyAPI.Web;

namespace NowTuneTG.Services;

public class SpotifyService
{
    private readonly SpotifySettings _settings;
    private readonly SpotifyTokenStore _tokenStore;

    private SpotifyClient? _spotifyClient;
    private SpotifyTokenInfo? _tokenInfo;

    public SpotifyService(SpotifySettings settings, SpotifyTokenStore tokenStore)
    {
        _settings = settings;
        _tokenStore = tokenStore;
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

    public async Task<bool> TryRestoreSessionAsync()
    {
        _tokenInfo = await _tokenStore.LoadAsync();

        if (_tokenInfo == null || string.IsNullOrWhiteSpace(_tokenInfo.RefreshToken))
            return false;

        await RefreshAccessTokenAsync();
        return true;
    }

    public async Task ExchangeCodeAsync(string code)
    {
        var tokenResponse = await new OAuthClient().RequestToken(
            new AuthorizationCodeTokenRequest(
                _settings.ClientId,
                _settings.ClientSecret,
                code,
                new Uri(_settings.RedirectUri)));

        _tokenInfo = new SpotifyTokenInfo
        {
            AccessToken = tokenResponse.AccessToken,
            RefreshToken = tokenResponse.RefreshToken,
            ExpiresIn = tokenResponse.ExpiresIn,
            CreatedAtUtc = DateTime.UtcNow
        };

        _spotifyClient = new SpotifyClient(_tokenInfo.AccessToken);

        await _tokenStore.SaveAsync(_tokenInfo);
    }

    private async Task RefreshAccessTokenAsync()
    {
        if (_tokenInfo == null)
            throw new InvalidOperationException("Token info is missing");

        var tokenResponse = await new OAuthClient().RequestToken(
            new AuthorizationCodeRefreshRequest(
                _settings.ClientId,
                _settings.ClientSecret,
                _tokenInfo.RefreshToken));

        _tokenInfo.AccessToken = tokenResponse.AccessToken;
        _tokenInfo.ExpiresIn = tokenResponse.ExpiresIn;
        _tokenInfo.CreatedAtUtc = DateTime.UtcNow;

        if (!string.IsNullOrWhiteSpace(tokenResponse.RefreshToken))
            _tokenInfo.RefreshToken = tokenResponse.RefreshToken;

        _spotifyClient = new SpotifyClient(_tokenInfo.AccessToken);

        await _tokenStore.SaveAsync(_tokenInfo);
    }

    private async Task EnsureAuthorizedAsync()
    {
        if (_tokenInfo == null)
            throw new InvalidOperationException("Spotify is not authorized");

        if (_spotifyClient == null || _tokenInfo.IsExpired())
            await RefreshAccessTokenAsync();
    }

    public async Task<NowPlaying?> GetNowPlayingAsync()
    {
        await EnsureAuthorizedAsync();

        var currentlyPlaying = await _spotifyClient!.Player.GetCurrentlyPlaying(
            new PlayerCurrentlyPlayingRequest(PlayerCurrentlyPlayingRequest.AdditionalTypes.Track)
        );

        if (currentlyPlaying == null)
            return null;

        if (currentlyPlaying.Item is not FullTrack track)
            return null;

        string artistNames = string.Join(", ", track.Artists.Select(a => a.Name));

        return new NowPlaying
        {
            TrackName = track.Name,
            ArtistName = artistNames,
            IsPlaying = currentlyPlaying.IsPlaying
        };
    }
}