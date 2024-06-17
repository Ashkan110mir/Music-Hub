using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Music_Website.Data.Albums_Data;
using Music_Website.Data.Music_Data;
using Music_Website.Data.Singer;
using Music_Website.Models;
using System.IO;

namespace Music_Website.Controllers
{

    [Authorize]
    public class SingerController : Controller
    {
        private ISingerData _singerdata;
        private IMusicData _musicdate;
        private IAlbumsData _albumdata;

        public SingerController(ISingerData Singerdata, IMusicData musicData, IAlbumsData albumsData)
        {
            _singerdata = Singerdata;
            _musicdate = musicData;
            _albumdata = albumsData;
        }

        public IActionResult Singer_page(string serachname, int pageid = 1)
        {
            if (serachname == null)
            {
                ViewBag.currentpage = pageid;
                int singer_count = _singerdata.singer_count();
                if (singer_count % 20 == 0)
                {
                    ViewBag.pagecount = singer_count / 20;
                }
                else
                {
                    ViewBag.pagecount = singer_count / 20 + 1;
                }
                return View("Views/Admin Page/Singer_Manage.cshtml", _singerdata.Get_Paging_Singer(pageid, 20));
            }
            else
            {
                return View("Views/Admin Page/Singer_Manage.cshtml", _singerdata.Search_singer(serachname));
            }
        }
        [HttpPost]
        public IActionResult Add_Singer(Singer addSinger)
        {
            if (addSinger.SingerName == "" || addSinger.Singer_Lastname == "" || addSinger.artistName == null || addSinger.Picture_File == null)
            {
                ViewBag.error = "لطفا مقادیر را درست وارد کنید";
                return View("Views/Admin Page/Singer_Manage.cshtml", _singerdata.Get_All_Singer());
            }
            else
            {

                string fileextension = Path.GetExtension(addSinger.Picture_File.FileName).ToString();
                fileextension = fileextension.ToUpper();
                //check format
                if (fileextension != ".JPG" && fileextension != ".PNG" && fileextension != ".SVG")
                {
                    ViewBag.error = "فرمت عکس درست نیست فرمت درست را وارد کنید";
                    return View("Views/Admin Page/Singer_Manage.cshtml", _singerdata.Get_All_Singer());
                }
                //format is ok
                else
                {
                    string filename = Guid.NewGuid().ToString() + fileextension.ToString();
                    ViewBag.error = "";
                    Singer singer = new Singer()
                    {
                        SingerName = addSinger.SingerName,
                        Singer_Lastname = addSinger.Singer_Lastname,
                        artistName = addSinger.artistName,
                        profile_picture_name = filename,
                    };
                    string resualt = _singerdata.Add_Singer(singer);
                    if (resualt == "false")
                    {
                        ViewBag.error = "اقزودن با خطا مواجه شد";
                    }
                    else if (resualt == "exist")
                    {
                        ViewBag.error = "خواننده تکراری است";
                    }
                    else
                    {
                        var copyfilepach = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "Singer Image", filename).ToString();
                        using (FileStream stream1 = new FileStream(copyfilepach, FileMode.Create))
                        {
                            addSinger.Picture_File.CopyTo(stream1);
                        }

                        ViewBag.error = "با موفقیت افزوده شد";
                    }
                    return View("Views/Admin Page/Singer_Manage.cshtml", _singerdata.Get_All_Singer());
                }

            }

        }

        public IActionResult Remove_Singer(int singerid, string pic)
        {
            List<int> singerlist = new List<int>();
            singerlist.Add(singerid);
            var main_album = _albumdata.get_albums_by_main_singers(singerlist);
            List<Music> musics = new List<Music>();
            //has main album
            if (main_album != null)
            {
                foreach (var album in main_album)
                {
                    musics.AddRange(_musicdate.Get_Music_By_Album_id(album.AlbumId));
                }
                if (musics.Count > 0)
                {
                    foreach (var music in musics)
                    {
                        string iamgepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "Music Image", music.Image_Filename);
                        string songpath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot","Music", music.Song_FileName);

                        System.IO.File.Delete(iamgepath);
                        System.IO.File.Delete(songpath);
                        _musicdate.remove_Music(music);
                    }
                }
                foreach (var album in main_album)
                {
                    
                    _albumdata.delete_Album(album.AlbumId);
                }
            }
            //it in others album
            var other_albums=_albumdata.getAlbumsBySingersId(singerlist);
            List<Singer> singers = new List<Singer>();
            var singer = _singerdata.Get_singer_by_id(singerlist.First());
            if (other_albums.Count > 0)
            {
               
                singers.Add(singer);
                foreach(var album in other_albums)
                {
                    _albumdata.add_or_remove_more_singer(album.AlbumId, singers, 0);
                }
            }
            //other music
            var musicss =_musicdate.Get_music_by_singerid(singerlist.First());
            if (musicss.Count>0)
            {
                foreach(var music in musicss)
                {
                    music.singers.Remove(singer);
                    //have not other singer
                    if (music.singers.Count == 0)
                    {
                        _musicdate.remove_Music(music);
                    }
                    // have other singer
                    else
                    {
                        _musicdate.edit_music(music);
                    }
                }
            }


            bool remove_was_Success = _singerdata.Delete_Singer(singerid);
            if (remove_was_Success)
            {
                string pach = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images/Singer Image", pic);
                System.IO.File.Delete(pach);
                ViewBag.messeage = "با موفقیت حذف شد";
            }
            else
            {
                ViewBag.messeage = "مشکلی در حذف پیش آمد";
            }

            return RedirectToAction(nameof(Singer_page));
        }

        public IActionResult Get_Edit_Singer(int singerid)
        {
            Singer singer = new Singer();
            singer = _singerdata.Get_singer_by_id(singerid);
            return View("Views/Admin Page/Singer_Edit.cshtml", singer);
        }
        [HttpPost]
        public IActionResult Edit_Singer(Singer? singer, string pic, int id)
        {
            if (singer.SingerName != null && singer.Singer_Lastname != null && singer.artistName != null)
            {
                //comlete change
                if (singer.Picture_File != null)
                {
                    string extention = Path.GetExtension(singer.Picture_File.FileName).ToString().ToUpper();
                    if (extention != ".JPG" && extention != ".PNG" && extention != ".SVG")
                    {
                        ViewBag.error = "فرمت تصویر درست نیست";
                        return View("Views/Admin Page/Singer_Edit.cshtml", _singerdata.Get_singer_by_id(id));

                    }
                    else
                    {
                        string filenname = Guid.NewGuid().ToString() + extention;
                        Singer editsinger = new Singer()
                        {
                            SingerId = id,
                            SingerName = singer.SingerName,
                            Singer_Lastname = singer.Singer_Lastname,
                            artistName = singer.artistName,
                            profile_picture_name = filenname,
                        };
                        bool editwassuccess = _singerdata.Edit_Singer(editsinger);
                        if (editwassuccess)
                        {
                            string pach = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "Singer Image", pic);
                            System.IO.File.Delete(pach);
                            string pach2 = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "Singer Image", filenname);
                            using (FileStream stream = new FileStream(pach2, FileMode.Create))
                            {
                                singer.Picture_File.CopyTo(stream);
                            }
                            return RedirectToAction(nameof(Singer_page));
                        }
                        else
                        {
                            ViewBag.error = "خطایی در ویرایش رخ داد";
                            return View("Views/Admin Page/Singer_Edit.cshtml", _singerdata.Get_singer_by_id(id));
                        }

                    }
                }

                else
                {
                    Singer editsinger = new Singer()
                    {
                        SingerId = id,
                        SingerName = singer.SingerName,
                        Singer_Lastname = singer.Singer_Lastname,
                        artistName = singer.artistName,
                        profile_picture_name = pic,
                    };
                    bool editwassuccess = _singerdata.Edit_Singer(editsinger);
                    if (editwassuccess)
                    {
                        return RedirectToAction(nameof(Singer_page));
                    }
                    else
                    {
                        ViewBag.error = "خطایی در ویرایش رخ داد";
                        return View("Views/Admin Page/Singer_Edit.cshtml", _singerdata.Get_singer_by_id(id));
                    }

                }

            }
            else
            {
                ViewBag.error = "همه فیلد ها را پر کنید";
                return View("Views/Admin Page/Singer_Edit.cshtml", _singerdata.Get_singer_by_id(id));
            }


        }
    }
}
