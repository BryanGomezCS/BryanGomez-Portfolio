// This is just creating a simple Music Manager struct to help implement more than one song in the game

using System.Collections.Generic;

namespace Snake.Core
{
    public struct MusicTrack
    {
        public string displayName;  // like hype, etc
        public string Audio;        // the name of the folder that contains the songs
        public string credit;       // for giving credit to artist
    }

    public static class MusicManager
    {
        public static List<MusicTrack> Playlist = new List<MusicTrack>
        {
            new MusicTrack { displayName = "Retro", Audio = "Audio/retro_song", credit = "Music: Retro Game Arcade by moodmode (via Pixabay)"},
            new MusicTrack { displayName = "Hype", Audio = "Audio/hype_song", credit = "Music: Bounce It by Next Route (via Audio Library)" },
            new MusicTrack { displayName = "Chill", Audio = "Audio/lofi_song", credit = "Music: Pixel Duck by Jhony Grimes (via Audio Library)" },
        };
    }
}
