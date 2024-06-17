using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Operations;
using Music_Website.Data.Admin_Data;
using Music_Website.Data.Albums_Data;
using Music_Website.Data.Comment_Data;
using Music_Website.Data.Music_Data;
using Music_Website.Data.Singer;
using Music_Website.Models;
using Music_Website.utility;
using NuGet.Packaging;
using NuGet.Packaging.Signing;
using System.Net.NetworkInformation;
using System.Security.Claims;
namespace Music_Website.Admin
{
    [Authorize]
    public class MusicController : Controller
    {
        private IAlbumsData _albumsData;
        private ISingerData _singerData;
        private IMusicData _musicData;
        private IAdminData _admindata;
        private ICommentData _commentdata;
        public MusicController(IAlbumsData albumsData, ISingerData singerData, IMusicData musicData, IAdminData adminData, ICommentData commentData)
        {
            _albumsData = albumsData;
            _singerData = singerData;
            _musicData = musicData;
            _admindata = adminData;
            _commentdata = commentData;
        }
        private Admin_Music_Page_ViewModel Refresh(List<int> allsingerid, string? searchname, int pageid = 1)
        {
            //singer not select for add
            if (allsingerid == null || allsingerid.Count < 1)
            {
                Admin_Music_Page_ViewModel admin_Music = new Admin_Music_Page_ViewModel();
                //show full music
                if (searchname == null)
                {
                    int total_count = _musicData.Music_Count();
                    if (total_count % 20 == 0)
                    {
                        ViewBag.pagecount = total_count / 20;
                    }
                    else
                    {
                        ViewBag.pagecount = (total_count / 20) + 1;
                    }
                    ViewBag.currentpage = pageid;
                    var singer = _singerData.GetSingers_Name_ViewModels();
                    admin_Music.singers = singer;
                    admin_Music.singers = admin_Music.singers.OrderBy(e => e.ArtistName).ToList();
                    admin_Music.Musics = _musicData.Get_Paging_Admin_music(pageid);
                    return admin_Music;
                }
                //show searched music
                else
                {
                    var singer = _singerData.GetSingers_Name_ViewModels();
                    admin_Music.singers = singer;
                    admin_Music.Musics = _musicData.Search_Music(searchname);
                    return admin_Music;
                }
            }
            //singer slecet for add
            else
            {
                List<Singer> singer = new List<Singer>();
                List<Albums> albums = new List<Albums>();
                albums.AddRange(_albumsData.get_albums_by_main_singers(allsingerid));
                foreach (var sing in allsingerid)
                {
                    singer.Add(_singerData.Get_singer_by_id(sing));

                }
                Admin_Music_Page_ViewModel admin_Music_Page_ViewModel = new Admin_Music_Page_ViewModel()
                {
                    albums = albums,
                    Full_Singer = singer,
                };
                admin_Music_Page_ViewModel.Musics = _musicData.Get_All_Music();
                return admin_Music_Page_ViewModel;
            }

        }
        public IActionResult Get_Album(List<int> allsingerid)
        {
            if (allsingerid.Count > 0)
            {
                return View("Views/Admin Page/Music_Manage.cshtml", Refresh(allsingerid, null));
            }
            else
            {
                ViewBag.error = "لطفا خواننده را انتخاب کنید";
                return View("Views/Admin Page/Music_Manage.cshtml", Refresh(allsingerid, null));
            }
        }
        public IActionResult Music_Manage_Page(string? searchname, int pageid = 1)
        {
            if (searchname == null)
            {
                return View("Views/Admin Page/Music_Manage.cshtml", Refresh(null, null, pageid));
            }
            else
            {
                return View("Views/Admin Page/Music_Manage.cshtml", Refresh(null, searchname, pageid));
            }

        }
        [HttpPost]
        public IActionResult Add_Music(Music music, List<int> allsingerid, int albumid)
        {
            //inputs are enterd
            if (music.Song_Name != null && music.Song_Name.Length <= 70 && allsingerid.Count > 0)
            {
                string song_extension = Path.GetExtension(music.Song_File.FileName).ToString().ToUpper();
                string image_extension = Path.GetExtension(music.Image_File.FileName).ToString().ToUpper();
                int adminid = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var admin = _admindata.get_admin(adminid);
                List<Singer> moresinger = new List<Singer>();
                foreach (var singer in allsingerid)
                {
                    moresinger.Add(_singerData.Get_singer_by_id(singer));
                }

                Music newmusic = new Music()
                {
                    Song_Name = music.Song_Name,
                    Song_Description = music.Song_Description,
                    Song_Date = DateTime.Now,
                    Music_Style = music.Music_Style,
                    in_main_index = false,
                    comment_status = music.comment_status,
                    admin = admin,
                    singers = moresinger,
                };
                //song file formats are ok
                if (song_extension == ".MP3")
                {
                    newmusic.Song_FileName = Guid.NewGuid().ToString() + song_extension;
                }
                //song file format is not correct
                else
                {
                    ViewBag.error = "لطفا فایل آهنگ را با فرمت درست انتخاب کنید";
                    return View("Views/Admin Page/Music_Manage.cshtml", Refresh(allsingerid, null));
                }
                //image file format is ok
                if (image_extension == ".JPG" || image_extension == ".SVG" || image_extension == ".PNG")
                {
                    newmusic.Image_Filename = Guid.NewGuid().ToString() + image_extension;
                }
                //image file format is not correct
                else
                {
                    ViewBag.error = "لطفا فایل عکس را با فرمت درست انتخاب کنید";
                    return View("Views/Admin Page/Music_Manage.cshtml", Refresh(allsingerid, null));
                }
                //its not in the album
                if (albumid == 0)
                {
                    newmusic.album = null;
                }
                //its has album
                else
                {
                    var album = _albumsData.GetAlbumByid(albumid);
                    newmusic.album = album;
                    _albumsData.change_count(album, 1);
                    List<Singer> moresingers = new List<Singer>();
                    foreach (var singerid in moresinger)
                    {
                        if (singerid.SingerId != album.main_singer.SingerId)
                        {
                            moresingers.Add(_singerData.Get_singer_by_id(singerid.SingerId));
                        }
                    }
                    _albumsData.add_or_remove_more_singer(album.AlbumId, moresingers, 1);
                }

                //start add
                bool add_status = _musicData.Add_Music(newmusic);
                //add was success
                if (add_status)
                {
                    //upload image
                    string imagepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "Music Image", newmusic.Image_Filename);
                    using (FileStream filestream = new FileStream(imagepath, FileMode.Create))
                    {
                        music.Image_File.CopyTo(filestream);
                    }

                    //upload music
                    string musicepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Music", newmusic.Song_FileName);
                    using (FileStream filestream = new FileStream(musicepath, FileMode.Create))
                    {
                        music.Song_File.CopyTo(filestream);
                    }
                    return RedirectToAction(nameof(Music_Manage_Page));
                }
                //add wasnt success
                else
                {
                    ViewBag.error = "خطایی در افزودن رخ داد";
                    return View("Views/Admin Page/Music_Manage.cshtml", Refresh(allsingerid, null));
                }

            }
            //input not enter
            else
            {
                ViewBag.error = "لطفا فیلدهارا پر کنید";
                return View("Views/Admin Page/Music_Manage.cshtml", Refresh(allsingerid, null));
            }




        }

        public IActionResult Remove_Music(int musicid, int albumid)
        {
            if (musicid != 0)
            {
                Music music = new Music();

                if (music != null)
                {

                    music = _musicData.Get_Music_By_Id(musicid);
                    bool remove_comment = _commentdata.delete_post_comment(musicid, 1);
                    if (albumid != 0)
                    {
                        Albums albums = new Albums();
                        albums = _albumsData.GetAlbumByid(albumid);
                        _albumsData.change_count(albums, 0);
                        List<Singer> singers = new List<Singer>();
                        singers = _singerData.Get_Singer_by_Music_Id(musicid);
                        foreach (var singer in singers.ToList())
                        {
                            if (singer.SingerId == albums.main_singer.SingerId)
                            {
                                singers.Remove(singer);
                            }
                        }
                        _albumsData.add_or_remove_more_singer(albumid, singers, 0);
                    }
                    bool removewassucces = _musicData.remove_Music(music);
                    string song_path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Music", music.Song_FileName);
                    string pic_path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "Music Image", music.Image_Filename);  
                    if (removewassucces)
                    {
                        System.IO.File.Delete(song_path);
                        System.IO.File.Delete(pic_path);
                        
                        return RedirectToAction(nameof(Music_Manage_Page));
                    }
                    else
                    {
                        ViewBag.error = "خطایی در حذف رخ داد";
                        return View("Views/Admin Page/Music_Manage.cshtml", Refresh(null, null));
                    }
                }
                else
                {
                    return BadRequest();
                }
            }
            else
            {
                return BadRequest();
            }

        }

        public IActionResult Edit_Music_page(int musicid)
        {
            var music = refresh_edit(musicid);
            foreach (var album in music.Musics)
            {
                ViewBag.albumid = album.AlbumId;
            }
            return View("Views/Admin Page/Music_Edit.cshtml", refresh_edit(musicid));
        }
        public Admin_Music_Page_ViewModel refresh_edit(int musicid)
        {
            List<Music> musics = new List<Music>();
            musics.Add(_musicData.Get_Music_By_Id(musicid));
            List<int> singerid = new List<int>();
            var Full_Singer = _singerData.Get_Singer_by_Music_Id(musicid);
            foreach (var singer in Full_Singer)
            {
                singerid.Add(singer.SingerId);
            }
            var album = _albumsData.get_albums_by_main_singers(singerid);
            album = album.OrderBy(e => e.AlbumName).ToList();
            Admin_Music_Page_ViewModel admin_Music_ = new Admin_Music_Page_ViewModel()
            {
                Full_Singer = _singerData.Get_Singer_by_Music_Id(musicid),
                albums = album,
                Musics = musics,
            };
            return admin_Music_;
        }
        [HttpPost]
        public IActionResult Edit_Music(Music editmusic)
        {
            //inputs are ok
            if (editmusic.Song_Name != null && editmusic.Song_Description != null && editmusic.Music_Style != null)
            {
                Music mainmusic = new Music();
                mainmusic = _musicData.Get_Music_By_Id(editmusic.SongId);
                string mainpicname = mainmusic.Image_Filename;
                string mainsongname = mainmusic.Song_FileName;
                var mainalbum = _albumsData.GetAlbumByid(mainmusic.AlbumId);
                var newalbum = _albumsData.GetAlbumByid(editmusic.AlbumId);
                Music Newmusic = new Music()
                {
                    SongId = editmusic.SongId,
                    Song_Name = editmusic.Song_Name,
                    Song_Description = editmusic.Song_Description,
                    comment_status = editmusic.comment_status,
                    Song_Date = mainmusic.Song_Date,
                    album = newalbum,
                    Music_Style = editmusic.Music_Style,
                };
                //image enter
                if (editmusic.Image_File != null)
                {
                    string picextenstin = Path.GetExtension(editmusic.Image_File.FileName).ToString().ToUpper();
                    //pic format is ok
                    if (picextenstin == ".PNG" || picextenstin == ".JPG" || picextenstin == ".SVG")
                    {
                        Newmusic.Image_Filename = Guid.NewGuid().ToString() + picextenstin;
                    }
                    //pic format is not ok
                    else
                    {

                        ViewBag.albumid = mainalbum.AlbumId;
                        ViewBag.error = "فزمت عکس را درست وارد کنید";
                        return View("Views/Admin Page/Music_Edit.cshtml", refresh_edit(editmusic.SongId));
                    }
                }
                //song enter
                if (editmusic.Song_File != null)
                {
                    string songextenson = Path.GetExtension(editmusic.Song_File.FileName).ToString().ToUpper();
                    //song format is ok
                    if (songextenson == ".MP3")
                    {
                        Newmusic.Song_FileName = Guid.NewGuid().ToString() + songextenson;
                    }
                    //song format is not ok
                    else
                    {
                        ViewBag.albumid = mainalbum.AlbumId;
                        ViewBag.error = "فرمت آهنگ را درست وارد کنید";
                        return View("Views/Admin Page/Music_Edit.cshtml", refresh_edit(editmusic.SongId));
                    }
                }
                bool editwassucces = _musicData.edit_music(Newmusic);
                // edit was succes
                if (editwassucces == true)
                {
                    if (editmusic.Image_File != null)
                    {
                        string mainimagepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "Music Image", mainpicname);
                        System.IO.File.Delete(mainimagepath);

                        string newpicpath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "Music Image", Newmusic.Image_Filename);
                        using (FileStream stream = new FileStream(newpicpath, FileMode.Create))
                        {
                            editmusic.Image_File.CopyTo(stream);
                        }
                    }
                    if (editmusic.Song_File != null)
                    {
                        string mainsongpath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Music", mainsongname);
                        System.IO.File.Delete(mainsongpath);

                        string newmusicpath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Music", Newmusic.Song_FileName);
                        using (FileStream stream = new FileStream(newmusicpath, FileMode.Create))
                        {
                            editmusic.Song_File.CopyTo(stream);
                        }
                    }
                    //album count change
                    if (mainalbum != null && editmusic.AlbumId == 0)
                    {
                        List<Singer> singers =new List<Singer>();
                        var more_singer = _singerData.Get_Singer_by_Music_Id(mainmusic.SongId);      
                        foreach (var singer in more_singer.ToList())
                        {
                            if (singer.SingerId == mainalbum.main_singer.SingerId)
                            {
                                singers.Remove(singer);
                            }
                        }
                        singers.AddRange(more_singer);
                        _albumsData.add_or_remove_more_singer(mainalbum.AlbumId, singers, 0);
                        _albumsData.change_count(mainalbum, 0);
                    }
                    if (mainalbum == null && editmusic.AlbumId != 0)
                    {
                        List<Singer> singers = new List<Singer>();
                        var more_singer = _singerData.Get_Singer_by_Music_Id(mainmusic.SongId);
                        foreach (var singer in more_singer.ToList())
                        {
                            if (singer.SingerId != newalbum.main_singer.SingerId)
                            {
                                singers.Add(singer);
                            }
                        }
                        _albumsData.add_or_remove_more_singer(newalbum.AlbumId, singers, 1);
                        _albumsData.change_count(newalbum, 1);
                    }
                    if (mainalbum != null && editmusic.AlbumId != 0)
                    {

                        //singerfor delete to album
                        List<Singer> singersforremove = new List<Singer>();
                        var more_singer_foreremove = _singerData.Get_Singer_by_Music_Id(mainmusic.SongId);
                        foreach (var singer in more_singer_foreremove.ToList())
                        {
                            if (singer.SingerId == mainalbum.main_singer.SingerId)
                            {
                                more_singer_foreremove.Remove(singer);
                            }
                        }
                        singersforremove.AddRange(more_singer_foreremove);
                        _albumsData.add_or_remove_more_singer(mainalbum.AlbumId, singersforremove, 0);
                        _albumsData.change_count(mainalbum, 0);


                        //singers for add to album
                        List<Singer> singers = new List<Singer>();
                        var more_singer = _singerData.Get_Singer_by_Music_Id(mainmusic.SongId);
                        foreach (var singer in more_singer.ToList())
                        {
                            if (singer.SingerId != newalbum.main_singer.SingerId)
                            {
                                singers.Add(singer);
                            }
                        }
                        _albumsData.add_or_remove_more_singer(newalbum.AlbumId, singers, 1);
                        _albumsData.change_count(newalbum, 1);
                    }
                    return RedirectToAction(nameof(Music_Manage_Page));
                }
                // edit wasnt succes
                else
                {
                    ViewBag.error = "خطایی در ویرایش رخ داد";
                    ViewBag.albumid = mainalbum.AlbumId;
                    return View("Views/Admin Page/Music_Edit.cshtml", refresh_edit(editmusic.SongId));
                }
            }
            //inputs are not ok
            else
            {
                ViewBag.error = "فیلد ها را پر کنید";
                var album = refresh_edit(editmusic.SongId);
                foreach (var item in album.Musics)
                {
                    ViewBag.albumid = item.album.AlbumId;
                }
                return View("Views/Admin Page/Music_Edit.cshtml", refresh_edit(editmusic.SongId));
            }
        }
    }
}
