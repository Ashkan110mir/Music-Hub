using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Differencing;
using Microsoft.Identity.Client;
using Music_Website.Data.Albums_Data;
using Music_Website.Data.Comment_Data;
using Music_Website.Data.Music_Data;
using Music_Website.Data.Music_Video_Data;
using Music_Website.Data.Remix_Data;
using Music_Website.Data.Singer;
using Music_Website.Models;

namespace Music_Website.Controllers
{
    public class MusicController : Controller
    {
        private IMusicData _musicdata;
        private IRemixData _remixData;
        private IMusicVideo_Data _musicVideoData;
        private IAlbumsData _albumsData;
        private ISingerData _singerData;
        private ICommentData _commentdata;
        public MusicController(IMusicData musicData, IRemixData remixdata, IMusicVideo_Data Musicvideodata, IAlbumsData albumsData, ISingerData singerData, ICommentData commentData)
        {
            _musicdata = musicData;
            _remixData = remixdata;
            _musicVideoData = Musicvideodata;
            _albumsData = albumsData;
            _singerData = singerData;
            _commentdata = commentData;
        }
        #region Album Controller
        public IActionResult Album_Page(string? Searchname,string?searchmusic,string?searchsinger,string?orderby, int pageid=1)
        {
            ViewBag.currentpage = pageid;
            if (Searchname == null && searchmusic==null&&searchsinger==null&& orderby==null)
            {
                int pagecount= _albumsData.albums_count();
                if(pagecount%5==0)
                {
                    ViewBag.pagecount = pagecount / 5;
                }
                else
                {
                    ViewBag.pagecount = pagecount / 5+1;
                }
                var albums = _albumsData.Get_paging_album(pageid);
                return View("Views/Music/Album.cshtml", albums);
                
            }
            else
            {
                int search_count = _albumsData.Search_album_user_count(Searchname,searchmusic,searchsinger);
                if(search_count%5==0)
                {
                    ViewBag.pagecount = search_count / 5;
                }
                else
                {
                    ViewBag.pagecount = search_count / 5 + 1;
                }
                ViewBag.searchname=Searchname;
                ViewBag.searchmusic = searchmusic;
                ViewBag.searchsinger = searchsinger;
                ViewBag.orderby=orderby;
                var seach_result = _albumsData.Search_album_user(Searchname, searchmusic, searchsinger, orderby, pageid);
                return View("Views/Music/Album.cshtml", seach_result);
            }
        }

        public IActionResult Album_Deteil(int albumid)
        {
            var album = _albumsData.Get_Album_Deteil(albumid);
            return View("Views/Music/Album_Detail.cshtml", album);
        }
        #endregion
        #region Singer Controller
        public IActionResult Singer_Page(string? Searchname,string? searchmusic, string? searchmv,string? searchalbum,string? orderby,int pageid=1)
        {
            ViewBag.currentpage = pageid;
            if (Searchname == null && searchmusic == null && searchmv==null&& searchalbum==null&& orderby==null)
            {
                int pagecount = _singerData.singer_count();
                if (pagecount % 5 == 0)
                {
                    ViewBag.pagecount = pagecount / 5;
                }
                else
                {
                    ViewBag.pagecount = pagecount / 5+1;
                }
                return View("Views/Music/Singer.cshtml", _singerData.Get_Paging_Singer(pageid,null));
            }
            else
            {
                int pagecount = _singerData.Search_singer_user_count(Searchname,searchmusic,searchmv,searchalbum);
                if(pagecount% 5 == 0)
                {
                    ViewBag.pagecount = pagecount / 5;
                }
                else
                {
                    ViewBag.pagecount = pagecount / 5 + 1;
                }
                ViewBag.searchname = Searchname;
                ViewBag.searchmusic = searchmusic;
                ViewBag.searchmv = searchmv;
                ViewBag.searchalbum = searchalbum;
                ViewBag.orderby = orderby;
                var search_result=_singerData.Search_singer_user(Searchname,searchmusic,searchmv,searchalbum,orderby,pageid);
                return View("Views/Music/Singer.cshtml", search_result);
            }
        }

        public IActionResult Singer_Info(int singerid)
        {
            var singer = _singerData.Get_Singer_Full_Info(singerid);
            return View("Views/Music/Singer_Info.cshtml", singer);
        }
        #endregion
        #region Music Cotroller

        public IActionResult Download_Music(string song_name, string fileurl)
        {

                string new_file_name = song_name + ".mp3";
                var file_path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Music", fileurl);
                var filebyte = System.IO.File.ReadAllBytes(file_path);
                return File(filebyte, "audio/mpeg", new_file_name);
        }
        public IActionResult music_page(string? searchname, string? searchsinger, string? searchstyle, string? orderby, int pageid = 1)
        {
            //not search
            int musiccount = _musicdata.Music_Count();
            if (searchname == null && searchsinger == null && searchstyle == null && orderby == null)
            {
                var musics = _musicdata.Get_paging_music(pageid);
                ViewBag.curentpage = pageid;

                if (musiccount % 5 == 0)
                {
                    ViewBag.pagecount = musiccount / 5;
                }
                else
                {
                    ViewBag.pagecount = musiccount / 5 + 1;
                }
                return View("Views/Music/Music.cshtml", musics);
            }
            //search
            else
            {
                List<Music> Musicsearch = new List<Music>();
                Musicsearch = _musicdata.Search_Music(searchname, searchsinger, searchstyle, orderby, pageid);
                int search_count = _musicdata.Search_Music_count(searchname, searchsinger, searchstyle, orderby);
                ViewBag.curentpage = pageid;
                if(search_count%5 == 0)
                {
                    ViewBag.pagecount = search_count / 5;
                }
                else
                {
                    ViewBag.pagecount = search_count / 5+1;
                }
                ViewBag.searchname = searchname;
                ViewBag.searchsinger=searchsinger;
                ViewBag.searchstyle=searchstyle;
                ViewBag.orderby=orderby;
                return View("Views/Music/Music.cshtml", Musicsearch);
            }
        }
        private Models.Mian_Page_View_Model.Singer_Comment_Viewmodel Full_song_refresh(int songid)
        {
            var music = _musicdata.Get_Music_By_Id(songid);
            if (music != null)
            {
                Models.Mian_Page_View_Model.Singer_Comment_Viewmodel singer_Comment = new Models.Mian_Page_View_Model.Singer_Comment_Viewmodel();
                singer_Comment.music = _musicdata.Get_Full_Music_info_by_id(songid);
                singer_Comment.comments = _commentdata.Get_Music_comment(songid);
                return singer_Comment;
            }
            return null;
        }
        public IActionResult Full_Song(int songid)
        {
            return View("Views/Music/Full_Song.cshtml", Full_song_refresh(songid));
        }

        #endregion
        #region music Video controller
        public IActionResult Download_Mv(string mv_name, string fileurl)
        {
            string extension = Path.GetExtension(fileurl).ToUpper();
            string new_file_name = mv_name + extension;
            var file_path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Music Video", fileurl);
            var filebyte = System.IO.File.ReadAllBytes(file_path);
            return File(filebyte, "multipart/form-data", new_file_name);
        }
        public IActionResult MV_page(string? searchname,string? searchsinger,string? orderby,int pageid=1)
        {
            ViewBag.currentpage = pageid;
            if (searchname == null && searchsinger==null&& orderby==null)
            {
               int pagecount=_musicVideoData.music_video_count();
                if(pagecount%5==0)
                {
                    ViewBag.pagecount = pagecount / 5;
                }
                else
                {
                    ViewBag.pagecount = (pagecount / 5)+1;
                }
                
                var mv = _musicVideoData.Get_Mv_Paging(pageid);
                mv = mv.OrderByDescending(e => e.MVId).ToList();
                return View("Views/Music/MUsic_Video.cshtml", mv);
            }
            else
            {
                int pagecount = _musicVideoData.Search_mv_for_user_count(searchname, searchsinger);
                if(pagecount%5==0)
                {
                    ViewBag.pagecount = pagecount / 5;
                }
                else
                {
                    ViewBag.pagecount = (pagecount / 5) + 1;
                }
                ViewBag.searchname = searchname;
                ViewBag.searchsinger = searchsinger;
                ViewBag.orderby = orderby;
                return View("Views/Music/MUsic_Video.cshtml", _musicVideoData.Search_mv_for_user(searchname,searchsinger,orderby,pageid));
            }

        }
        private Models.Mian_Page_View_Model.MV_Comment_ViewModel Full_Mv_refresh(int mvid)
        {
            Models.Mian_Page_View_Model.MV_Comment_ViewModel mv_comment = new Models.Mian_Page_View_Model.MV_Comment_ViewModel();
            mv_comment.MV = _musicVideoData.Get_Full_mv_info(mvid);
            mv_comment.comments = _commentdata.Get_Mv_Comment(mvid);
            return mv_comment;
        }
        public IActionResult Full_Mv(int mvid)
        {
            return View("Views/Music/Full_MV.cshtml", Full_Mv_refresh(mvid));
        }


        #endregion
        #region Remix Controller
        public IActionResult Download_Remix(string remix_name, string fileurl)
        {
            string extension = Path.GetExtension(fileurl).ToUpper();
            string new_file_name = remix_name + extension;
            var file_path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Music","Remix", fileurl);
            var filebyte = System.IO.File.ReadAllBytes(file_path);
            return File(filebyte, "audio/mpeg", new_file_name);
        }
        public IActionResult Remix(string? searchname,string? searchcreator,string? searchsong,string? orderby,int pageid=1)
        {
            //its not search
            ViewBag.curentpage = pageid;
            if (searchname == null && searchcreator==null&& searchsong==null&& orderby==null)
            {
                int remix_count = _remixData.RemixCount();
                if (remix_count % 5 == 0)
                {
                    ViewBag.pagecount = remix_count / 5;
                }
                else
                {
                    ViewBag.pagecount=(remix_count / 5)+1;
                }
                
                return View("Views/Music/Remix.cshtml", _remixData.Get_Paging_remix(pageid));
            }
            //its search
            else
            {
                List<Remix> remixes = new List<Remix>();
                remixes=_remixData.Search_remix_user(searchname,searchcreator,searchsong,orderby,pageid);
                int pagecount = _remixData.search_remix_user_count(searchname, searchcreator, searchsong);
                if(pagecount%5==0)
                {
                    ViewBag.pagecount=pagecount/5;
                }
                else
                {
                    ViewBag.pagecount = pagecount / 5 + 1;
                }
                ViewBag.searchname = searchname;
                ViewBag.searchcreator = searchcreator;
                ViewBag.searchsong = searchsong;
                ViewBag.orderby =orderby;

                return View("Views/Music/Remix.cshtml", remixes);
            }
        }
        private Models.Mian_Page_View_Model.Remix_comment_Viewmodel Full_Remix_refresh(int remixid)
        {
            Models.Mian_Page_View_Model.Remix_comment_Viewmodel remix_Comment = new Models.Mian_Page_View_Model.Remix_comment_Viewmodel();
            remix_Comment.remix = _remixData.Get_Full_Remix_Info(remixid);
            remix_Comment.comments = _commentdata.Get_Remix_Comment(remixid);
            return remix_Comment;
        }
        public IActionResult Full_remix(int remixid)
        {
            return View("Views/Music/Full_Remix.cshtml", Full_Remix_refresh(remixid));
        }

        #endregion
        #region Comment controller
        [HttpPost]
        public IActionResult Add_comment(Comments addcomments, int commentfor, int id)
        {
            if (addcomments.comment_Name != null && addcomments.comment_Name.Length<=200 && addcomments.Comment_Text != null && addcomments.Comment_Text.Length<=900 && addcomments.Email_Address != null && commentfor != 0 && id != 0)
            {
                Comments comments = new Comments();
                comments = addcomments;
                //add comment for music
                if (commentfor == 1)
                {
                    Music music = new Music();
                    music = _musicdata.Get_Music_By_Id(id);
                    if (music != null)
                    {
                        comments.music = music;
                        if (music.comment_status == 1)
                        {
                            if (User.Identity.IsAuthenticated)
                            {
                                comments.Admin_Accepted = true;
                            }
                            else
                            {
                                comments.Admin_Accepted = false;
                            }
                        }
                        if (music.comment_status == 2)
                        {
                            comments.Admin_Accepted = true;
                        }

                        bool add_comment_status = _commentdata.add_comment(comments);
                        if (add_comment_status == true)
                        {
                            return RedirectToAction(nameof(Full_Song), new { songid = id });
                        }
                        else
                        {
                            ViewBag.mess = "خطایی در افزودن نظر رخ داد";
                            return View("Views/Music/Full_Song.cshtml", Full_song_refresh(id));
                        }
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
                if (commentfor == 2)
                {
                    var remix = _remixData.get_remix_by_id(id);
                    if (remix != null)
                    {
                        if (remix.comment_status == 1)
                        {
                            if (User.Identity.IsAuthenticated)
                            {
                                comments.Admin_Accepted = true;
                            }
                            else
                            {
                                comments.Admin_Accepted = false;
                            }
                        }
                        else
                        {
                            comments.Admin_Accepted = true;
                        }
                        comments.Remix = remix;
                        bool add_comment_status = _commentdata.add_comment(comments);
                        if (add_comment_status == true)
                        {
                            return RedirectToAction(nameof(Full_remix), new { remixid = id });
                        }
                        else
                        {
                            ViewBag.mess = "خطایی در افزودن رخ داد";
                            return View("Views/Music/Full_Remix.cshtml", Full_Remix_refresh(id));
                        }
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
                if (commentfor == 3)
                {
                    var mv = _musicVideoData.get_musicvideo_by_id(id);
                    if (mv != null)
                    {
                        if (mv.comment_status == 1)
                        {
                            if (User.Identity.IsAuthenticated)
                            {
                                comments.Admin_Accepted = true;
                            }
                            else
                            {
                                comments.Admin_Accepted = false;
                            }
                        }
                        if (mv.comment_status == 2)
                        {
                            comments.Admin_Accepted = true;
                        }
                        comments.music_video = mv;
                        bool add_comment_status = _commentdata.add_comment(comments);
                        if (add_comment_status == true)
                        {
                            return RedirectToAction(nameof(Full_Mv), new { mvid = mv.MVId });
                        }
                        else
                        {
                            ViewBag.mess = "خطایی در افزودن رخ داد";
                            return View("Views/Music/Full_MV.cshtml", Full_Mv_refresh(id));
                        }
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
                else
                {
                    return View();
                }
            }
            else
            {
                if (commentfor == 1)
                {
                    ViewBag.mess = "فیلد ها را پر کنید";

                    return View("Views/Music/Full_Song.cshtml", Full_song_refresh(id));
                }
                if (commentfor == 2)
                {
                    ViewBag.mess = "لطفا فیلد ها را پر کنید";
                    return View("Views/Music/Full_Remix.cshtml", Full_Remix_refresh(id));
                }
                if (commentfor == 3)
                {
                    ViewBag.mess = "لطفا فیلد ها را پر کنید";
                    return View("Views/Music/Full_MV.cshtml", Full_Mv_refresh(id));
                }
                else
                {
                    return BadRequest();
                }
            }

        }
        [HttpPost]
        public IActionResult Add_Reply(Comments Add_reply, int replyfor, int postid, int parentid, string replyto)
        {
            if (Add_reply.comment_Name != null && Add_reply.comment_Name.Length<=200 && Add_reply.Comment_Text != null && Add_reply.Comment_Text.Length <= 900 && Add_reply.Email_Address != null)
            {
                var parent = _commentdata.Get_Comment_by_id(parentid);
                Comments newreply = new Comments()
                {
                    comment_Name = Add_reply.comment_Name,
                    Email_Address = Add_reply.Email_Address,
                    Comment_Text = $"@{replyto}" + " " + Add_reply.Comment_Text,
                    Parent = parent,
                };
                if (replyfor == 1)
                {
                    var music = _musicdata.Get_Music_By_Id(postid);
                    if (music.comment_status == 1)
                    {
                        if (User.Identity.IsAuthenticated)
                        {
                            newreply.Admin_Accepted = true;
                        }
                        else
                        {
                            newreply.Admin_Accepted = false;
                        }
                    }
                    if (music.comment_status == 2)
                    {
                        newreply.Admin_Accepted = true;
                    }
                }
                if (replyfor == 2)
                {
                    var remix = _remixData.get_remix_by_id(postid);
                    if (remix.comment_status == 1)
                    {
                        if (User.Identity.IsAuthenticated)
                        {
                            newreply.Admin_Accepted = true;
                        }
                        else
                        {
                            newreply.Admin_Accepted = false;
                        }
                    }
                    if (remix.comment_status == 2)
                    {
                        {
                            newreply.Admin_Accepted = true;
                        }
                    }
                }
                if (replyfor == 3)
                {
                    var mv = _musicVideoData.get_musicvideo_by_id(postid);
                    if (mv.comment_status == 1)
                    {
                        if (User.Identity.IsAuthenticated)
                        {
                            newreply.Admin_Accepted = true;
                        }
                        else
                        {
                            newreply.Admin_Accepted = false;
                        }
                    }
                    if (mv.comment_status == 2)
                    {
                        newreply.Admin_Accepted = true;
                    }
                }
                bool add_reply_status = _commentdata.add_comment(newreply);
                if (add_reply_status == true)
                {
                    if (replyfor == 1)
                    {
                        return RedirectToAction(nameof(Full_Song), new { songid = postid });
                    }
                    if (replyfor == 2)
                    {
                        return RedirectToAction(nameof(Full_remix), new { remixid = postid });
                    }
                    else
                    {
                        return RedirectToAction(nameof(Full_Mv), new { mvid = postid });
                    }

                }
                else
                {
                    if (replyfor == 1)
                    {
                        ViewBag.mess = "خطایی در افزودن رخ داد";
                        return View("Views/Music/Full_Song.cshtml", Full_song_refresh(postid));
                    }
                    if (replyfor == 2)
                    {
                        ViewBag.mess = "خطایی در افزودن رخ داد";
                        return View("Views/Music/Full_Remix.cshtml", Full_Remix_refresh(postid));
                    }
                    if (replyfor == 3)
                    {
                        ViewBag.mess = "خطایی در افزودن رخ داد";
                        return View("Views/Music/Full_MV.cshtml", Full_Mv_refresh(postid));
                    }
                    else
                    {
                        return BadRequest();
                    }
                }

            }
            else
            {
                if (replyfor == 1)
                {
                    ViewBag.mess = "فیلد ها را پر کنید";
                    return View("Views/Music/Full_Song.cshtml", Full_song_refresh(postid));
                }
                if (replyfor == 2)
                {
                    ViewBag.mess = "لطفا فیلد ها را پر کنید";
                    return View("Views/Music/Full_Remix.cshtml", Full_Remix_refresh(postid));
                }
                if (replyfor == 3)
                {
                    ViewBag.mess = "لطفا فیلد ها را پر کنید";
                    return View("Views/Music/Full_MV.cshtml", Full_Mv_refresh(postid));
                }
                else
                {
                    return BadRequest();
                }
            }
        }
        #endregion
    }
}
