using System;
using System.Collections.Generic;

namespace PlayList.Util
{
    public sealed class Song : IEquatable<Song>
    {
        public string Title { get; set; }
        public string Album { get; set; }
        public string Artist { get; set; }
        public TimeSpan Duration { get; set; }
        public string Path { get; set; } // 179
        public string AlbumArtPath { get; set; }
        public int Size { get; set; } // 001
        public DateTime DateModified { get; set; } // 003
        public DateTime DateCreated { get; set; } // 004
        public string PrimaryGenre { get; set; } // Appended field
        public string Genre { get; set; } // 016
        public string Rating { get; set; } // 019

        public string Category { get; set; }
        public List<string> Playlists { get; set; }

        public override int GetHashCode()
        {
            return Path.ToLower().GetHashCode();
        }

        public override bool Equals(object right)
        {
            if (ReferenceEquals(right, null))
                return false;
            if (ReferenceEquals(this, right))
                return true;

            return GetType() == right.GetType() && Equals(right as Song);
        }

        public bool Equals(Song other)
        {
            return (other.GetHashCode() == GetHashCode());
        }

        public override string ToString()
        {
            return Artist + " - " + Title;
        }

        public static bool operator ==(Song first, Song second)
        {
            return first != null && first.Equals(second);
        }

        public static bool operator !=(Song first, Song second)
        {
            return !(first == second);
        }

    }
}
