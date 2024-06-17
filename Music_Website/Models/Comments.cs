namespace Music_Website.Models
{
    public class Comments
    {
        public int CommentId { get; set; }

        public int? ParentId { get; set; }

        public string? Email_Address { get; set; }

        public string? Comment_Text { get; set; }

        public string? comment_Name { get;set; }

        public bool? Admin_Accepted { get; set; }

        public int? SongId { get; set; }
        public int? MusicVideoId { get; set; }

        public int? RemixID { get; set; }
        //relations

        public Comments? Parent { get; set; }

        public ICollection<Comments>? Child { get; set; }
        public Music? music { get; set; }

        public Music_Video? music_video { get; set; }

        public Remix? Remix { get; set; }



    }
}
