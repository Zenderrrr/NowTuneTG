using NowTuneTG.Models;

namespace NowTuneTG.Services;


public class BioFormatter{
    public string Format(NowPlaying? NowPlaying)
    {
        if(NowPlaying is null || !NowPlaying.IsPlaying)
            return "|| Nothing playing";

        return $"{NowPlaying.ArtistName} - {NowPlaying.TrackName}";
    }
}