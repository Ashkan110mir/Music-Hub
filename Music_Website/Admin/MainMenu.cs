using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Music_Website.Data.Music_Data;
using Music_Website.Data.Music_Video_Data;
using Music_Website.Models;

namespace Music_Website.Admin
{
    [Authorize]
    public class MainMenu : Controller
    {
        private IMusicData _musicData;
        private IMusicVideo_Data _musicVideoData;
        public MainMenu(IMusicData musicData, IMusicVideo_Data musicVideo_Data)
        {
            _musicData = musicData;
            _musicVideoData = musicVideo_Data;
        }
        public Mian_index_viewmodel refresh()
        {
            Mian_index_viewmodel mian_Index = new Mian_index_viewmodel();
            
            var newsongs=_musicData.Get_not_main_menu_selected_music().ToList();
            newsongs = newsongs.OrderBy(x => x.Song_Name).ToList();

            var newmv= _musicVideoData.Get_not_main_menu_selected_mv();
            newmv = newmv.OrderBy(e=>e.Mvname).ToList();


            mian_Index.Select_new_Music = newsongs;
            mian_Index.select_new_mv = newmv;
            mian_Index.Selected_Music = _musicData.Get_Main_Index_musics();
            mian_Index.Selected_mv = _musicVideoData.Get_main_menu_mv();
            return mian_Index;
        }
        public IActionResult Index_Manage()
        {
            return View("Views/Admin Page/Index_Manage.cshtml", refresh());
        }
        #region Mv Manage
        public IActionResult Select_new_mv(int mvid)
        {
            var mv = _musicVideoData.get_musicvideo_by_id(mvid);
            var previousMV = _musicVideoData.Get_main_menu_mv();
            if (mv != null)
            {
                if (previousMV != null)
                {
                    previousMV.in_main_index = false;
                    _musicVideoData.edit_mv(previousMV);
                }
                mv.in_main_index = true;
                _musicVideoData.edit_mv(mv);
                return RedirectToAction(nameof(Index_Manage));
            }
            else
            {
                return NotFound();
            }
        }
        public IActionResult Delete_Selected_Mv(int mvid)
        {
            var mv = _musicVideoData.get_musicvideo_by_id(mvid);
            if (mv != null)
            {
                mv.in_main_index = false;
                _musicVideoData.edit_mv(mv);
                return RedirectToAction(nameof(Index_Manage));
            }
            else
            {
                return NotFound();
            }
        }
        #endregion
        public IActionResult Select_new_Music(List<int> musicid)
        {
            int music_number = _musicData.Get_Index_music_Number();
            int a = musicid.Count + music_number;
            if (musicid.Count > 0 && musicid.Count < 11 && a <= 10)
            {
                Music editmusic = new Music();
                foreach (var music in musicid)
                {
                    editmusic = _musicData.Get_Music_By_Id(music);
                    editmusic.in_main_index = true;
                    _musicData.edit_music(editmusic);
                }
                return RedirectToAction(nameof(Index_Manage));
            }
            else
            {
                ViewBag.musicmess = "تعداد آهنگ درست نیست";
                return View("Views/Admin Page/Index_Manage.cshtml", refresh());
            }
        }

        public IActionResult Delete_Selected_Music(int musicid)
        {
            var music = _musicData.Get_Music_By_Id(musicid);
            if (music != null)
            {
                music.in_main_index = false;
                bool edit_status = _musicData.edit_music(music);
                if(edit_status)
                {
                    return RedirectToAction("Index_Manage", "MainMenu");
                }
                else
                {
                    ViewBag.deletemess = "حذف با خطا مواجه شد";
                    return View("Views/Admin Page/Index_Manage.cshtml", refresh());
                }
                
            }
            else
            {
                return NotFound();
            }

        }
    }
}
