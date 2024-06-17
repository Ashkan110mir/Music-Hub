using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGeneration.EntityFrameworkCore;
using Music_Website.Data.Admin_Data;
using Music_Website.Data.Music_Data;
using Music_Website.Data.Remix_Data;
using Music_Website.Models;
using System.Security.Claims;
using System.Security.Cryptography.Xml;
using System.IO;
using Music_Website.Data.Comment_Data;

namespace Music_Website.Admin
{
    public class RemixController : Controller
    {
        private IRemixData _remixData;
        private IMusicData _musicData;
        private IAdminData _adminData;
        private ICommentData _commentData;
        public RemixController(IRemixData remixData, IMusicData musicData, IAdminData adminData, ICommentData commentData)
        {
            _remixData = remixData;
            _musicData = musicData;
            _adminData = adminData;
            _commentData = commentData;
        }
        public Admin_Remix_Viewmodel refresh(string? searchname, int? pageid=1)
        {

            Admin_Remix_Viewmodel remix_Viewmodel = new Admin_Remix_Viewmodel();
            var music= _musicData.Get_Music_name();
            music=music.OrderBy(e=>e.Name).ToList();
            remix_Viewmodel.music_name = music;
            if (searchname == null && pageid!=null)
            {
                int page =int.Parse(pageid.ToString());
                remix_Viewmodel.remixes = _remixData.Get_Paging_Remix_admin(page);
            }
            else
            {
                remix_Viewmodel.remixes = _remixData.search_remix(searchname);
            }
            return remix_Viewmodel;
        }
        public IActionResult Get_remix_Page(string? searchname, int pageid = 1)
        {
            ViewBag.currentpage = pageid;
            int total_Remix = _remixData.RemixCount();
            if (total_Remix % 20 == 0)
            {
                ViewBag.pagecount=total_Remix / 20;
            }
            else
            {
                ViewBag.pagecount = total_Remix / 20+1;
            }
            return View("Views/Admin Page/Remix_Manage.cshtml", refresh(searchname, pageid));
        }
        [HttpPost]
        public IActionResult Add_remix(Remix addremix, int musicid)
        {
            if (addremix.RemixName != null && addremix.Remix_Creator != null && addremix.Remixfile != null)
            {
                string fileformat = Path.GetExtension(addremix.Remixfile.FileName).ToUpper();
                if (fileformat == ".MP3")
                {
                    int adminid = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                    var admin = _adminData.get_admin(adminid);
                    Remix newremix = new Remix()
                    {
                        RemixName = addremix.RemixName,
                        Remix_Creator = addremix.Remix_Creator,
                        comment_status = addremix.comment_status,
                        admin = admin,
                        File_Name = Guid.NewGuid().ToString() + fileformat
                    };
                    if (musicid != 0)
                    {
                        newremix.music = _musicData.Get_Music_By_Id(musicid);
                    }
                    bool addwassuccess = _remixData.add_remix(newremix);
                    if (addwassuccess)
                    {
                        string remixpath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Music", "Remix", newremix.File_Name);
                        using (FileStream stream = new FileStream(remixpath, FileMode.Create))
                        {
                            addremix.Remixfile.CopyTo(stream);
                        }
                        return RedirectToAction(nameof(Get_remix_Page));
                    }
                    else
                    {
                        ViewBag.error = "خطایی در افزودن رخ داد";
                        return View("Views/Admin Page/Remix_Manage.cshtml", refresh(null, null));
                    }
                }
                else
                {
                    ViewBag.error = "فرمت فایل درست نیست";
                    return View("Views/Admin Page/Remix_Manage.cshtml", refresh(null, null));
                }

            }
            else
            {
                ViewBag.error = "لطفا فیلد ها را پر کنید";
                return View("Views/Admin Page/Remix_Manage.cshtml", refresh(null, null));
            }
        }

        public IActionResult Delete_remix(int remixid)
        {
            if (remixid != 0)
            {
                var remix = _remixData.get_remix_by_id(remixid);
                bool remove_comment = _commentData.delete_post_comment(remixid, 2);
                bool remove_status = _remixData.delete_remix(remix);
                if (remove_status == true)
                {
                    string deltepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Music", "Remix", remix.File_Name);
                    System.IO.File.Delete(deltepath);
                    return RedirectToAction(nameof(Get_remix_Page));

                }
                else
                {
                    ViewBag.berror = "خطایی در حذف رخ داد";
                    return View("Views/Admin Page/Remix_Manage.cshtml", refresh(null, null));
                }

            }
            else
            {
                return BadRequest();
            }
        }

        public Admin_Remix_Viewmodel edit_refrsh(int refrshid)
        {
            List<Remix> remix = new List<Remix>();
            remix.Add(_remixData.get_remix_by_id(refrshid));
            var musicname = _musicData.Get_Music_name();
            musicname = musicname.OrderBy(e => e.Name).ToList();
            Admin_Remix_Viewmodel admin_Remix_ = new Admin_Remix_Viewmodel()
            {
                music_name = musicname,
                remixes = remix
            };
            return admin_Remix_;
        }
        public IActionResult Edit_remix_page(int remixid)
        {
            var remix = edit_refrsh(remixid);
            if (remix.remixes.FirstOrDefault().music != null)
            {
                ViewBag.mainsongid = remix.remixes.FirstOrDefault().music.SongId;
            }
            else
            {
                ViewBag.mainsongid = 0;
            }
            return View("Views/Admin Page/Remix_Edit.cshtml", remix);
        }
        [HttpPost]
        public IActionResult Edit_remix(Remix remix, int music_id)
        {
            if (remix != null)
            {
                if (remix.Remix_Creator != null && remix.RemixName != null)
                {
                    var main_remix = _remixData.get_remix_by_id(remix.RemixId);
                    string mainfilename = main_remix.File_Name;
                    Remix newremix = new Remix()
                    {
                        RemixName = main_remix.RemixName,
                        RemixId = remix.RemixId,
                        Remix_Creator = remix.Remix_Creator,
                        comment_status = remix.comment_status,
                    };
                    if (music_id == 0)
                    {
                        newremix.music = null;
                    }
                    else
                    {
                        newremix.music = _musicData.Get_Music_By_Id(music_id);
                    }
                    if (remix.Remixfile != null)
                    {
                        string newextension = Path.GetExtension(remix.Remixfile.FileName).ToUpper();
                        if (newextension == ".MP3")
                        {
                            newremix.File_Name = Guid.NewGuid().ToString() + newextension;
                        }
                        else
                        {
                            ViewBag.error = "فرمت فایل درست نیست";
                            var getmainmusic = edit_refrsh(remix.RemixId);
                            if (getmainmusic.remixes.FirstOrDefault().music != null)
                            {
                                ViewBag.mainsongid = getmainmusic.remixes.FirstOrDefault().music.SongId;
                            }
                            else
                            {
                                ViewBag.mainsongid = 0;
                            }
                            return View("Views/Admin Page/Remix_Edit.cshtml", edit_refrsh(remix.RemixId));
                        }
                    }
                    bool editstatus = _remixData.Edit_remix(newremix);
                    if (editstatus == true)
                    {
                        if (remix.Remixfile != null)
                        {
                            string removepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Music", "Remix", mainfilename);
                            System.IO.File.Delete(removepath);

                            string newpath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Music", "Remix", newremix.File_Name);
                            using (FileStream stream = new FileStream(newpath, FileMode.Create))
                            {
                                remix.Remixfile.CopyTo(stream);
                            }
                        }
                        return RedirectToAction(nameof(Get_remix_Page));
                    }
                    else
                    {
                        var getmainmusic = edit_refrsh(remix.RemixId);
                        if (getmainmusic.remixes.FirstOrDefault().music != null)
                        {
                            ViewBag.mainsongid = getmainmusic.remixes.FirstOrDefault().music.SongId;
                        }
                        else
                        {
                            ViewBag.mainsongid = 0;
                        }
                        ViewBag.error = "خطایی در ویرایش رخ داد";
                        return View("Views/Admin Page/Remix_Edit.cshtml", edit_refrsh(remix.RemixId));
                    }
                }
                else
                {
                    var getmainmusic = edit_refrsh(remix.RemixId);
                    if (getmainmusic.remixes.FirstOrDefault().music != null)
                    {
                        ViewBag.mainsongid = getmainmusic.remixes.FirstOrDefault().music.SongId;
                    }
                    else
                    {
                        ViewBag.mainsongid = 0;
                    }
                    ViewBag.error = "لطفا فیلد ها را پر کنید";
                    return View("Views/Admin Page/Remix_Edit.cshtml", edit_refrsh(remix.RemixId));
                }
            }
            else
            {
                var getmainmusic = edit_refrsh(remix.RemixId);
                if (getmainmusic.remixes.FirstOrDefault().music != null)
                {
                    ViewBag.mainsongid = getmainmusic.remixes.FirstOrDefault().music.SongId;
                }
                else
                {
                    ViewBag.mainsongid = 0;
                }
                ViewBag.error = "حجم فایل درست نیست";
                return View("Views/Admin Page/Remix_Edit.cshtml", edit_refrsh(remix.RemixId));
            }
        }
    }

}

