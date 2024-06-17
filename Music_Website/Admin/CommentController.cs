using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Music_Website.Data.Comment_Data;
using Music_Website.Models;
using System.Security.Claims;
using System.Security.Cryptography.Xml;

namespace Music_Website.Admin
{
    [Authorize]
    public class CommentController : Controller
    {
        private ICommentData _commentData;
        public CommentController(ICommentData commentData)
        {
            _commentData = commentData;
        }
        public List<Comments> Refresh(int pageid = 1)
        {
            var a = _commentData.Get_Paging_comment(pageid);
            return a;
        }
        public IActionResult Comment_manage_Page(int pageid = 1)
        {
            ViewBag.currentpage = pageid;
            int commentcount = _commentData.parent_comment_count();
            if (commentcount % 10 == 0)
            {
                ViewBag.pagecount = commentcount / 10;
            }
            else
            {
                ViewBag.pagecount = commentcount / 10 + 1;
            }
            return View("Views/Admin Page/Comment_Manage.cshtml", Refresh(pageid));
        }
        public IActionResult Add_reply(int commentid, string replyto, string replaytext, int? postid, int? posttype)
        {
            if (commentid != 0 && replyto != null && replaytext != null)
            {
                var maincomment = _commentData.Get_Comment_by_id(commentid);
                if (maincomment != null)
                {
                    Comments comments = new Comments()
                    {
                        comment_Name = User.FindFirstValue(ClaimTypes.Name),
                        Email_Address = User.FindFirstValue(ClaimTypes.Email),
                        Comment_Text = $"@{replyto}" + " " + replaytext,
                        Parent = maincomment,
                        Admin_Accepted = true,
                    };
                    _commentData.add_comment(comments);
                    if (posttype == null || posttype == 0)
                    {
                        return RedirectToAction(nameof(Comment_manage_Page));
                    }
                    else
                    {
                        return RedirectToAction(nameof(Show_comment), new { postid = postid, post_type = posttype });
                    }
                }
                else
                {
                    return View();
                }
            }
            else
            {
                return View("Views/Admin Page/Comment_Manage.cshtml", Refresh());
            }
        }
        public IActionResult Delete_Comment(int commentid, int? postid, int? post_type)
        {
            bool delete_statis = _commentData.Delete_Comment(commentid);
            if (delete_statis == true)
            {
                if (post_type == null || post_type == 0)
                {
                    return RedirectToAction(nameof(Comment_manage_Page));
                }
                else
                {
                    return RedirectToAction(nameof(Show_comment), new { postid = postid, post_type = post_type });
                }
            }
            else
            {
                return View();
            }
        }
        public IActionResult changecommentstatus(int commentid, int commentstatuschange, int? postid, int? posttype)
        {
            if (commentid != 0 && commentstatuschange == 0 || commentstatuschange == 1)
            {
                bool changestatus = _commentData.Change_comment_status(commentid, commentstatuschange);
                if (changestatus == true)
                {
                    if (posttype == null || posttype == 0)
                    {
                        return View("Views/Admin Page/Comment_Manage.cshtml", Refresh());
                    }
                    else
                    {
                        return RedirectToAction(nameof(Show_comment), new { postid = postid, post_type = posttype });
                    }
                }
                else
                {
                    return View();
                }
            }
            else
            {
                return BadRequest();
            }
        }
        //post type==1 Music
        //post type==2 remix
        //post type==3 MV
        //post type==4 accepted comment
        //post type==5 not accepted comment
        //post type==6 All Parents Comment
        //post type==7 accpted reply
        //post type==8 Not accepted reply
        //post type==9 All reply
        public IActionResult Show_comment(int pageid = 1, int postid = 0, int post_type = 0)
        {
            ViewBag.posttype = post_type;
            ViewBag.postid = postid;
            ViewBag.currentpage = pageid;
            if (post_type == 1)
            {
                var music_comment = _commentData.get_music_comments_for_admin(postid);
                int count = music_comment.Count;
                if (count % 20 == 0)
                {
                    ViewBag.pagecount = music_comment.Count / 20;
                }
                else
                {
                    ViewBag.pagecount = music_comment.Count / 20 + 1;
                }
                return View("Views/Admin Page/Comment_Manage.cshtml", music_comment);
            }
            if (post_type == 2)
            {
                var remix_comment = _commentData.Get_remix_comment_for_admin(postid);
                if (remix_comment != null)
                {
                    int count = remix_comment.Count;
                    if (count % 20 == 0)
                    {
                        ViewBag.pagecount = remix_comment.Count / 20;
                    }
                    else
                    {
                        ViewBag.pagecount = remix_comment.Count / 20 + 1;
                    }
                    return View("Views/Admin Page/Comment_Manage.cshtml", remix_comment);
                }
                else
                {
                    return View("Views/Admin Page/Comment_Manage.cshtml");
                }
            }
            if (post_type == 3)
            {
                var mv = _commentData.get_mv_comments_for_admin(postid);
                if (mv != null)
                {
                    int count = mv.Count;
                    if (count % 20 == 0)
                    {
                        ViewBag.pagecount = mv.Count / 20;
                    }
                    else
                    {
                        ViewBag.pagecount = mv.Count / 20 + 1;
                    }

                    return View("Views/Admin Page/Comment_Manage.cshtml", mv);
                }
                else
                {
                    return View("Views/Admin Page/Comment_Manage.cshtml");
                }
            }
            if (post_type == 4)
            {
                var accepted_comment = _commentData.Get_accepted_comments();
                if (accepted_comment != null)
                {
                    int count = accepted_comment.Count;
                    if (count % 20 == 0)
                    {
                        ViewBag.pagecount = accepted_comment.Count / 20;
                    }
                    else
                    {
                        ViewBag.pagecount = accepted_comment.Count / 20 + 1;
                    }

                    return View("Views/Admin Page/Comment_Manage.cshtml", accepted_comment);
                }
                else
                {
                    return View("Views/Admin Page/Comment_Manage.cshtml");
                }
            }
            if (post_type == 5)
            {
                var not_accepted_comment = _commentData.Get_not_accepted_comment();
                if (not_accepted_comment != null)
                {
                    int count = not_accepted_comment.Count;
                    if (count % 20 == 0)
                    {
                        ViewBag.pagecount = not_accepted_comment.Count / 20;
                    }
                    else
                    {
                        ViewBag.pagecount = not_accepted_comment.Count / 20 + 1;
                    }

                    return View("Views/Admin Page/Comment_Manage.cshtml", not_accepted_comment);
                }
                else
                {
                    return View("Views/Admin Page/Comment_Manage.cshtml");
                }
            }
            if (post_type == 6)
            {
                var parent_cooment = _commentData.Get_All_parent_comment();
                if (parent_cooment != null)
                {
                    int count = parent_cooment.Count;
                    if (count % 20 == 0)
                    {
                        ViewBag.pagecount = parent_cooment.Count / 20;
                    }
                    else
                    {
                        ViewBag.pagecount = parent_cooment.Count / 20 + 1;
                    }

                    return View("Views/Admin Page/Comment_Manage.cshtml", parent_cooment);
                }
                else
                {
                    return View("Views/Admin Page/Comment_Manage.cshtml");
                }

            }
            if (post_type == 7)
            {
                var accepted_reply = _commentData.Get_all_accpted_reply();
                if (accepted_reply != null)
                {
                    int count = accepted_reply.Count;
                    if (count % 20 == 0)
                    {
                        ViewBag.pagecount = accepted_reply.Count / 20;
                    }
                    else
                    {
                        ViewBag.pagecount = accepted_reply.Count / 20 + 1;
                    }

                    return View("Views/Admin Page/Comment_Manage.cshtml", accepted_reply);
                }
                else
                {
                    return View("Views/Admin Page/Comment_Manage.cshtml");
                }
            }
            if (post_type == 8)
            {
                var Not_accepted_reply = _commentData.Get_all_not_accpeted_reply();
                if (Not_accepted_reply != null)
                {
                    int count = Not_accepted_reply.Count;
                    if (count % 20 == 0)
                    {
                        ViewBag.pagecount = Not_accepted_reply.Count / 20;
                    }
                    else
                    {
                        ViewBag.pagecount = Not_accepted_reply.Count / 20 + 1;
                    }

                    return View("Views/Admin Page/Comment_Manage.cshtml", Not_accepted_reply);
                }
                else
                {
                    return View("Views/Admin Page/Comment_Manage.cshtml");
                }
            }
            if (post_type == 9)
            {
                var all_reply = _commentData.Get_all_Reply();
                if (all_reply != null)
                {
                    int count = all_reply.Count;
                    if (count % 20 == 0)
                    {
                        ViewBag.pagecount = all_reply.Count / 20;
                    }
                    else
                    {
                        ViewBag.pagecount = all_reply.Count / 20 + 1;
                    }

                    return View("Views/Admin Page/Comment_Manage.cshtml", all_reply);
                }
                else
                {
                    return View("Views/Admin Page/Comment_Manage.cshtml");
                }
            }
            else
            {
                return RedirectToAction(nameof(Comment_manage_Page));
            }

        }
    }
}
