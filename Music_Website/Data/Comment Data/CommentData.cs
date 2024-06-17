using Microsoft.EntityFrameworkCore;
using Music_Website.Models;
using System.Collections.Immutable;

namespace Music_Website.Data.Comment_Data
{
    public class CommentData : ICommentData
    {
        private Context db;
        public CommentData(Context db)
        {
            this.db = db;
        }
        public bool add_comment(Comments comments)
        {
            try
            {
                db.comment.Add(comments);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public int Comment_Count()
        {
            return db.comment.Count();
        }
        public List<Comments> Get_all_comment_for_admin_page()
        {
            //List<Comments> Commentlist = new List<Comments>();
            return db.comment.Where(e => e.ParentId == null).Include(e => e.Child.OrderBy(e => e.Admin_Accepted == true)).Include(e => e.Remix).Include(e => e.music).Include(e => e.music_video).OrderBy(e => e.Admin_Accepted == true).ToList();
        }

        public List<Comments> Get_Music_comment(int musicid)
        {
            return db.comment.Where(e => e.music.SongId == musicid && e.ParentId == null && e.Admin_Accepted == true).Include(e => e.Child.Where(e => e.Admin_Accepted == true)).ToList();
        }
        public List<Comments> Get_Mv_Comment(int mvid)
        {
            return db.comment.Where(e => e.music_video.MVId == mvid && e.ParentId == null && e.Admin_Accepted == true).Include(e => e.Child).ToList();
        }

        public List<Comments> Get_Remix_Comment(int remixid)
        {
            return db.comment.Where(e => e.Remix.RemixId == remixid && e.ParentId == null && e.Admin_Accepted == true).Include(e => e.Child).ToList();
        }

        public Comments Get_Comment_by_id(int commentid)
        {
            return db.comment.Where(e => e.CommentId == commentid).FirstOrDefault();
        }

        public bool Delete_Comment(int commentid)
        {
            try
            {
                var comment_child = db.comment.Where(e => e.ParentId == commentid).ToList();
                if (comment_child.Count > 0)
                {
                    foreach (var deletecomment in comment_child)
                    {
                        db.comment.Remove(deletecomment);
                    }

                }
                var parent_comment = db.comment.Where(e => e.CommentId == commentid).FirstOrDefault();
                db.Remove(parent_comment);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool delete_post_comment(int postid, int post_type)
        {
            try
            {
                //1=music
                if (post_type == 1)
                {
                    var comments = db.comment.Where(e => e.music.SongId == postid).ToList();
                    if (comments.Any())
                    {
                        foreach (var comment in comments)
                        {
                            var childcomment = db.comment.Where(e => e.ParentId == comment.CommentId).ToList();
                            if (childcomment.Any())
                            {
                                foreach (var child in childcomment)
                                {
                                    db.Remove(child);
                                }
                            }
                        }
                        db.Remove(comments);
                    }

                }
                //2=remix
                if (post_type == 2)
                {
                    var comments = db.comment.Where(e => e.Remix.RemixId == postid).ToList();
                    if (comments.Any())
                    {
                        foreach (var comment in comments)
                        {
                            var childcomment = db.comment.Where(e => e.ParentId == comment.CommentId).ToList();
                            if (childcomment.Any())
                            {
                                foreach (var child in childcomment)
                                {
                                    db.Remove(child);
                                }
                            }
                        }
                        db.Remove(comments);
                    }
                }
                //3=mv
                if (post_type == 3)
                {
                    var comments = db.comment.Where(e => e.music_video.MVId == postid).ToList();
                    if (comments.Any())
                    {
                        foreach (var comment in comments)
                        {
                            var childcomment = db.comment.Where(e => e.ParentId == comment.CommentId).ToList();
                            if (childcomment.Any())
                            {
                                foreach (var child in childcomment)
                                {
                                    db.Remove(child);
                                }
                            }
                        }
                        db.Remove(comments);
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool Change_comment_status(int commentid, int changecode)
        {
            try
            {
                var comment = db.comment.Where(e => e.CommentId == commentid).FirstOrDefault();
                if (changecode == 0 && comment.Admin_Accepted == true)
                {
                    comment.Admin_Accepted = false;
                }
                if (changecode == 1 && comment.Admin_Accepted == false)
                {
                    comment.Admin_Accepted = true;
                }
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public List<Comments> Get_Paging_comment(int pageid)
        {
            int skip = (pageid - 1) * 10;
            return db.comment.Where(e => e.ParentId == null).OrderByDescending(e => e.Admin_Accepted == false).Skip(skip).Take(10)
                .Include(e => e.Child.OrderByDescending(e => e.Admin_Accepted == false))
                .Include(e => e.Remix)
                .Include(e => e.music)
                .Include(e => e.music_video).ToList();
        }
        public int child_comment_count()
        {
            return db.comment.Where(e => e.ParentId != null).Count();
        }
        public int accpeted_child_comment_count()
        {
            return db.comment.Where(e => e.ParentId != null && e.Admin_Accepted == true).Count();
        }
        public int not_accpeted_child_comment_count()
        {
            return db.comment.Where(e => e.ParentId != null && e.Admin_Accepted == false).Count();
        }
        public int parent_comment_count()
        {
            return db.comment.Where(e => e.ParentId == null).Count();
        }
        public int accepted_parent_comment_count()
        {
            return db.comment.Where(e => e.ParentId == null && e.Admin_Accepted == true).Count();
        }
        public int not_accepted_parent_comment_count()
        {
            return db.comment.Where(e => e.ParentId == null && e.Admin_Accepted == false).Count();
        }
        public List<Comments> get_music_comments_for_admin(int postid)
        {
            return db.comment.Where(e => e.music.SongId == postid && e.Parent == null).Include(e => e.music).Include(e => e.Child.OrderByDescending(e => e.Admin_Accepted == false)).OrderByDescending(e => e.Admin_Accepted == false).ToList();
        }
        public List<Comments> get_mv_comments_for_admin(int postid)
        {
            return db.comment.Where(e => e.music_video.MVId == postid && e.Parent == null).Include(e => e.music_video).Include(e => e.Child.OrderByDescending(e => e.Admin_Accepted == false)).OrderByDescending(e => e.Admin_Accepted == false).ToList();

        }
        public List<Comments> Get_remix_comment_for_admin(int postid)
        {
            return db.comment.Where(e => e.Remix.RemixId == postid && e.Parent == null).Include(e => e.Remix).Include(e => e.Child.OrderByDescending(e => e.Admin_Accepted == false)).OrderByDescending(e => e.Admin_Accepted == false).ToList();

        }
        public List<Comments> Get_accepted_comments()
        {
            return db.comment.Where(e => e.Admin_Accepted == true && e.Parent == null).Include(e => e.music).Include(e => e.music_video).Include(e => e.Remix).Include(e => e.Child).ToList();
        }
        public List<Comments> Get_not_accepted_comment()
        {
            return db.comment.Where(e => e.Admin_Accepted == false && e.Parent == null).Include(e => e.music).Include(e => e.music_video).Include(e => e.Remix).Include(e => e.Child).ToList();

        }
        public List<Comments> Get_All_parent_comment()
        {
            return db.comment.Where(e => e.Parent == null).Include(e => e.music).Include(e => e.music_video).Include(e => e.Remix).ToList();
        }
        public List<Comments> Get_all_accpted_reply()
        {
            return db.comment.Where(e => e.Admin_Accepted == true && e.Parent != null).Include(e => e.Parent).Include(e => e.music).Include(e => e.Remix).Include(e => e.music_video).ToList();
        }
        public List<Comments> Get_all_not_accpeted_reply()
        {
            return db.comment.Where(e => e.Admin_Accepted == false && e.Parent != null).Include(e => e.Parent).Include(e => e.music).Include(e => e.Remix).Include(e => e.music_video).ToList();

        }
        public List<Comments> Get_all_Reply()
        {
            return db.comment.Where(e=>e.Parent != null).Include(e => e.Parent).Include(e => e.music).Include(e => e.Remix).Include(e => e.music_video).ToList();

        }


    }
}
