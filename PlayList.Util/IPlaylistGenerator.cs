using System.Collections.Generic;

namespace PlayList.Util
{
    public interface IPlaylistGenerator
    {
        IEnumerable<Song> GeneratePlayList(IEnumerable<Song> songProvider, int maximumSongCount, IEnumerable<Song> alwaysAdd);
    }
}