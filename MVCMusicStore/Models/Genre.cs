using System.Collections.Generic;

namespace MVCMusicStore.Models
{
    public class Genre
    {
        public Genre() : base() { }
        public Genre(int genreId, string name, string desc)
        {
            GenreId = genreId;
            Name = name;
            Description = desc;
        }
        public int GenreId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual List<Album> Albums { get; set; }
    }
}
