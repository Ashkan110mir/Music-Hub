using Azure;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Music_Website.Data.Admin_Data;
using Music_Website.Data.Albums_Data;
using Music_Website.Data.Comment_Data;
using Music_Website.Data.Contact_Us_Data;
using Music_Website.Data.Music_Data;
using Music_Website.Data.Music_Video_Data;
using Music_Website.Data.Remix_Data;
using Music_Website.Data.Singer;
using Music_Website.Models;
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Net.Sockets;
using System.Security.Claims;
using static System.Net.Mime.MediaTypeNames;


namespace Music_Website.Admin_Page
{
    [Authorize]
    public class AdminController : Controller
    {
        #region Data
        private ISingerData _singerdata;
        private IMusicData _musicData;
        private IMusicVideo_Data _musicvideodata;
        private IRemixData _remixdata;
        private IAlbumsData _albumsData;
        private ICommentData _commentData;
        private IContactusDATA _contactusDATA;
        private IAdminData _adminData;
        #endregion

        public AdminController(ISingerData singerData, IMusicData musicData, IMusicVideo_Data musicvideodata, IRemixData remixData, IAlbumsData albumsData, ICommentData commentData, IContactusDATA contactusDATA, IAdminData adminData)
        {
            _singerdata = singerData;
            _musicData = musicData;
            _musicvideodata = musicvideodata;
            _remixdata = remixData;
            _albumsData = albumsData;
            _commentData = commentData;
            _contactusDATA = contactusDATA;
            _adminData = adminData;
        }
        //0=singer image
        //1=music image
        //2=music video
        //3=music video poster
        [Route("showmedia")]
        [AllowAnonymous]
        public IActionResult show_media(int show_media_type, string url,string? posterpath)
        {
            string[] strings = new string[3];
            strings[0] = show_media_type.ToString();
            strings[1] = url;
            strings[2]= posterpath;
            string urlname = strings[1].ToString();
            return View("Views/Show Media/Show_Media.cshtml", strings);
        }

        [AllowAnonymous] 
        public IActionResult Admin_Login()
        {
            return View("Views/Admin Page/Admin_Login.cshtml");

        }
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Admin_Login(string username, string password, bool remembreme)
        {
            var admin = _adminData.get_admin(username, password);
            if (admin != null)
            {
                var claim = new List<Claim>()
                {
                    new Claim(ClaimTypes.NameIdentifier,admin.Admin_ID.ToString()),
                    new Claim(ClaimTypes.Name,admin.Admin_Name.ToString()),
                    new Claim(ClaimTypes.Email,admin.Email.ToString()),
                };
                var identity = new ClaimsIdentity(claim, CookieAuthenticationDefaults.AuthenticationScheme);
                var Principal = new ClaimsPrincipal(identity);
                var proparties = new AuthenticationProperties
                {
                    IsPersistent = remembreme
                };
                
                HttpContext.SignInAsync(Principal, proparties);
                return RedirectToAction(nameof(admin_page));
            }
            else
            {
                ViewBag.mess = "نام کاربری یا رمز عبور اشتباه است";
                return View("Views/Admin Page/Admin_Login.cshtml");
            }
        }

        public IActionResult Admin_Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Admin_Login));
        }
        public IActionResult admin_page()
        {
            AdminDashboardViewModel admindash = new AdminDashboardViewModel()
            {
                singer_count = _singerdata.singer_count(),
                Music_count = _musicData.Music_Count(),
                music_video_count = _musicvideodata.music_video_count(),
                remix_count = _remixdata.RemixCount(),
                albums_count = _albumsData.albums_count(),
                accpect_parent_comment_count = _commentData.accepted_parent_comment_count(),
                not_accpect_paremt_comment_count = _commentData.not_accepted_parent_comment_count(),
                parent_comment_count=_commentData.parent_comment_count(),
                accpect_child_comment_count=_commentData.accpeted_child_comment_count(),
                not_accpect_child_comment_count=_commentData.not_accpeted_child_comment_count(),
                child_comment_count=_commentData.child_comment_count(),
                comment_count=_commentData.Comment_Count(),
                notseeing_contactus = _contactusDATA.not_see_count(),
                seeing_contactus = _contactusDATA.see_count(),
            };
            return View("Views/Admin Page/Admin_Page.cshtml", admindash);
        }
    }
}
