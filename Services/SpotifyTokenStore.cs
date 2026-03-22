using System.Text.Json;
using NowTuneTG.Models;

namespace NowTuneTG.Services;

public class SpotifyTokenStore
{
    private const string FilePath = "spotify_tokens.json";

    public async Task SaveAsync(SpotifyTokenInfo tokenInfo)
    {
        string json = JsonSerializer.Serialize(tokenInfo, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        await File.WriteAllTextAsync(FilePath, json);
    }

    public async Task<SpotifyTokenInfo?> LoadAsync()
    {
        if (!File.Exists(FilePath))
            return null;

        string json = await File.ReadAllTextAsync(FilePath);
        return JsonSerializer.Deserialize<SpotifyTokenInfo>(json);
    }
}