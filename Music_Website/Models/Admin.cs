using System.ComponentModel.DataAnnotations;

namespace Music_Website.Models
{
    public class Admin
    {
        public int Admin_ID { get; set; }

        public string? Admin_Name { get; set; }

        public string? Admin_lastname { get; set; }

        public DateTime? Admin_dateofbirth { get; set; }
        
        public string? Username { get; set; }
        
        public string? Password { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        [Phone]
        public string? Phone { get; set; }
        
        public bool? isprimary_admin { get; set; }
        //relation
        public ICollection<Music>? musics { get; set; }
        public ICollection<Music_Video>? music_Videos { get; set; }
        public ICollection<Remix>? remixes { get; set; }
    }
}
