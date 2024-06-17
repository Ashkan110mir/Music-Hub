namespace Music_Website.Models
{
    public class Singer
    {
        public int SingerId { get; set; }

        public string? SingerName { get; set; }

        public string? Singer_Lastname { get; set; }

        public string? artistName { get; set; }

        public string? profile_picture_name { get; set; }

        public IFormFile Picture_File { get; set; }

        //relation
        public ICollection<Albums>? is_main_singer { get; set; }
        public ICollection<Albums>? Albums { get; set; }
        public ICollection<Music>? musics { get; set; }
        public ICollection<Music_Video>? music_Videos { get; set; }

    }
}
