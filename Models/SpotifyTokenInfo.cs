namespace NowTuneTG.Models;

public class SpotifyTokenInfo
{
    public string AccessToken { get; set; } = "";
    public string RefreshToken { get; set; } = "";
    public int ExpiresIn { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public bool IsExpired()
    {
        return DateTime.UtcNow >= CreatedAtUtc.AddSeconds(ExpiresIn - 60);
    }
}