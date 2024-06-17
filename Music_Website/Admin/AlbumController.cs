using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Music_Website.Data.Albums_Data;
using Music_Website.Data.Music_Data;
using Music_Website.Data.Singer;
using Music_Website.Models;

namespace Music_Website.Admin_Page
{
    [Authorize]
    public class AlbumController : Controller
    {
        private ISingerData _singerdata;
        private IAlbumsData _albumsdata;
        private IMusicData _musicdata;
        public AlbumController(ISingerData singer, IAlbumsData albumsData, IMusicData musicData)
        {
            _singerdata = singer;
            _albumsdata = albumsData;
            _musicdata = musicData;
        }

        public Albums_Singer_ViewModel Refresh(string? searchname, int pageid = 1)
        {
            if (searchname == "")
            {
                Albums_Singer_ViewModel albums_Singer = new Albums_Singer_ViewModel();
                albums_Singer.albums = _albumsdata.Get_paging_album_admin(pageid);
                var singers = _singerdata.GetSingers_Name_ViewModels();
                singers = singers.OrderBy(e => e.ArtistName).ToList();
                albums_Singer.singers = singers;
                return albums_Singer;
            }
            else
            {
                Albums_Singer_ViewModel albums_Singer = new Albums_Singer_ViewModel();
                albums_Singer.albums = _albumsdata.Search_Albums(searchname);
                albums_Singer.singers = _singerdata.GetSingers_Name_ViewModels();
                return albums_Singer;
            }
        }
        public IActionResult Albums_Page(string? searchname, int pageid = 1)
        {
            if (searchname == null)
            {
                ViewBag.currentpage = pageid;
                int pagecount = _albumsdata.albums_count();
                if (pagecount % 20 == 0)
                {
                    ViewBag.pagecount = pagecount / 20;
                }
                else
                {
                    ViewBag.pagecount = pagecount / 20 + 1;
                }
                return View("Views/Admin Page/Albums_Manage.cshtml", Refresh("", pageid));
            }
            else
            {
                return View("Views/Admin Page/Albums_Manage.cshtml", Refresh(searchname));
            }
        }
        [HttpPost]
        public IActionResult Add_Albums(string name, int singersid)
        {
            if (name != null && singersid != 0)
            {
                var singer = _singerdata.Get_singer_by_id(singersid);
                bool addwassucces = _albumsdata.Add_Album(name, singer);
                if (addwassucces)
                {
                    return RedirectToAction(nameof(Albums_Page));
                }
                else
                {
                    ViewBag.terror = "افزودن با خطا مواجه شد";
                    return View("Views/Admin Page/Albums_Manage.cshtml", Refresh(""));
                }
            }
            else
            {
                ViewBag.terror = "نام آلبوم یا خواننده آن مشخص نشده";
                return View("Views/Admin Page/Albums_Manage.cshtml", Refresh(""));
            }

        }
        public IActionResult Edit_Album_page(int albumid)
        {
            var album = _albumsdata.GetAlbumsByid(albumid);
            var singer = _singerdata.GetSingers_Name_ViewModels().OrderBy(e => e.ArtistName).ToList();
            Albums_Singer_ViewModel albums_Singer_ = new Albums_Singer_ViewModel()
            {
                singers = singer,
                albums = album
            };
            return View("Views/Admin Page/Albums_Edit.cshtml", albums_Singer_);
        }
        public IActionResult Edit_Album(Albums album)
        {
            if (album.AlbumName != null)
            {
                if (_albumsdata.Edit_Albums(album))
                {
                    return RedirectToAction(nameof(Albums_Page));
                }
                else
                {
                    ViewBag.berror = "خطایی در ویرایش رخ داد";
                    return View("Views/Admin Page/Albums_Manage.cshtml", Refresh(""));
                }
            }
            else
            {
                ViewBag.berror = "لطفا نام آلبوم را وارد کنید";
                return View("Views/Admin Page/Albums_Manage.cshtml", Refresh(""));
            }

        }
        public IActionResult Delete_Album(int albumid)
        {
            if (albumid > 0)
            {
                List<Music> album_musics = new List<Music>();
                album_musics.AddRange(_musicdata.Get_Music_By_Album_id(albumid));
                foreach (var music in album_musics)
                {
                    string iamgepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "Music Image", music.Image_Filename);
                    string songpath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Music", music.Song_FileName);

                    System.IO.File.Delete(iamgepath);
                    System.IO.File.Delete(songpath);
                    _musicdata.remove_Music(music);
                }
                bool deletwassuccess = _albumsdata.delete_Album(albumid);
                if (deletwassuccess == true)
                {
                    return RedirectToAction(nameof(Albums_Page));
                }
                else
                {
                    ViewBag.berror = "خطایی در حذف رخ داد";
                    return View("Views/Admin Page/Albums_Manage.cshtml", Refresh(""));
                }
            }
            else
            {
                return NotFound();
            }
        }

    }
}
