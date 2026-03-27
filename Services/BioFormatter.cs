using NowTuneTG.Models;

namespace NowTuneTG.Services;

public class BioFormatter
{
    public string Format(NowPlaying? nowPlaying)
    {
        string text = nowPlaying == null || !nowPlaying.IsPlaying
            ? "⏸ Nothing playing"
            : $"Now playing: {nowPlaying.ArtistName} - {nowPlaying.TrackName}";

        const int maxLength = 70;

        if (text.Length > maxLength)
            text = text[..maxLength];

        return text;
    }
}