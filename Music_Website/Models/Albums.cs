namespace Music_Website.Models
{
    public class Albums
    {
        public int AlbumId { get; set; }

        public string? AlbumName { get; set; }

        public int AlbumCount { get; set; }





        //relation
        public Singer? main_singer { get; set; }
        public ICollection<Singer>? singers { get; set; }
        public ICollection<Music>? musics { get; set; }


    }
}
