using Music_Website.Models;

namespace Music_Website.Data.Comment_Data
{
    public interface ICommentData
    {
        public int Comment_Count();

        public int child_comment_count();

        public int accpeted_child_comment_count();

        public int not_accpeted_child_comment_count();

        public int parent_comment_count();
        public int accepted_parent_comment_count();
        public int not_accepted_parent_comment_count();

        public List<Comments> Get_all_comment_for_admin_page();

        public List<Comments> Get_Music_comment(int musicid);
        public List<Comments> Get_Mv_Comment(int mvid);

        public List<Comments> Get_Remix_Comment(int remixid);
        public bool add_comment(Comments comments);
        public Comments Get_Comment_by_id(int commentid);

        public bool Delete_Comment(int commentid);

        public bool Change_comment_status(int commentid, int changecode);

        public bool delete_post_comment(int postid, int post_type);
        public List<Comments> Get_Paging_comment(int pageid);
        public List<Comments> get_music_comments_for_admin(int postid);
        public List<Comments> get_mv_comments_for_admin(int postid);
        public List<Comments> Get_remix_comment_for_admin(int postid);
        public List<Comments> Get_accepted_comments();
        public List<Comments> Get_not_accepted_comment();
        public List<Comments> Get_All_parent_comment();
        public List<Comments> Get_all_accpted_reply();
        public List<Comments> Get_all_not_accpeted_reply();
        public List<Comments> Get_all_Reply();



    }
}
