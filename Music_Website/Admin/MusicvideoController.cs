using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Music_Website.Data.Admin_Data;
using Music_Website.Data.Comment_Data;
using Music_Website.Data.Music_Video_Data;
using Music_Website.Data.Singer;
using Music_Website.Models;
using Newtonsoft.Json.Converters;
using System.Security.Claims;

namespace Music_Website.Admin
{
    [Authorize]
    public class MusicvideoController : Controller
    {
        private IMusicVideo_Data _musicvideodata;
        private ISingerData _singerData;
        private IAdminData _adminData;
        private ICommentData _commentData;
        public MusicvideoController(IMusicVideo_Data musicvideodata, ISingerData singerData, IAdminData adminData, ICommentData commentData)
        {
            _musicvideodata = musicvideodata;
            _singerData = singerData;
            _adminData = adminData;
            _commentData = commentData;
        }

        public Admin_Music_Video_Viewmodel refresh(string? searchname, int pageid)
        {
            Admin_Music_Video_Viewmodel admin_Music_s = new Admin_Music_Video_Viewmodel();
            var singer = _singerData.GetSingers_Name_ViewModels();
            singer = singer.OrderBy(e => e.ArtistName).ToList();
            admin_Music_s.singerlist = singer;
            if (searchname == null)
            {
                admin_Music_s.music_video = _musicvideodata.Get_Mv_paging_admin(pageid);
            }
            else
            {
                admin_Music_s.music_video = _musicvideodata.search_mv(searchname);
            }
            return admin_Music_s;
        }
        public IActionResult Get_Music_Video_Page(string? searchname, int pageid = 1)
        {
            if (searchname == null)
            {
                int total_mv_count = _musicvideodata.music_video_count();
                if (total_mv_count % 20 == 0)
                {
                    ViewBag.pagecount = total_mv_count / 20;
                }
                else
                {
                    ViewBag.pagecount = total_mv_count / 20 + 1;
                }
                ViewBag.currentpage = pageid;
                return View("Views/Admin Page/Music_Video_manage.cshtml", refresh(null, pageid));
            }
            else
            {
                return View("Views/Admin Page/Music_Video_manage.cshtml", refresh(searchname, pageid));

            }
        }
        [HttpPost]
        [RequestSizeLimit(64 * 1024 * 1024)]
        public IActionResult Add_mvmusic(Music_Video music_Video, List<int> singerid)
        {
            //file size is ok
            if (music_Video != null)
            {
                // all input are ok
                if (music_Video.Mvname != null && music_Video.Mv_Describe != null && music_Video.Mv_publishdate != null && music_Video.MVfile != null && singerid.Count > 0 && music_Video.MvposterFile != null)
                {
                    string mvextention = Path.GetExtension(music_Video.MVfile.FileName).ToString().ToUpper();
                    string posterextensin = Path.GetExtension(music_Video.MvposterFile.FileName).ToString().ToUpper();
                    if (mvextention == ".MP4" || mvextention == ".MVK" && posterextensin == ".PNG" || posterextensin == ".JPG" || posterextensin == ".SVG")
                    {
                        List<Singer> singerinfo = new List<Singer>();
                        foreach (var singer in singerid)
                        {
                            singerinfo.Add(_singerData.Get_singer_by_id(singer));
                        }
                        var admin = _adminData.get_admin(int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)));
                        Music_Video new_music_video = new Music_Video()
                        {
                            Mvname = music_Video.Mvname,
                            Mv_Describe = music_Video.Mv_Describe,
                            comment_status = music_Video.comment_status,
                            admin = admin,
                            Mv_publishdate = DateTime.Now,
                            singers = singerinfo,
                            File_Name = Guid.NewGuid() + mvextention,
                            MvposterName = Guid.NewGuid() + posterextensin,
                            in_main_index = false,
                        };
                        bool add_was_succes = _musicvideodata.Add_music_video(new_music_video);
                        // add was succes
                        if (add_was_succes)
                        {
                            string mvpath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Music Video", new_music_video.File_Name);
                            using (FileStream stream = new FileStream(mvpath, FileMode.Create))
                            {
                                music_Video.MVfile.CopyTo(stream);
                            }
                            string posterpath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Music Video/video-poster", new_music_video.MvposterName);
                            using (FileStream stream = new FileStream(posterpath, FileMode.Create))
                            {
                                music_Video.MvposterFile.CopyTo(stream);
                            }
                            return RedirectToAction(nameof(Get_Music_Video_Page));
                        }
                        else
                        {
                            ViewBag.error = "خطایی در افزودن رخ داد";
                            return View("Views/Admin Page/Music_Video_manage.cshtml", refresh(null, 0));
                        }
                    }
                    else
                    {
                        ViewBag.error = "فرمت فایل ها درست نیست";
                        return View("Views/Admin Page/Music_Video_manage.cshtml", refresh(null, 0));
                    }

                }
                //field is empty
                else
                {
                    ViewBag.error = "همه فیلد هارا پر کنید";
                    return View("Views/Admin Page/Music_Video_manage.cshtml", refresh(null, 0));
                }
            }
            //file size not ok
            else
            {

                ViewBag.error = "حجم فایل انتخابی زیاد است";
                return View("Views/Admin Page/Music_Video_manage.cshtml", refresh(null, 0));

            }
        }
        public IActionResult Remove_mvmusic(int mvid, string mvfilename)
        {
            if (mvid != 0)
            {
                bool delete_comment = _commentData.delete_post_comment(mvid, 3);
                bool deletewassucces = _musicvideodata.Remove_music_video(mvid);
                if (deletewassucces == true)
                {
                    string mvpath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Music Video", mvfilename);
                    System.IO.File.Delete(mvpath);
                    return RedirectToAction(nameof(Get_Music_Video_Page));
                }
                else
                {
                    ViewBag.berror = "خطایی در حذف رخ داد";
                    return View("Views/Admin Page/Music_Video_manage.cshtml", refresh(null, 0));
                }

            }
            else
            {
                return BadRequest();
            }

        }

        public IActionResult Edit_Mvmusic_page(int mvid)
        {
            if (mvid != 0)
            {
                var mv = _musicvideodata.get_musicvideo_by_id(mvid);
                if (mv == null)
                {
                    return BadRequest();
                }
                return View("Views/Admin Page/Music_Video_Edit.cshtml", mv);
            }
            else
            {
                return BadRequest();
            }
        }
        [HttpPost]
        [RequestSizeLimit(64 * 1024 * 1024)]
        public IActionResult Edit_mv(Music_Video editmusic_Video, int mvid)
        {
            if (editmusic_Video != null)
            {
                var mainmv = _musicvideodata.get_musicvideo_by_id(editmusic_Video.MVId);
                string mainfilename = mainmv.File_Name;
                string mainpostername = mainmv.MvposterName;
                if (editmusic_Video.Mvname != null && editmusic_Video.Mv_Describe != null)
                {
                    Music_Video newmv = new Music_Video()
                    {
                        MVId = mvid,
                        Mvname = editmusic_Video.Mvname,
                        Mv_Describe = editmusic_Video.Mv_Describe,
                        Mv_publishdate = mainmv.Mv_publishdate,
                        comment_status = editmusic_Video.comment_status,
                    };
                    if (editmusic_Video.MVfile != null)
                    {
                        string mvextension = Path.GetExtension(editmusic_Video.MVfile.FileName).ToString().ToUpper();
                        if (mvextension == ".MP4" || mvextension == ".MKV")
                        {
                            newmv.File_Name = Guid.NewGuid().ToString() + mvextension;
                        }
                        else
                        {
                            ViewBag.error = "فرمت موزیک ویدیو درست نیست";
                            return View("Views/Admin Page/Music_Video_Edit.cshtml", _musicvideodata.get_musicvideo_by_id(editmusic_Video.MVId));
                        }
                    }
                    if (editmusic_Video.MvposterFile != null)
                    {
                        string posterextention = Path.GetExtension(editmusic_Video.MvposterFile.FileName).ToString().ToUpper();
                        if (posterextention == ".JPG" || posterextention == ".PNG" || posterextention == ".SVG")
                        {
                            newmv.MvposterName = Guid.NewGuid().ToString() + posterextention;
                        }
                        else
                        {
                            ViewBag.error = "پوستر درست نیست";
                            return View("Views/Admin Page/Music_Video_Edit.cshtml", _musicvideodata.get_musicvideo_by_id(editmusic_Video.MVId));
                        }
                    }
                    bool editwasssucces = _musicvideodata.edit_mv(newmv);
                    if (editwasssucces)
                    {
                        if (editmusic_Video.MVfile != null)
                        {
                            string deletefilepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Music Video", mainfilename);
                            System.IO.File.Delete(deletefilepath);

                            string newfilepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Music Video", newmv.File_Name);
                            using (FileStream stream = new FileStream(newfilepath, FileMode.Create))
                            {
                                editmusic_Video.MVfile.CopyTo(stream);
                            }
                        }
                        if (editmusic_Video.MvposterFile != null)
                        {
                            string deleteposterpath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Music Video", "video-poster", mainpostername);
                            System.IO.File.Delete(deleteposterpath);

                            string newposterpath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Music Video", "video-poster", newmv.MvposterName);
                            using (FileStream stream = new FileStream(newposterpath, FileMode.Create))
                            {
                                editmusic_Video.MvposterFile.CopyTo(stream);
                            }
                        }
                        return RedirectToAction(nameof(Get_Music_Video_Page));
                    }
                    else
                    {
                        var mv = _musicvideodata.get_musicvideo_by_id(editmusic_Video.MVId);
                        ViewBag.error = "خطایی در ویرایش رخ داد";
                        return View("Views/Admin Page/Music_Video_Edit.cshtml", mv);
                    }

                }
                else
                {
                    var mv = _musicvideodata.get_musicvideo_by_id(editmusic_Video.MVId);
                    ViewBag.error = "فیلد ها را پر گنید";
                    return View("Views/Admin Page/Music_Video_Edit.cshtml", mv);
                }
            }
            else
            {
                var mv = _musicvideodata.get_musicvideo_by_id(editmusic_Video.MVId);
                ViewBag.error = "حجم فایل ارسالی درست نیست";
                return View("Views/Admin Page/Music_Video_Edit.cshtml", mv);
            }
        }
    }
}
