namespace Music_Website.Models
{
    public class AdminDashboardViewModel
    {
        public int Music_count { get; set; }

        public int singer_count { get; set; }

        public int music_video_count { get; set; }

        public int remix_count { get; set; }
        
        public int albums_count { get;set; }

        public int not_accpect_paremt_comment_count { get; set; }

        public int accpect_parent_comment_count { get; set; }

        public int parent_comment_count { get; set; }
        public int not_accpect_child_comment_count { get; set; }
        public int accpect_child_comment_count { get; set; }
        public int child_comment_count { get; set; }

        public int comment_count { get; set; }
        public int notseeing_contactus { get; set; }

        public int seeing_contactus { get; set; }
         
    }
}
