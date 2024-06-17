using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations;
using System.Composition.Convention;
using System.Runtime.InteropServices;

namespace Music_Website.Models
{
    public class Music
    {
        public int SongId { get; set; }

        public int? AlbumId { get; set; }
        public string? Song_Name { get; set; }

        public string? Song_Description { get; set; }

        public DateTime Song_Date { get; set; } 

        public string? Music_Style { get; set; }
        public string? Song_FileName { get; set; }
        public string? Image_Filename { get; set; }

        public IFormFile? Image_File { get; set; }
        public IFormFile? Song_File { get; set; }

        public int? comment_status { get; set; }
        public bool? in_main_index { get; set; }

        //relation

        public Admin? admin { get; set; }
        public ICollection<Remix>? remixes { get; set; }

        public ICollection<Comments>? comments { get; set; }
        public ICollection<Singer>? singers { get; set; }
        public Albums? album { get; set; }

    }
}
